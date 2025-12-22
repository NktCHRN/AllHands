using MediatR;

namespace AllHands.Application.Features.TimeOffRequests.GetById;

public sealed record GetTimeOffRequestByIdQuery(Guid Id) : IRequest<TimeOffRequestDto>;
