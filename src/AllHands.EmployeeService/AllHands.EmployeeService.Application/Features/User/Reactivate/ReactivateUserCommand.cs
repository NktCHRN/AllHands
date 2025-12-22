using MediatR;

namespace AllHands.EmployeeService.Application.Features.User.Reactivate;

public sealed record ReactivateUserCommand(Guid UserId, Guid GlobalUserId, IReadOnlyList<Guid> RoleIds) : IRequest;
