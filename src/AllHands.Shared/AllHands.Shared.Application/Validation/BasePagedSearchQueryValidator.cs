using AllHands.Shared.Application.Queries;
using FluentValidation;

namespace AllHands.Shared.Application.Validation;

public class BasePagedSearchQueryValidator : AbstractValidator<PagedSearchQuery>
{
    public BasePagedSearchQueryValidator()
    {
        Include(new BasePagedQueryValidator());
    }
}
