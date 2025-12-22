using FluentValidation;

namespace AllHands.Application.Features.Employees.Update;

public sealed class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator(EmployeeCommandBaseValidator baseValidator)
    {
        Include(baseValidator);
    }
}
