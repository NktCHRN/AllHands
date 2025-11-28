using System.Security.Claims;

namespace AllHands.Infrastructure.Auth;

public interface ITicketModifier
{
    Task UpdateClaimsAsync(AuthDbContext dbContext, Guid userId,
        Func<IReadOnlyList<Claim>> createNewClaims, CancellationToken cancellationToken);
}