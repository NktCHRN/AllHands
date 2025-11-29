namespace AllHands.Application.Dto;

public sealed record EmployeeTitleDto(
    Guid Id, 
    string FirstName,
    string? MiddleName,
    string LastName,
    string Email);
    