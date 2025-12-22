using MediatR;

namespace AllHands.Application.Features.Employees.ResendInvitation;

public sealed record ResendInvitationCommand(Guid EmployeeId) : IRequest;
