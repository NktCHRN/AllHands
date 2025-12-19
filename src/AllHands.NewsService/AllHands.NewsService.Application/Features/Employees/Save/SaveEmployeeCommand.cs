using MediatR;

namespace AllHands.NewsService.Application.Features.Employees.Save;

public sealed record SaveEmployeeCommand(
    Guid Id,
    string FirstName,
    string? MiddleName,
    string LastName,
    string Email,
    Guid CompanyId,
    DateTimeOffset EventOccurredAt) : IRequest;
