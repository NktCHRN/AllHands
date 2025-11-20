using System.Security.Cryptography;
using AllHands.Domain.Exceptions;
using AllHands.Infrastructure.Abstractions;
using AllHands.Infrastructure.Auth.Entities;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AllHands.Infrastructure.Auth;

public sealed class InvitationService(AuthDbContext dbContext, TimeProvider timeProvider, IOptions<InvitationTokenProviderOptions> optionsContainer) : IInvitationService
{
    private const int WorkFactor = 12;
    private readonly InvitationTokenProviderOptions _options = optionsContainer.Value;
    
    private const string Alphanumeric = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private const int TokenLength = 32;
    
    public async Task CreateAsync(Guid userId, Guid issuerId, CancellationToken cancellationToken)
    {
        var currentUtcTime = timeProvider.GetUtcNow();
        
        var latestValidCreationDateTime = currentUtcTime.AddSeconds(-_options.TokenRecreationTimeoutInSeconds);
        var invitationInTimeoutRange = await dbContext.Invitations.FirstOrDefaultAsync(
            i => i.UserId == userId && i.IssuedAt > latestValidCreationDateTime,
            cancellationToken);
        if (invitationInTimeoutRange is not null)
        {
            throw new EntityAlreadyExistsException($"This user was already invited. " +
                                                   $"Please wait {(invitationInTimeoutRange.IssuedAt - latestValidCreationDateTime).Humanize(2)} to create a new invitation.");
        }
        
        var token = RandomNumberGenerator.GetString(Alphanumeric, TokenLength);     
        // TODO: Send email with token here.
        
        var invitation = new Invitation()
        {
            Id = Guid.CreateVersion7(),
            TokenHash = Hash(token),
            IssuedAt = currentUtcTime,
            ExpiresAt = currentUtcTime.AddMinutes(_options.LifeTimeInMinutes),
            UserId = userId,
            IssuerId = issuerId,
        };
        
        await dbContext.Invitations.AddAsync(invitation, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    
    private static string Hash(string invitationToken)
    {
        return BCrypt.Net.BCrypt.HashPassword(invitationToken, WorkFactor, true);
    }
    
    public async Task<UseInvitationResult> UseAsync(Guid id, string invitationToken, CancellationToken cancellationToken)
    {
        var invitation = await dbContext.Invitations.FirstOrDefaultAsync(i => i.Id == id, cancellationToken: cancellationToken);
        var isTokenCorrect = invitation is not null && Verify(invitationToken, invitation.TokenHash);
        if (!isTokenCorrect)
        {
            throw new UserUnauthorizedException("Invalid invitation token.");
        }

        if (invitation!.IsUsed)
        {
            throw new UserUnauthorizedException("Invitation is already used.");
        }

        if (invitation.ExpiresAt < timeProvider.GetUtcNow())
        {
            throw new UserUnauthorizedException("Invitation token is expired. Please, ask your company's HR department to generate a new one.");
        }
                
        invitation.IsUsed = true;
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return new UseInvitationResult(invitation.UserId);
    }

    private static bool Verify(string invitationToken, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(invitationToken, hash, true);
    }
}
