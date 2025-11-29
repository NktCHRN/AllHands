using FluentValidation;

namespace AllHands.Application.Features.Positions;

public sealed class PositionCommandBaseValidator : AbstractValidator<PositionCommandBase>
{
    public PositionCommandBaseValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255);
    }
}
