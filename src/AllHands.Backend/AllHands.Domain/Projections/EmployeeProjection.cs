using AllHands.Domain.Events.Employee;
using AllHands.Domain.Models;
using JasperFx.Events;
using JasperFx.Events.Daemon;
using Marten.Events.Aggregation;
// ReSharper disable UnusedMember.Global

namespace AllHands.Domain.Projections;

public sealed class EmployeeProjection : SingleStreamProjection<Employee, Guid>
{
    public EmployeeProjection()
    {
        IncludeType<EmployeeCreatedEvent>();
        IncludeType<EmployeeUpdatedEvent>();
        IncludeType<EmployeeUpdatedBySelfEvent>();
        IncludeType<EmployeeFiredEvent>();
        IncludeType<EmployeeDeletedEvent>();
        IncludeType<EmployeeRegisteredEvent>();
        IncludeType<EmployeeRehiredEvent>();
        IncludeType<EmployeeAvatarUpdated>();
    }
    
    public override (Employee?, ActionType) DetermineAction(Employee? snapshot, Guid identity,
        IReadOnlyList<IEvent> events)
    {
        var actionType = ActionType.Store;
 
        if (snapshot == null && events.HasNoEventsOfType<EmployeeCreatedEvent>())
        {
            return (snapshot, ActionType.Nothing);
        }
 
        var eventData = events.ToQueueOfEventData();
        while (eventData.Count != 0)
        {
            var data = eventData.Dequeue();
            switch (data)
            {
                case EmployeeCreatedEvent @event:
                    snapshot = new Employee()
                    {
                        Id = @event.EntityId, 
                        UserId = @event.UserId,
                        CompanyId = @event.CompanyId,
                        PositionId = @event.PositionId,
                        ManagerId = @event.ManagerId,
                        Email = @event.Email, 
                        NormalizedEmail = @event.NormalizedEmail,
                        FirstName = @event.FirstName,
                        MiddleName = @event.MiddleName,
                        LastName = @event.LastName,
                        PhoneNumber = @event.PhoneNumber,
                        WorkStartDate = @event.WorkStartDate,
                        Status = EmployeeStatus.Unactivated,
                        CreatedAt = @event.OccurredAt
                    };
                    break;
 
                case EmployeeUpdatedEvent @event when snapshot is { Deleted: false }:
                    if (actionType == ActionType.StoreThenSoftDelete) continue;
                    snapshot.UpdatedAt = @event.OccurredAt;
                    snapshot.PositionId = @event.PositionId;
                    snapshot.ManagerId = @event.ManagerId;
                    snapshot.Email = @event.Email;
                    snapshot.FirstName = @event.FirstName;
                    snapshot.MiddleName = @event.MiddleName;
                    snapshot.LastName = @event.LastName;
                    snapshot.PhoneNumber = @event.PhoneNumber;
                    snapshot.WorkStartDate = @event.WorkStartDate;
                    snapshot.NormalizedEmail = @event.NormalizedEmail;
                    break;
                
                case EmployeeUpdatedBySelfEvent @event when snapshot is { Deleted: false }:
                    if (actionType == ActionType.StoreThenSoftDelete) continue;
                    snapshot.UpdatedAt = @event.OccurredAt;
                    snapshot.FirstName = @event.FirstName;
                    snapshot.MiddleName = @event.MiddleName;
                    snapshot.LastName = @event.LastName;
                    snapshot.PhoneNumber = @event.PhoneNumber;
                    break;
                
                case EmployeeFiredEvent @event when snapshot is { Deleted: false }:
                    if (actionType == ActionType.StoreThenSoftDelete) continue;
                    snapshot.UpdatedAt = @event.OccurredAt;
                    snapshot.Status = EmployeeStatus.Fired;
                    break;
                
                case EmployeeRegisteredEvent @event when snapshot is { Deleted: false }:
                    if (actionType == ActionType.StoreThenSoftDelete) continue;
                    snapshot.UpdatedAt = @event.OccurredAt;
                    snapshot.Status = EmployeeStatus.Active;
                    break;
                
                case EmployeeRehiredEvent @event when snapshot is { Deleted: false }:
                    if (actionType == ActionType.StoreThenSoftDelete) continue;
                    snapshot.UpdatedAt = @event.OccurredAt;
                    snapshot.Status = EmployeeStatus.Active;
                    break;
                
                case EmployeeAvatarUpdated @event when snapshot is { Deleted: false }:
                    if (actionType == ActionType.StoreThenSoftDelete) continue;
                    snapshot.UpdatedAt = @event.OccurredAt;
                    break;
 
                case EmployeeDeletedEvent @event when snapshot is { Deleted: false }:
                    snapshot.Deleted = true;
                    snapshot.DeletedAt = @event.OccurredAt;
                    actionType = ActionType.StoreThenSoftDelete;
                    break;
            }
        }
 
        return (snapshot, actionType);
    }
}
