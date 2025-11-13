namespace AllHands.Application.Abstractions;

public sealed record CurrentUserDto(
    Guid Id,
    string Email,
    string? PhoneNumber,
    string FirstName,
    string? MiddleName,
    string LastName)
{
    
}
