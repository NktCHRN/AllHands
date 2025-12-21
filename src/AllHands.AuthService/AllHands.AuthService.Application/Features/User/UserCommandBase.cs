namespace AllHands.AuthService.Application.Features.User;

public abstract record UserCommandBase(
    Guid PositionId,
    Guid ManagerId,
    string Email, 
    string FirstName,
    string? MiddleName,
    string LastName,
    string? PhoneNumber,
    DateOnly WorkStartDate);
