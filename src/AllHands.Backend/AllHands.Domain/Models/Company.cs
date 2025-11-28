using AllHands.Domain.Abstractions;

namespace AllHands.Domain.Models;

public sealed class Company : ISoftDeletable
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string EmailDomain { get; set; }
    public required string IanaTimeZone {get;set;}
    public required ISet<DayOfWeek> WorkDays { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
