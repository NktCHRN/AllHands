using System.Text.Json.Serialization;
using AllHands.Shared.Domain.Abstractions;

namespace AllHands.TimeOffService.Domain.Models;

public sealed class Company : ISoftDeletable, IIdentifiable
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string IanaTimeZone {get;set;}
    public required ISet<DayOfWeek> WorkDays { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    [JsonIgnore]
    public bool Deleted { get; set; }
    [JsonIgnore]
    public DateTimeOffset? DeletedAt { get; set; }
}
