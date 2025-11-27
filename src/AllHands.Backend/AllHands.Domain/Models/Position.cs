using AllHands.Domain.Abstractions;

namespace AllHands.Domain.Models;

public sealed class Position : ICompanyResource, ISoftDeletable
{
    public required Guid Id {get; set;}
    public required string Name {get; set;}
    public required string NormalizedName {get; set;}
    public required Guid CompanyId { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
