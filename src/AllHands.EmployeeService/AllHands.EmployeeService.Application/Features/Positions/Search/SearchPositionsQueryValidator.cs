using AllHands.Shared.Application.Queries;
using AllHands.Shared.Application.Validation;
using FluentValidation;

namespace AllHands.EmployeeService.Application.Features.Positions.Search;

public sealed class SearchPositionsQueryValidator : AbstractValidator<PagedSearchQuery>
{
    public SearchPositionsQueryValidator()
    {
        Include(new BasePagedSearchQueryValidator());
    }
}
