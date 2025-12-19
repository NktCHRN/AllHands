using MediatR;

namespace AllHands.NewsService.Application.Features.Employees.Delete;

public sealed record DeleteEmployeeCommand(Guid Id, DateTimeOffset EventOccurredAt) : IRequest;
