using AllHands.Domain.Abstractions;
using AllHands.Domain.Events.Employee;

namespace AllHands.Domain.Models;

public sealed class Employee : ISoftDeletable, ICompanyResource
{
    public Guid Id { get; internal set; }
    public string FirstName { get; internal set; } = string.Empty;
    public string? MiddleName { get; internal set; }
    public string LastName { get; internal set; } = string.Empty;
    public string Email { get; internal set; } = string.Empty;
    public string? PhoneNumber { get; internal set; }
    public EmployeeStatus Status { get; internal set; } = EmployeeStatus.Active;
    public string? AvatarFileName { get; internal set; }
    public DateTimeOffset CreatedAt { get; internal set; }
    public DateTimeOffset? UpdatedAt { get; internal set; }
    public DateTimeOffset WorkStartDate { get; internal set; }
    public DateTimeOffset? DeletedAt { get; internal set; }
    public Guid ManagerId { get; internal set; }
    public Employee? Manager { get; internal set; }
    public Guid PositionId { get; internal set; }
    public Position? Position { get; internal set; }
    public Guid CompanyId { get; internal set; }
    public Company? Company { get; internal set; }
    
    public Guid UserId { get; internal set; }

    internal Employee() { }
}
