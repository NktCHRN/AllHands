using MediatR;

namespace AllHands.TimeOffService.Application.Features.Companies.Save;

public sealed record SaveCompanyCommand(
    Guid Id,
    string Name,
    string IanaTimeZone,
    ISet<DayOfWeek> WorkDays,
    DateTimeOffset EventOccurredAt) : IRequest;
