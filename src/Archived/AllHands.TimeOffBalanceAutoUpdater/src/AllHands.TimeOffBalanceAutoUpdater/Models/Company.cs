using Marten.Metadata;

namespace AllHands.TimeOffBalanceAutoUpdater.Models;

public sealed class Company : ISoftDeleted
{
    public required Guid Id { get; set; }
    public required string IanaTimeZone {get;set;}
    public bool Deleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
