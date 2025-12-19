using System.Text.Json.Serialization;
using AllHands.Shared.Domain.Abstractions;

namespace AllHands.NewsService.Domain.Models;

public sealed class NewsPost : ISoftDeletable, IIdentifiable
{
    public required Guid Id { get; set; }
    public required string Text { get; set; }
    public required Guid AuthorId { get; set; }
    public required Guid CompanyId { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    [JsonIgnore]
    public bool Deleted { get; set; }
    [JsonIgnore]
    public DateTimeOffset? DeletedAt { get; set; }
}
