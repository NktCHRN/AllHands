using FluentValidation;
using PhoneNumbers;

namespace AllHands.Application.Features.Employees;

public sealed class EmployeeCommandBaseValidator : AbstractValidator<EmployeeCommandBase>
{
    public EmployeeCommandBaseValidator(PhoneNumberUtil phoneNumberUtil)
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(255);
        RuleFor(x => x.MiddleName)
            .MaximumLength(255);
        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(255);
        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(230)     // Technical limitation - we also need to concat it with company id for Identity accounts. 
            .EmailAddress();
        RuleFor(x => x.PhoneNumber)
            .Must(x =>
            {
                if (string.IsNullOrEmpty(x))
                {
                    return true;
                }
                
                try
                {
                    return phoneNumberUtil.IsValidNumber(phoneNumberUtil.Parse(x, null));
                }
                catch
                {
                    return false;
                }
            })
            .WithMessage("Phone number must be in E.164 format.");
    }
}
