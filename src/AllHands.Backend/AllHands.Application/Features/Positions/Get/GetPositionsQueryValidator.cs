using AllHands.Application.Queries;
using AllHands.Application.Validation;
using FluentValidation;

namespace AllHands.Application.Features.Positions.Get;

public sealed class GetPositionsQueryValidator : AbstractValidator<PagedSearchQuery>
{
    public GetPositionsQueryValidator()
    {
        Include(new BasePagedSearchQueryValidator());
    }
}
