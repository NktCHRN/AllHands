using MediatR;

namespace AllHands.TimeOffService.Application.Features.Employees.UpdateStatus;

public sealed record UpdateEmployeeStatusCommand(
    Guid Id,
    string Status,
    DateTimeOffset EventOccurredAt) : IRequest;
