using MediatR;

namespace AllHands.TimeOffService.Application.Features.Holiday.Get;

public sealed record GetHolidaysQuery(DateOnly Start, DateOnly End) : IRequest<GetHolidaysResult>;
