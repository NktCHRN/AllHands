using Marten.Metadata;

namespace AllHands.TimeOffBalanceAutoUpdater.Models;

public sealed class TimeOffType : ISoftDeleted
{
    public required Guid Id { get; set; }
    public required decimal DaysPerYear { get; set; }
    public bool Deleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
