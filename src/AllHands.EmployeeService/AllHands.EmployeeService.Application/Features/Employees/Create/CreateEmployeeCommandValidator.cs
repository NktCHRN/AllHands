using FluentValidation;

namespace AllHands.EmployeeService.Application.Features.Employees.Create;

public sealed class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator(EmployeeCommandBaseValidator baseValidator)
    {
        Include(baseValidator);
    }
}
