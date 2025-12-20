using MediatR;

namespace AllHands.TimeOffService.Application.Features.Employees.Delete;

public sealed record DeleteEmployeeCommand(Guid Id) : IRequest;
