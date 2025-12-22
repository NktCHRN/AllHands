using AllHands.Application.Queries;
using FluentValidation;

namespace AllHands.Application.Validation;

public class BasePagedSearchQueryValidator : AbstractValidator<PagedSearchQuery>
{
    public BasePagedSearchQueryValidator()
    {
        Include(new BasePagedQueryValidator());
    }
}
