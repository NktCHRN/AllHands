using FluentValidation;

namespace AllHands.Application.Features.TimeOffTypes.Create;

public sealed class CreateTimeOffTypeCommandValidator : AbstractValidator<CreateTimeOffTypeCommand>
{
    public CreateTimeOffTypeCommandValidator(TimeOffTypeBaseCommandValidator baseValidator)
    {
        Include(baseValidator);
    }
}
