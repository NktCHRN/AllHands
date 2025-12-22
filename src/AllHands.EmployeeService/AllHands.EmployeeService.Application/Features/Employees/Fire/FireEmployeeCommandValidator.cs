using FluentValidation;

namespace AllHands.EmployeeService.Application.Features.Employees.Fire;

public sealed class FireEmployeeCommandValidator : AbstractValidator<FireEmployeeCommand>
{
    public FireEmployeeCommandValidator()
    {
        RuleFor(c => c.Reason)
            .NotEmpty()
            .MaximumLength(511);
    }
}
