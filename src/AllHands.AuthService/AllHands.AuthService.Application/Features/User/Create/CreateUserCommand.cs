namespace AllHands.AuthService.Application.Features.User.Create;

public sealed record CreateUserCommand(
    string Email,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? PhoneNumber,
    Guid EmployeeId)
    : UserCommandBase(Email, FirstName, MiddleName, LastName, PhoneNumber);
