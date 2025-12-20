using System.Security.Claims;

namespace AllHands.AuthService.Infrastructure.Auth;

public interface ITicketModifier
{
    Task UpdateClaimsAsync(AuthDbContext dbContext, Guid userId,
        Func<IReadOnlyList<Claim>> createNewClaims, bool takeOnlyNewClaims = false,
        CancellationToken cancellationToken = default);

    Task ExpireActiveSessionsAsync(AuthDbContext dbContext, Guid userId, CancellationToken cancellationToken);
}