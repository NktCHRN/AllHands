using AllHands.Domain.Abstractions;

namespace AllHands.Domain.Models;

public sealed class Employee : ISoftDeletable, ICompanyResource
{
    public required Guid Id { get; set; }
    public required string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;
    public bool IsHidden { get; set; }
    public string? AvatarFileName { get; set; }
    public string? FireReason { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public required DateTimeOffset StartedWorkingAt { get; set; }
    public DateTimeOffset? FiredAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public required Guid ManagerId { get; set; }
    public Employee? Manager { get; set; }
    public required Guid PositionId { get; set; }
    public Position? Position { get; set; }
    public required Guid CompanyId { get; set; }
    public Company? Company { get; set; }
}
