namespace AllHands.Shared.Contracts.Messaging.Events.Roles;

public sealed record RoleDeletedEvent(Guid Id, Guid ReplacementRoleId, Guid CompanyId) : IAllHandsEvent
{
    public string GroupId => Id.ToString();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
