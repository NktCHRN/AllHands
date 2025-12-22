using FluentValidation;

namespace AllHands.EmployeeService.Application.Features.Employees.Delete;

public sealed class DeleteEmployeeCommandValidator : AbstractValidator<DeleteEmployeeCommand>
{
    public DeleteEmployeeCommandValidator()
    {
        RuleFor(e => e.Reason)
            .NotEmpty()
            .MaximumLength(511);
    }
}
