namespace AllHands.AuthService.Application.Features.Employees.Create;

public sealed record CreateEmployeeAccountResult(Guid Id, Guid RoleId, Guid GlobalUserId);
