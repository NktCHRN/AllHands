using AllHands.Application.Dto;

namespace AllHands.Application.Features.TimeOffBalances.GetHistoryByEmployeeId;

public sealed record TimeOffBalancesHistoryItemDto(
    Guid BalanceId,
    Guid TypeId,
    string TypeName,
    string TypeEmoji,
    DateTimeOffset Timestamp,
    decimal Delta,
    Guid? UpdatedByEmployeeId)
{
    public EmployeeTitleDto? UpdatedBy { get; set; }
}
