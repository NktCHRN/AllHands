using AllHands.Shared.Domain.Abstractions;

namespace AllHands.TimeOffService.Domain.Events.TimeOffBalance;

public sealed record TimeOffBalanceAutomaticallyUpdated(
    Guid EntityId,
    decimal Delta) : IDomainEvent<Guid>
{
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
    public Guid StreamId => EntityId;
}
