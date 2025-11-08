namespace AllHands.Domain.Abstractions;

public abstract record AuditableEvent(Guid EntityId, Guid PerformedByUserId)
{
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;   
}
