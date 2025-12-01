using MediatR;

namespace AllHands.Application.Features.TimeOffRequests.Reject;

public record RejectTimeOffRequestCommand(string Reason) : IRequest
{
    public Guid Id { get; set; }
}
