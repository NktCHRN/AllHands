using AllHands.Application.Queries;
using AllHands.Application.Validation;
using FluentValidation;

namespace AllHands.Application.Features.Positions.Search;

public sealed class SearchPositionsQueryValidator : AbstractValidator<PagedSearchQuery>
{
    public SearchPositionsQueryValidator()
    {
        Include(new BasePagedSearchQueryValidator());
    }
}
