using System.Text.Json.Serialization;
using AllHands.Shared.Domain.Abstractions;

namespace AllHands.EmployeeService.Domain.Models;

public sealed class Position : ICompanyResource, ISoftDeletableAuditable, IIdentifiable
{
    public required Guid Id {get; set;}
    public required string Name {get; set;}
    public required string NormalizedName {get; set;}
    public required Guid CompanyId { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid? UpdatedByUserId { get; set; }
    [JsonIgnore]
    public bool Deleted { get; set; }
    [JsonIgnore]
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedByUserId { get; set; }
}
