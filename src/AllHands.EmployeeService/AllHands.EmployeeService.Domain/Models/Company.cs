using System.Text.Json.Serialization;
using AllHands.Shared.Domain.Abstractions;

namespace AllHands.EmployeeService.Domain.Models;

public sealed class Company : ISoftDeletable, IIdentifiable
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string EmailDomain { get; set; }
    public required string IanaTimeZone {get;set;}
    public bool IsSameDomainValidationEnforced { get; set; }
    public required ISet<DayOfWeek> WorkDays { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid? UpdatedByUserId { get; set; }
    [JsonIgnore]
    public bool Deleted { get; set; }
    [JsonIgnore]
    public DateTimeOffset? DeletedAt { get; set; }
}
