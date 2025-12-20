using MediatR;

namespace AllHands.AuthService.Application.Features.User.RegisterFromInvitation;

public sealed record RegisterFromInvitationCommand(
    Guid InvitationId, 
    string InvitationToken,
    string Password) : IRequest
{
}
