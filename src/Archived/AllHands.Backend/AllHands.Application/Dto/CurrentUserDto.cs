namespace AllHands.Application.Dto;

public sealed record CurrentUserDto(
    Guid Id,
    string Email,
    string? PhoneNumber,
    string FirstName,
    string? MiddleName,
    string LastName,
    Guid CompanyId)
{
    
}
