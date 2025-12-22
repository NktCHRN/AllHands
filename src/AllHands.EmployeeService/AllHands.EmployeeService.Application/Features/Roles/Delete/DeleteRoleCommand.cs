using MediatR;

namespace AllHands.EmployeeService.Application.Features.Roles.Delete;

public sealed record DeleteRoleCommand(Guid Id) : IRequest;
