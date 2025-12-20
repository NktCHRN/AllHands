using AllHands.Shared.Contracts.Messaging.Events;

namespace AllHands.Auth.Contracts.Messaging;

public record CompanySessionsRecalculationRequestedEvent(Guid CompanyId, Guid RequesterId) : IAllHandsEvent
{
    public string GroupId => CompanyId.ToString();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
