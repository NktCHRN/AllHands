using System.Text.Json.Serialization;
using AllHands.Shared.Domain.Abstractions;

namespace AllHands.NewsService.Domain.Models;

public sealed class Employee : ISoftDeletable, ICompanyResource, IIdentifiable
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    [JsonIgnore]
    public bool Deleted { get; set; }
    [JsonIgnore]
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid CompanyId { get; set; }

    public Employee() { }
}
