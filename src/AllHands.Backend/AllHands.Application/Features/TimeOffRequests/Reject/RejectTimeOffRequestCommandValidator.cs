using FluentValidation;

namespace AllHands.Application.Features.TimeOffRequests.Reject;

public sealed class RejectTimeOffRequestCommandValidator : AbstractValidator<RejectTimeOffRequestCommand>
{
    public RejectTimeOffRequestCommandValidator()
    {
        RuleFor(request => request.Reason)
            .NotEmpty()
            .MaximumLength(255);
    }
}
