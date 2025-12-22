namespace AllHands.EmployeeService.Application.Features.Employees.Create;

public sealed record SendCompleteRegistrationEmailCommand(
    string Email,
    string FirstName,
    string AdminName,
    Guid InvitationId,
    string Token);
    