using MediatR;

namespace AllHands.TimeOffService.Application.Features.Employees.Save;

public sealed record SaveEmployeeCommand(
    Guid Id,
    string FirstName,
    string? MiddleName,
    string LastName,
    string Email,
    Guid CompanyId,
    DateTimeOffset EventOccurredAt,
    Guid ManagerId,
    string? Status,
    DateOnly WorkStartDate) : IRequest;
