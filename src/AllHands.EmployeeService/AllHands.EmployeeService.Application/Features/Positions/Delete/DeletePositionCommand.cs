using MediatR;

namespace AllHands.EmployeeService.Application.Features.Positions.Delete;

public sealed record DeletePositionCommand(Guid Id) : IRequest;
