using FluentValidation;

namespace AllHands.EmployeeService.Application.Features.Positions.Create;

public sealed class CreatePositionCommandValidator : AbstractValidator<CreatePositionCommand>
{
    public CreatePositionCommandValidator(PositionCommandBaseValidator baseValidator)
    {
        Include(baseValidator);
    }
}
