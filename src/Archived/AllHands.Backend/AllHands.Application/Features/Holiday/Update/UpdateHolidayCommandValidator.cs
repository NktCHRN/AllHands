using FluentValidation;

namespace AllHands.Application.Features.Holiday.Update;

public sealed class UpdateHolidayCommandValidator : AbstractValidator<UpdateHolidayCommand>
{
    public UpdateHolidayCommandValidator(HolidayCommandBaseValidator baseValidator)
    {
        Include(baseValidator);
    }
}
