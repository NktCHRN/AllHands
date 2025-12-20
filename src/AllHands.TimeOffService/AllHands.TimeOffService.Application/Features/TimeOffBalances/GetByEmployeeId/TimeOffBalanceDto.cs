namespace AllHands.TimeOffService.Application.Features.TimeOffBalances.GetByEmployeeId;

public sealed record TimeOffBalanceDto(
    Guid TypeId,
    string TypeName,
    string TypeEmoji,
    decimal Days,
    decimal DaysPerYear);
