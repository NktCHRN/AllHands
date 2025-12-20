namespace AllHands.AuthService.Application.Features.Employees.Create;

public sealed record CreateEmployeeCommand(
    Guid PositionId,
    Guid ManagerId,
    string Email,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? PhoneNumber,
    DateOnly WorkStartDate,
    Guid EmployeeId)
    : EmployeeCommandBase(PositionId, ManagerId, Email, FirstName, MiddleName, LastName, PhoneNumber, WorkStartDate);
