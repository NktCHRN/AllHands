using System.Security.Claims;
using System.Text.Json;
using AllHands.Domain.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace AllHands.Infrastructure.Auth;

public sealed class AllHandsTicketStore(IDbContextFactory<AuthDbContext> dbContextFactory, IDistributedCache cache, TicketSerializer ticketSerializer, TimeProvider timeProvider) : ITicketStore, ITicketModifier
{
    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var userId = Guid.Parse(ticket.Principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("Unexpected NameIdentifier."));
        var serializedTicket = ticketSerializer.Serialize(ticket);
        var session = new AllHandsSession()
        {
            Key = Guid.NewGuid(),
            TicketValue = serializedTicket,
            UserId = userId,
            IssuedAt = ticket.Properties.IssuedUtc,
            ExpiresAt = ticket.Properties.ExpiresUtc
        };
        await dbContext.AddAsync(session);
        await dbContext.SaveChangesAsync();

        await cache.SetStringAsync($"sessions:{session.Key}", JsonSerializer.Serialize(session), new DistributedCacheEntryOptions()
        {
            AbsoluteExpiration = session.ExpiresAt
        });
        
        return session.Key.ToString();
    }

    public async Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var sessionId = Guid.Parse(key);
        var session = await dbContext.Sessions.FirstOrDefaultAsync(s => s.Key == sessionId);
        if (session == null)
        {
            throw new UserUnauthorizedException("Session was not found");
        }
        
        session.TicketValue = ticketSerializer.Serialize(ticket);
        session.ExpiresAt = ticket.Properties.ExpiresUtc;
        
        await dbContext.SaveChangesAsync();
        
        await cache.SetStringAsync($"sessions:{session.Key}", JsonSerializer.Serialize(session), new DistributedCacheEntryOptions()
        {
            AbsoluteExpiration = session.ExpiresAt
        });
    }

    public async Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var cachedTicket = await cache.GetStringAsync($"sessions:{key}");
        AllHandsSession? session;
        if (cachedTicket != null)
        {
            session = JsonSerializer.Deserialize<AllHandsSession>(cachedTicket);
            return session!.IsRevoked
                ? null
                : ticketSerializer.Deserialize(session.TicketValue);
        }

        var sessionId = Guid.Parse(key);
        session = await dbContext.Sessions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Key == sessionId);

        if (session == null || session.ExpiresAt < timeProvider.GetUtcNow() || session.IsRevoked)
        {
            return null;
        }
        
        await cache.SetStringAsync($"sessions:{session.Key}", JsonSerializer.Serialize(session), new DistributedCacheEntryOptions()
        {
            AbsoluteExpiration = session.ExpiresAt
        });
        return ticketSerializer.Deserialize(session.TicketValue);
    }

    public async Task RemoveAsync(string key)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var sessionId = Guid.Parse(key);
        var session = await dbContext.Sessions.FirstOrDefaultAsync(s => s.Key == sessionId);
        if (session == null)
        {
            return;
        }
        
        session.ExpiresAt = timeProvider.GetUtcNow();
        session.IsRevoked = true;
        await dbContext.SaveChangesAsync();
        
        await cache.RemoveAsync($"sessions:{session.Key}");
    }

    public async Task UpdateClaimsAsync(AuthDbContext dbContext, Guid userId,
        Func<IReadOnlyList<Claim>> createNewClaims, bool takeOnlyNewClaims, CancellationToken cancellationToken)
    {
        var activeSessions = await GetActiveSessions(dbContext, userId, cancellationToken);

        foreach (var session in activeSessions)
        {
            var ticketValue = ticketSerializer.Deserialize(session.TicketValue);
            var newClaims = createNewClaims();
            var claims = takeOnlyNewClaims 
                ? new List<Claim>() 
                : ticketValue!.Principal.Claims
                .Where(c => newClaims.All(nc => c.Type != nc.Type))
                .ToList();
            claims.AddRange(newClaims);
            var newPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
            var newTicket = new AuthenticationTicket(
                newPrincipal, 
                ticketValue!.Properties, 
                ticketValue.AuthenticationScheme
            );
            session.ExpiresAt = session.ExpiresAt?.AddMinutes(1);
            newTicket.Properties.ExpiresUtc = session.ExpiresAt;
            session.TicketValue = ticketSerializer.Serialize(newTicket);
            
            await cache.SetStringAsync($"sessions:{session.Key}", JsonSerializer.Serialize(session), new DistributedCacheEntryOptions()
            {
                AbsoluteExpiration = session.ExpiresAt
            }, token: cancellationToken);
        };
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task ExpireActiveSessionsAsync(AuthDbContext dbContext, Guid userId, CancellationToken cancellationToken)
    {
        var currentDateTime = timeProvider.GetUtcNow();
        var activeSessions = await GetActiveSessions(dbContext, userId, cancellationToken);

        foreach (var session in activeSessions)
        {
            session.ExpiresAt = currentDateTime;
            session.IsRevoked = true;
            
            await cache.SetStringAsync($"sessions:{session.Key}", JsonSerializer.Serialize(session), new DistributedCacheEntryOptions()
            {
                AbsoluteExpiration = session.ExpiresAt?.AddMinutes(1)       // Just to prevent failures and weird cache overwrites before saving changes to DB. Token is revoked anyway.
            }, token: cancellationToken);
        };
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    
    private async Task<IReadOnlyList<AllHandsSession>> GetActiveSessions(AuthDbContext dbContext, Guid userId, CancellationToken cancellationToken)
    {
        var currentDateTime = timeProvider.GetUtcNow();

        var sessions = await dbContext.Sessions
            .Where(x => x.UserId == userId && x.ExpiresAt >= currentDateTime && !x.IsRevoked)
            .ToListAsync(cancellationToken: cancellationToken);

        return sessions;
    }
}
