namespace AllHands.Shared.Contracts.Messaging.Events.Roles;

public sealed record RoleCreatedEvent(Guid Id, string Name, Guid CompanyId, bool IsDefault) : IAllHandsEvent
{
    public string GroupId => Id.ToString();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
