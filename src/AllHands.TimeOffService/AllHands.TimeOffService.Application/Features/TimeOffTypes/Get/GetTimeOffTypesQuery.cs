using MediatR;

namespace AllHands.TimeOffService.Application.Features.TimeOffTypes.Get;

public record GetTimeOffTypesQuery() : IRequest<GetTimeOffTypesResult>;
