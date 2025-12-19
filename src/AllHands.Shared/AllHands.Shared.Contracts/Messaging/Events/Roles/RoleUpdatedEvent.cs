namespace AllHands.Shared.Contracts.Messaging.Events.Roles;

public sealed record RoleUpdatedEvent(Guid Id, string Name, Guid CompanyId) : IAllHandsEvent
{
    public string GroupId => Id.ToString();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
