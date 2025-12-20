using AllHands.TimeOffService.Application.Dto;

namespace AllHands.TimeOffService.Application.Features.TimeOffBalances.GetHistoryByEmployeeId;

public sealed record TimeOffBalancesHistoryItemDto(
    Guid BalanceId,
    Guid TypeId,
    string TypeName,
    string TypeEmoji,
    DateTimeOffset Timestamp,
    decimal Delta,
    Guid? UpdatedByEmployeeId,
    TimeOffBalancesHistoryItemType ChangeType)
{
    public EmployeeTitleDto? UpdatedBy { get; set; }
}
