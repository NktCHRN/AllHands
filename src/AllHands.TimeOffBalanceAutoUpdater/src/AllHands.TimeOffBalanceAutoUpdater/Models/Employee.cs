using Marten.Metadata;

namespace AllHands.TimeOffBalanceAutoUpdater.Models;

public sealed class Employee : ISoftDeleted
{
    public Guid Id { get; set; }
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;
    public DateOnly WorkStartDate { get; set; }
    public bool Deleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
