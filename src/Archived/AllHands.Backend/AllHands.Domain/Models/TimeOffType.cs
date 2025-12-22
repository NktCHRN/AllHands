using AllHands.Domain.Abstractions;

namespace AllHands.Domain.Models;

public sealed class TimeOffType : IIdentifiable, ISoftDeletable
{
    public required Guid Id { get; set; }
    public required Guid CompanyId { get; set; }
    public required int Order { get; set; }
    public required string Name { get; set; }
    public required string Emoji { get; set; }
    public required decimal DaysPerYear { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid? UpdatedByUserId { get; set; }
    public bool Deleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedByUserId { get; set; }
}
