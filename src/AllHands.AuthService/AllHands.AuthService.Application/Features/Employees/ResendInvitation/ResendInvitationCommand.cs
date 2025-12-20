using MediatR;

namespace AllHands.AuthService.Application.Features.Employees.ResendInvitation;

public sealed record ResendInvitationCommand(Guid EmployeeId) : IRequest;
