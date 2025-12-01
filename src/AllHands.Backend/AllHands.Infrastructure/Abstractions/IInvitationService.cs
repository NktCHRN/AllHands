using AllHands.Application.Dto;

namespace AllHands.Infrastructure.Abstractions;

public interface IInvitationService
{
    Task<InvitationCreationResult> CreateAsync(Guid userId, Guid issuerId, CancellationToken cancellationToken);
    Task UseAsync(Guid id, string invitationToken, CancellationToken cancellationToken);
}
