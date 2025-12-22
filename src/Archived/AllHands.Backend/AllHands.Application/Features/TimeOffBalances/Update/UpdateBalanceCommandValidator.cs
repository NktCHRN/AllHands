using FluentValidation;

namespace AllHands.Application.Features.TimeOffBalances.Update;

public sealed class UpdateBalanceCommandValidator : AbstractValidator<UpdateBalanceCommand>
{
    public UpdateBalanceCommandValidator()
    {
        RuleFor(c => c.Delta)
            .LessThanOrEqualTo(365 * 3)
            .GreaterThanOrEqualTo(-365 * 3);
        RuleFor(c => c.DaysPerYear)
            .LessThanOrEqualTo(365);
        RuleFor(c => c.Reason)
            .NotEmpty()
            .MaximumLength(255);
    }
}
