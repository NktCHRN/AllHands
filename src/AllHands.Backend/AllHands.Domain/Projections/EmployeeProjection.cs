using AllHands.Domain.Events.Employee;
using AllHands.Domain.Models;
using Marten.Events.Aggregation;
// ReSharper disable UnusedMember.Global

namespace AllHands.Domain.Projections;

public sealed class EmployeeProjection : SingleStreamProjection<Employee, Guid>
{
    public Employee Create(EmployeeCreatedEvent @event)
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

    public void Apply(EmployeeDeletedEvent @event, Employee view)
    {
        view.DeletedAt = @event.OccurredAt;
    }

    public void Apply(EmployeeFiredEvent @event, Employee view)
    {
        view.UpdatedAt = @event.OccurredAt;
        view.Status = EmployeeStatus.Fired;
    }

    public void Apply(EmployeeInfoUpdatedEvent @event, Employee view)
    {
        view.UpdatedAt = @event.OccurredAt;
        view.PositionId = @event.PositionId;
        view.ManagerId = @event.ManagerId;
        view.Email = @event.Email;
        view.FirstName = @event.FirstName;
        view.MiddleName = @event.MiddleName;
        view.LastName = @event.LastName;
        view.PhoneNumber = @event.PhoneNumber;
        view.WorkStartDate = @event.WorkStartDate;
        view.AvatarFileName = @event.AvatarFileName;
    }

    public void Apply(EmployeeRegisteredEvent @event, Employee view)
    {
        view.UpdatedAt = @event.OccurredAt;
        view.FirstName = @event.FirstName;
        view.MiddleName = @event.MiddleName;
        view.LastName = @event.LastName;
        view.PhoneNumber = @event.PhoneNumber;
        view.AvatarFileName = @event.AvatarFileName;
        view.Status = EmployeeStatus.Active;
    }

    public void Apply(EmployeeRehiredEvent @event, Employee view)
    {
        view.UpdatedAt = @event.OccurredAt;
        view.Status = EmployeeStatus.Active;
    }
}
