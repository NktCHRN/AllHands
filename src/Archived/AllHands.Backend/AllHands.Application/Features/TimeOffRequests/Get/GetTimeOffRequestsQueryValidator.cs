using AllHands.Application.Validation;
using FluentValidation;

namespace AllHands.Application.Features.TimeOffRequests.Get;

public sealed class GetTimeOffRequestsQueryValidator : AbstractValidator<GetTimeOffRequestsQuery>
{
    public GetTimeOffRequestsQueryValidator(BasePagedQueryValidator baseValidator)
    {
        Include(baseValidator);

        RuleFor(x => x.Status)
            .IsInEnum();
    }
}
