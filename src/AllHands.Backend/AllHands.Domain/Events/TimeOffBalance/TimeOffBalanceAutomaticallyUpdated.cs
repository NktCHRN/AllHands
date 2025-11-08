namespace AllHands.Domain.Events.TimeOffBalance;

public sealed record TimeOffBalanceAutomaticallyUpdated(
    Guid EntityId,
    decimal Amount)
{
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;   
}
