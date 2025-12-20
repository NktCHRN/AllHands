namespace AllHands.TimeOffService.Application.Features.TimeOffBalances.GetHistoryByEmployeeId;

public enum TimeOffBalancesHistoryItemType
{
    Undefined = 0,
    ManualAdjustment = 1,
    AutoUpdate = 2,
    TimeOffRequest = 3,
    TimeOffRequestCancellationOrRejection = 4
}
