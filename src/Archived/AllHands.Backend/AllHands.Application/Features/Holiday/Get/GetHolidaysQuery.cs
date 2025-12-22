using MediatR;

namespace AllHands.Application.Features.Holiday.Get;

public sealed record GetHolidaysQuery(DateOnly Start, DateOnly End) : IRequest<GetHolidaysResult>;
