using MediatR;

namespace AllHands.Application.Features.User.RegisterFromInvitation;

public sealed record RegisterFromInvitationCommand(
    Guid InvitationId, 
    string InvitationToken,
    string Password) : IRequest
{
}
