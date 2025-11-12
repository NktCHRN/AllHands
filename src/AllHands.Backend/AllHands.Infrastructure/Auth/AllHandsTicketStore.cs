using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace AllHands.Infrastructure.Auth;

public sealed class AllHandsTicketStore(AuthDbContext dbContext, IDistributedCache cache, TicketSerializer ticketSerializer, TimeProvider timeProvider) : ITicketStore
{
    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var userId = Guid.Parse(ticket.Principal.Identity!.Name!);
        var serializedTicket = ticketSerializer.Serialize(ticket);
        var session = new AllHandsSession()
        {
            Key = Guid.CreateVersion7(),
            TicketValue = serializedTicket,
            UserId = userId,
            IssuesAt = ticket.Properties.IssuedUtc,
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
        var sessionId = Guid.Parse(key);
        var session = await dbContext.Sessions.FirstOrDefaultAsync(s => s.Key == sessionId);
        if (session == null)
        {
            throw new InvalidOperationException("Session was not found");
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
        var cachedTicket = await cache.GetStringAsync($"sessions:{key}");
        AllHandsSession? session;
        if (cachedTicket != null)
        {
            session = JsonSerializer.Deserialize<AllHandsSession>(cachedTicket);
            return ticketSerializer.Deserialize(session!.TicketValue);
        }

        var sessionId = Guid.Parse(key);
        session = await dbContext.Sessions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Key == sessionId);

        if (session == null || session.ExpiresAt < timeProvider.GetUtcNow())
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
        var sessionId = Guid.Parse(key);
        var session = await dbContext.Sessions.FirstOrDefaultAsync(s => s.Key == sessionId);
        if (session == null)
        {
            return;
        }
        
        session.ExpiresAt = timeProvider.GetUtcNow();
        await dbContext.SaveChangesAsync();
        
        await cache.RemoveAsync($"sessions:{session.Key}");
    }
}
