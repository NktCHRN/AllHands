using AllHands.Shared.Application.Validation;
using FluentValidation;

namespace AllHands.EmployeeService.Application.Features.Employees.Search;

public sealed class SearchEmployeesQueryValidator : AbstractValidator<SearchEmployeesQuery>
{
    public SearchEmployeesQueryValidator(BasePagedSearchQueryValidator baseValidator)
    {
        Include(baseValidator);

        RuleFor(x => x.Status)
            .IsInEnum();
    }
}
