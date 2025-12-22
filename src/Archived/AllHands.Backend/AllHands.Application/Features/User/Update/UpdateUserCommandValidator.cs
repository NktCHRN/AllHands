using FluentValidation;
using PhoneNumbers;

namespace AllHands.Application.Features.User.Update;

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator(PhoneNumberUtil phoneNumberUtil)
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(255);
        RuleFor(x => x.MiddleName)
            .NotEmpty()
            .MaximumLength(255);
        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(255);
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
