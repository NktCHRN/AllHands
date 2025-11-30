using MediatR;

namespace AllHands.Application.Features.TimeOffTypes.Get;

public record GetTimeOffTypesQuery() : IRequest<GetTimeOffTypesResult>;
