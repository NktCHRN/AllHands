using MediatR;

namespace AllHands.AuthService.Application.Features.User.ResendInvitation;

public sealed record ResendInvitationCommand(Guid EmployeeId) : IRequest;
