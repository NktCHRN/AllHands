using MediatR;

namespace AllHands.TimeOffService.Application.Features.TimeOffRequests.Cancel;

public sealed record CancelTimeOffRequestCommand(Guid Id) : IRequest;
