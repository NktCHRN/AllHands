namespace AllHands.EmployeeService.Application.Features.Employees.Update;

public sealed record UpdateIdentityUserCommand(
    Guid UserId,
    string Email,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? PhoneNumber,
    Guid? RoleId);
    