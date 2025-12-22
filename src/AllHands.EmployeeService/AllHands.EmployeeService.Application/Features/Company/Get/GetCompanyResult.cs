namespace AllHands.EmployeeService.Application.Features.Company.Get;

public sealed record GetCompanyResult(
    Guid Id,
    string Name,
    string? Description,
    string EmailDomain,
    bool IsSameDomainValidationEnforced,
    string IanaTimeZone,
    IReadOnlyList<DayOfWeek> WorkDays,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    DateTimeOffset? DeletedAt);
    