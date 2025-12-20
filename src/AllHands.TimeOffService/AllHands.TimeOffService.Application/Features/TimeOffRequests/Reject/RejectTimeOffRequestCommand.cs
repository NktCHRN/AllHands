using MediatR;

namespace AllHands.TimeOffService.Application.Features.TimeOffRequests.Reject;

public record RejectTimeOffRequestCommand(string Reason) : IRequest
{
    public Guid Id { get; set; }
}
