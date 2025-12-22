using FluentValidation;

namespace AllHands.EmployeeService.Application.Features.Employees.Update;

public sealed class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator(EmployeeCommandBaseValidator baseValidator)
    {
        Include(baseValidator);
    }
}
