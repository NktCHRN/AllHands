using FluentValidation;

namespace AllHands.EmployeeService.Application.Features.Positions;

public sealed class PositionCommandBaseValidator : AbstractValidator<PositionCommandBase>
{
    public PositionCommandBaseValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255);
    }
}
