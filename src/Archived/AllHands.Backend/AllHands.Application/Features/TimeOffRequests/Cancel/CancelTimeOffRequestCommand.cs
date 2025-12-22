using MediatR;

namespace AllHands.Application.Features.TimeOffRequests.Cancel;

public sealed record CancelTimeOffRequestCommand(Guid Id) : IRequest;
