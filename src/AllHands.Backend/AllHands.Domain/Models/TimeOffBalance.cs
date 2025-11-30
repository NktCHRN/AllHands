using System.Text.Json.Serialization;
using AllHands.Domain.Abstractions;

namespace AllHands.Domain.Models;

public sealed class TimeOffBalance : IIdentifiable
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid TypeId { get; set; }
    [JsonIgnore]
    public TimeOffType? Type { get; set; }
    public decimal Days { get; set; }
    public decimal DaysPerYear { get; set; }

    public DateTimeOffset? LastAutoUpdate { get; set; }
}
