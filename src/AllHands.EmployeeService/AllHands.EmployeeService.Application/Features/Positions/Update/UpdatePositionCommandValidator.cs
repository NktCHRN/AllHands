using FluentValidation;

namespace AllHands.EmployeeService.Application.Features.Positions.Update;

public sealed class UpdatePositionCommandValidator : AbstractValidator<PositionCommandBase>
{
    public UpdatePositionCommandValidator(PositionCommandBaseValidator baseValidator)
    {
        Include(baseValidator);
    }
}
