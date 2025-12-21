namespace AllHands.Shared.Contracts.Messaging.Events.Users;

public sealed record UserReactivatedEvent(Guid Id, Guid GlobalUserId, IReadOnlyList<Guid> RoleIds, Guid CompanyId) : IAllHandsEvent
{
    public string GroupId => Id.ToString();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
