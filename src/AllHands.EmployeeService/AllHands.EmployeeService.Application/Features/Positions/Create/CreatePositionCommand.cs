using AllHands.EmployeeService.Application.Dto;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.Positions.Create;

public sealed record CreatePositionCommand(string Name) : PositionCommandBase(Name), IRequest<CreatedEntityDto>;
