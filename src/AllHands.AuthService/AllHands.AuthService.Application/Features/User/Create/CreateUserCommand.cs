namespace AllHands.AuthService.Application.Features.User.Create;

public sealed record CreateUserCommand(
    Guid PositionId,
    Guid ManagerId,
    string Email,
    string FirstName,
    string? MiddleName,
    string LastName,
    string? PhoneNumber,
    DateOnly WorkStartDate,
    Guid EmployeeId)
    : UserCommandBase(PositionId, ManagerId, Email, FirstName, MiddleName, LastName, PhoneNumber, WorkStartDate);
