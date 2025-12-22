using FluentValidation;

namespace AllHands.EmployeeService.Application.Features.Company.Update;

public sealed class UpdateCompanyValidator : AbstractValidator<UpdateCompanyCommand>
{
    public UpdateCompanyValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255);
        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(4000);
        RuleFor(x => x.EmailDomain)
            .NotEmpty()
            .MaximumLength(255);
        RuleFor(x => x.IanaTimeZone)
            .NotEmpty()
            .Must(tz => TimeZoneInfo.TryFindSystemTimeZoneById(tz, out _))
            .WithMessage($"{nameof(UpdateCompanyCommand.IanaTimeZone)} must be a valid IANA timezone.");
        RuleForEach(x => x.WorkDays)
            .IsInEnum();
    }
}
