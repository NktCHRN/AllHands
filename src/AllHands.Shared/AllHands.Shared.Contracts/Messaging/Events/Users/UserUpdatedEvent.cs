namespace AllHands.Shared.Contracts.Messaging.Events.Users;

public record UserUpdatedEvent(Guid Id, IReadOnlyList<Guid> RoleIds, Guid CompanyId) : IAllHandsEvent
{
    public string GroupId => Id.ToString();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
