using MediatR;

namespace AllHands.EmployeeService.Application.Features.Company.Update;

public sealed record UpdateCompanyCommand(
    string Name, 
    string? Description, 
    string EmailDomain,
    bool IsSameDomainValidationEnforced,
    string IanaTimeZone,
    IReadOnlyList<DayOfWeek> WorkDays) : IRequest;
