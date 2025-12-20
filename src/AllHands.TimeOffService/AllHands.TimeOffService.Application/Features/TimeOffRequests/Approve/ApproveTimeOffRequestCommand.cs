using MediatR;

namespace AllHands.TimeOffService.Application.Features.TimeOffRequests.Approve;

public sealed record ApproveTimeOffRequestCommand(Guid Id) : IRequest;
