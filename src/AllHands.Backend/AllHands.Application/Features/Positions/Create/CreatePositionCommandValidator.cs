using FluentValidation;

namespace AllHands.Application.Features.Positions.Create;

public sealed class CreatePositionCommandValidator : AbstractValidator<CreatePositionCommand>
{
    public CreatePositionCommandValidator(PositionCommandBaseValidator baseValidator)
    {
        Include(baseValidator);
    }
}
