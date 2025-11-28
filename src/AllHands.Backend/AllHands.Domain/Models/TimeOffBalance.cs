using DeterministicGuids;
using System.Text.Json.Serialization;

namespace AllHands.Domain.Models;

public sealed class TimeOffBalance
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid TypeId { get; set; }
    [JsonIgnore]
    public TimeOffType? Type { get; set; }
    public decimal Days { get; set; }

    public DateTimeOffset? LastAutoUpdate { get; set; }
}
