namespace AllHands.Shared.Contracts.Messaging.Events.Invitations;

public sealed record InvitationAcceptedEvent(Guid Id, Guid UserId, Guid CompanyId) : IAllHandsEvent
{
    public string GroupId => Id.ToString();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
