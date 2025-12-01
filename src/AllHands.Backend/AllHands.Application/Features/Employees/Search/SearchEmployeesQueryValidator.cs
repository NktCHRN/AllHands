using AllHands.Application.Validation;
using FluentValidation;

namespace AllHands.Application.Features.Employees.Search;

public sealed class SearchEmployeesQueryValidator : AbstractValidator<SearchEmployeesQuery>
{
    public SearchEmployeesQueryValidator(BasePagedSearchQueryValidator baseValidator)
    {
        Include(baseValidator);
    }
}
