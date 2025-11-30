using AllHands.Application.Abstractions;
using FluentValidation;

namespace AllHands.Application.Features.TimeOffTypes;

public sealed class TimeOffTypeBaseCommandValidator : AbstractValidator<TimeOffTypeBaseCommand>
{
    public TimeOffTypeBaseCommandValidator(ITimeOffEmojiValidator timeOffEmojiValidator)
    {
        RuleFor(t => t.Name)
            .NotEmpty()
            .MaximumLength(255);
        
        RuleFor(t => t.Emoji)
            .NotEmpty()
            .Must(timeOffEmojiValidator.IsAllowed)
            .WithMessage("The emoji is not allowed. Please, pick emoji from a list of allowed ones.");

        RuleFor(t => t.DaysPerYear)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(365);
    }
}
