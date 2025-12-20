using AllHands.Shared.Application.Validation;
using FluentValidation;

namespace AllHands.TimeOffService.Application.Features.TimeOffBalances.GetHistoryByEmployeeId;

public sealed class GetTimeOffBalancesHistoryQueryValidator : AbstractValidator<GetTimeOffBalancesHistoryQuery>
{
    public GetTimeOffBalancesHistoryQueryValidator(BasePagedQueryValidator baseValidator)
    {
        Include(baseValidator);
    }
}
