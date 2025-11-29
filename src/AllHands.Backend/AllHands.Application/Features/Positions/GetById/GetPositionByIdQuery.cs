using AllHands.Application.Dto;
using MediatR;

namespace AllHands.Application.Features.Positions.GetById;

public record GetPositionByIdQuery(Guid Id) : IRequest<PositionDto>;
