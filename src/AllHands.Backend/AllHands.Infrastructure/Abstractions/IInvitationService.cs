using AllHands.Infrastructure.Auth;

namespace AllHands.Infrastructure.Abstractions;

public interface IInvitationService
{
    Task CreateAsync(Guid userId, Guid issuerId, CancellationToken cancellationToken);
    Task<UseInvitationResult> UseAsync(Guid id, string invitationToken, CancellationToken cancellationToken);
}
