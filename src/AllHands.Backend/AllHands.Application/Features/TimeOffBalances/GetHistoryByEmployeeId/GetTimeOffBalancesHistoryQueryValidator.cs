using AllHands.Application.Validation;
using FluentValidation;

namespace AllHands.Application.Features.TimeOffBalances.GetHistoryByEmployeeId;

public sealed class GetTimeOffBalancesHistoryQueryValidator : AbstractValidator<GetTimeOffBalancesHistoryQuery>
{
    public GetTimeOffBalancesHistoryQueryValidator(BasePagedQueryValidator baseValidator)
    {
        Include(baseValidator);
    }
}
