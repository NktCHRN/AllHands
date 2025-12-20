using MediatR;

namespace AllHands.AuthService.Application.Features.Employees.UpdateRole;

public sealed record UpdateEmployeeRoleCommand(Guid UserId, Guid RoleId) : IRequest
{
}
