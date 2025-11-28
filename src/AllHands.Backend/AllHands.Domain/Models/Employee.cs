using AllHands.Domain.Abstractions;
using System.Text.Json.Serialization;

namespace AllHands.Domain.Models;

public sealed class Employee : ISoftDeletable, ICompanyResource
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NormalizedEmail { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;
    public string? AvatarFileName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateOnly WorkStartDate { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid ManagerId { get; set; }
    [JsonIgnore]
    public Employee? Manager { get; set; }
    public Guid PositionId { get; set; }
    [JsonIgnore]
    public Position? Position { get; set; }
    public Guid CompanyId { get; set; }
    [JsonIgnore]
    public Company? Company { get; set; }
    
    public Guid UserId { get; set; }

    public Employee() { }
}
