using AllHands.EmployeeService.Application.Dto;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.Positions.GetById;

public record GetPositionByIdQuery(Guid Id) : IRequest<PositionDto>;
