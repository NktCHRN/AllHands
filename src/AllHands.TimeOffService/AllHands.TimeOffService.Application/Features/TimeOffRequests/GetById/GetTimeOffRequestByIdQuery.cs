using MediatR;

namespace AllHands.TimeOffService.Application.Features.TimeOffRequests.GetById;

public sealed record GetTimeOffRequestByIdQuery(Guid Id) : IRequest<TimeOffRequestDto>;
