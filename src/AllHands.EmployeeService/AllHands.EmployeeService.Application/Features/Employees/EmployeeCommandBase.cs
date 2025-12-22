namespace AllHands.EmployeeService.Application.Features.Employees;

public abstract record EmployeeCommandBase(
    Guid PositionId,
    Guid ManagerId,
    string Email, 
    string FirstName,
    string? MiddleName,
    string LastName,
    string? PhoneNumber,
    DateOnly WorkStartDate);
