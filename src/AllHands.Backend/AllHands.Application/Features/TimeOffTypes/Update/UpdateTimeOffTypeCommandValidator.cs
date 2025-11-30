using FluentValidation;

namespace AllHands.Application.Features.TimeOffTypes.Update;

public sealed class UpdateTimeOffTypeCommandValidator : AbstractValidator<UpdateTimeOffTypeCommand>
{
    public UpdateTimeOffTypeCommandValidator(TimeOffTypeBaseCommandValidator baseValidator)
    {
        Include(baseValidator);

        RuleFor(x => x.Order)
            .GreaterThan(0);
    }
}
