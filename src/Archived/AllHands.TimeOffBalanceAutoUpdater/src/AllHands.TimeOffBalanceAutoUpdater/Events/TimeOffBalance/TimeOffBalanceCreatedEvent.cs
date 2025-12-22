namespace AllHands.TimeOffBalanceAutoUpdater.Events.TimeOffBalance;

public sealed record TimeOffBalanceCreatedEvent(Guid EntityId, Guid EmployeeId, Guid TypeId, decimal DaysPerYer)
{
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
    public Guid StreamId => EntityId;
}
