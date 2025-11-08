using AllHands.Domain.Abstractions;
using AllHands.Domain.Events.Employee;

namespace AllHands.Domain.Models;

public sealed class Employee : ISoftDeletable, ICompanyResource
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string? MiddleName { get; private set; }
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public EmployeeStatus Status { get; private set; } = EmployeeStatus.Active;
    public string? AvatarFileName { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    public DateTimeOffset WorkStartDate { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public Guid ManagerId { get; private set; }
    public Employee? Manager { get; private set; }
    public Guid PositionId { get; private set; }
    public Position? Position { get; private set; }
    public Guid CompanyId { get; private set; }
    public Company? Company { get; private set; }
    
    public Guid UserId { get; private set; }

    private Employee() { }
    
    public static Employee Create(EmployeeCreatedEvent @event)
    {
        return new Employee()
        {
            Id = @event.EntityId, 
            UserId = @event.UserId,
            CompanyId = @event.CompanyId,
            PositionId = @event.PositionId,
            ManagerId = @event.ManagerId,
            Email = @event.Email, 
            FirstName = @event.FirstName,
            MiddleName = @event.MiddleName,
            LastName = @event.LastName,
            PhoneNumber = @event.PhoneNumber,
            WorkStartDate = @event.WorkStartDate,
            Status = EmployeeStatus.Unactivated,
            CreatedAt = @event.OccurredAt
        };
    }

    public void Apply(EmployeeDeletedEvent @event)
    {
        DeletedAt = @event.OccurredAt;
    }

    public void Apply(EmployeeFiredEvent @event)
    {
        UpdatedAt = @event.OccurredAt;
        Status = EmployeeStatus.Fired;
    }

    public void Apply(EmployeeInfoUpdatedEvent @event)
    {
        UpdatedAt = @event.OccurredAt;
        PositionId = @event.PositionId;
        ManagerId = @event.ManagerId;
        Email = @event.Email;
        FirstName = @event.FirstName;
        MiddleName = @event.MiddleName;
        LastName = @event.LastName;
        PhoneNumber = @event.PhoneNumber;
        WorkStartDate = @event.WorkStartDate;
        AvatarFileName = @event.AvatarFileName;
    }

    public void Apply(EmployeeRegisteredEvent @event)
    {
        UpdatedAt = @event.OccurredAt;
        FirstName = @event.FirstName;
        MiddleName = @event.MiddleName;
        LastName = @event.LastName;
        PhoneNumber = @event.PhoneNumber;
        AvatarFileName = @event.AvatarFileName;
        Status = EmployeeStatus.Active;
    }

    public void Apply(EmployeeRehiredEvent @event)
    {
        UpdatedAt = @event.OccurredAt;
        Status = EmployeeStatus.Active;
    }
}
