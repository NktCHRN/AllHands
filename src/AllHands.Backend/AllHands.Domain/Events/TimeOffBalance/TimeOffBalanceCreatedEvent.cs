using AllHands.Domain.Abstractions;

namespace AllHands.Domain.Events.TimeOffBalance;

public sealed record TimeOffBalanceCreatedEvent(Guid EntityId, Guid EmployeeId, Guid TypeId) : IDomainEvent<Guid>
{
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
    public Guid StreamId => EntityId;
}
