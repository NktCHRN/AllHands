namespace AllHands.Domain.Abstractions;

public abstract record AuditableEvent(Guid PerformedByIdentityId, Guid PerformedByEmployeeId)
{
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;   
}
