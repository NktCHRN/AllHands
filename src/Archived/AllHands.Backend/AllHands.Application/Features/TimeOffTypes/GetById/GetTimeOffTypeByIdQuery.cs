using MediatR;

namespace AllHands.Application.Features.TimeOffTypes.GetById;

public sealed record GetTimeOffTypeByIdQuery(Guid Id) : IRequest<TimeOffTypeDto>;
