namespace AllHands.Domain.Abstractions;

public abstract record AuditableEvent(Guid EntityId, Guid PerformedByUserId) : IDomainEvent<Guid>
{
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
    public virtual Guid StreamId =>  EntityId;
}
