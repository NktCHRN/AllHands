using AllHands.Domain.Abstractions;

namespace AllHands.Domain.Events.TimeOffBalance;

public sealed record TimeOffBalanceAutomaticallyUpdated(
    Guid EntityId,
    decimal Delta) : IDomainEvent<Guid>
{
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
    public Guid StreamId => EntityId;
}
