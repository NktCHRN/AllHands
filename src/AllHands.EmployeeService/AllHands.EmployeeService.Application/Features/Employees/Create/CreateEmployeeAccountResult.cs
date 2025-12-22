namespace AllHands.EmployeeService.Application.Features.Employees.Create;

public sealed record CreateEmployeeAccountResult(Guid Id, Guid InvitationId, string InvitationToken);
