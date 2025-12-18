namespace AllHands.Shared.Contracts.Messaging.Events.Companies;

public sealed record CompanyUpdatedEvent(
    Guid Id, 
    string Name,
    string? Description,
    string EmailDomain,
    string IanaTimeZone,
    bool IsSameDomainValidationEnforced,
    ISet<DayOfWeek> WorkDays) : IAllHandsEvent
{
    public string GroupId => Id.ToString();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
