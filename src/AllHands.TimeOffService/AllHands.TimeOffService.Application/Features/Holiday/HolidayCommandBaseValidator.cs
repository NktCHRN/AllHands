using FluentValidation;

namespace AllHands.TimeOffService.Application.Features.Holiday;

public sealed class HolidayCommandBaseValidator : AbstractValidator<HolidayCommandBase>
{
   public HolidayCommandBaseValidator()
   {
      RuleFor(x => x.Name)
         .NotEmpty()
         .MaximumLength(255);
      RuleFor(x => x.Date)
         .Must(x => x >= DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-3)) &&
                    x <= DateOnly.FromDateTime(DateTime.UtcNow.AddYears(3)))
         .WithMessage("Holiday must be in between +- 3 years from current date.");
   }
}
