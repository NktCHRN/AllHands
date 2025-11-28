using AllHands.Domain.Abstractions;

namespace AllHands.Domain.Models;

public sealed class Position : ICompanyResource, ISoftDeletable
{
    public required Guid Id {get; set;}
    public required string Name {get; set;}
    public required string NormalizedName {get; set;}
    public required Guid CompanyId { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid? UpdatedByUserId { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedByUserId { get; set; }
}
