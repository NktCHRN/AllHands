using FluentValidation;

namespace AllHands.TimeOffService.Application.Features.Holiday.Update;

public sealed class UpdateHolidayCommandValidator : AbstractValidator<UpdateHolidayCommand>
{
    public UpdateHolidayCommandValidator(HolidayCommandBaseValidator baseValidator)
    {
        Include(baseValidator);
    }
}
