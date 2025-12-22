namespace AllHands.EmployeeService.Application.Features.Employees.Create;

public sealed record CreateIdentityUserCommand(string Email,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? PhoneNumber,
    Guid EmployeeId);
    