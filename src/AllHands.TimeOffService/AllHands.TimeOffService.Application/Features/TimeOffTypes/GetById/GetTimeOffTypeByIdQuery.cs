using MediatR;

namespace AllHands.TimeOffService.Application.Features.TimeOffTypes.GetById;

public sealed record GetTimeOffTypeByIdQuery(Guid Id) : IRequest<TimeOffTypeDto>;
