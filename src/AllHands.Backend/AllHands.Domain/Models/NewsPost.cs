using AllHands.Domain.Abstractions;

namespace AllHands.Domain.Models;

public sealed class NewsPost : ISoftDeletable, IIdentifiable
{
    public required Guid Id { get; set; }
    public required string Text { get; set; }
    public required Guid AuthorId { get; set; }
    public required Guid CompanyId { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public bool Deleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
