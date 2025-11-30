using FluentValidation;

namespace AllHands.Application.Features.Holiday.Create;

public sealed class CreateHolidayCommandValidator : AbstractValidator<CreateHolidayCommand>
{
    public CreateHolidayCommandValidator(HolidayCommandBaseValidator baseValidator)
    {
        Include(baseValidator);
    }
}
