namespace AllHands.TimeOffBalanceAutoUpdater.Events.TimeOffBalance;

public sealed record TimeOffBalanceAutomaticallyUpdated(
    Guid EntityId,
    decimal Delta)
{
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
    public Guid StreamId => EntityId;
}
