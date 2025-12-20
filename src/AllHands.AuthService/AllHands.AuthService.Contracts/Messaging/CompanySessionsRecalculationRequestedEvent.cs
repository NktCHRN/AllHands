namespace AllHands.Auth.Contracts.Messaging;

public record CompanySessionsRecalculationRequestedEvent(Guid CompanyId, Guid RequesterId)
{
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
