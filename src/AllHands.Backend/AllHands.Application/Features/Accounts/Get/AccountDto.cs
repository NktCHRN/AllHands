using AllHands.Application.Dto;

namespace AllHands.Application.Features.Accounts.Get;

public sealed record AccountDto(
    Guid EmployeeId,
    string FirstName,
    string? MiddleName,
    string LastName,
    string Email,
    CompanyDto Company);
