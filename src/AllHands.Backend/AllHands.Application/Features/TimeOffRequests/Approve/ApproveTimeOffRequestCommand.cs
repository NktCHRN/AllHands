using MediatR;

namespace AllHands.Application.Features.TimeOffRequests.Approve;

public sealed record ApproveTimeOffRequestCommand(Guid Id) : IRequest;
