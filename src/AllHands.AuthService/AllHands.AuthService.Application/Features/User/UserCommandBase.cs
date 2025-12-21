namespace AllHands.AuthService.Application.Features.User;

public abstract record UserCommandBase(
    string Email, 
    string FirstName,
    string? MiddleName,
    string LastName,
    string? PhoneNumber);
