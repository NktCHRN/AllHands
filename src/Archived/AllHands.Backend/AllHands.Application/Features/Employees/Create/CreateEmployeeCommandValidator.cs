using FluentValidation;

namespace AllHands.Application.Features.Employees.Create;

public sealed class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator(EmployeeCommandBaseValidator baseValidator)
    {
        Include(baseValidator);
    }
}
