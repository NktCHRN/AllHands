using AllHands.Domain.EventGroupers;
using AllHands.Domain.Events.TimeOffBalance;
using AllHands.Domain.Events.TimeOff;
using DeterministicGuids;
using JasperFx.Events;
using Marten.Events.Projections;

namespace AllHands.Domain.Models;

public sealed class TimeOffBalance
{
    public Guid Id { get; internal set; }
    public Guid EmployeeId { get; internal set; }
    public Guid TypeId { get; internal set; }
    public TimeOffType? Type { get; internal set; }
    public decimal Days { get; internal set; }

    public static Guid GetId(Guid employeeId, Guid typeId) =>
        DeterministicGuid.Create(employeeId, typeId.ToString());
}

public class EmployeeTimeOffBalanceItemProjection : MultiStreamProjection<TimeOffBalance, Guid>
{
    public EmployeeTimeOffBalanceItemProjection()
    {
        Identity<IEvent<TimeOffBalanceAutomaticallyUpdated>>(x => x.StreamId);
        Identity<IEvent<TimeOffBalanceManuallyUpdated>>(x => x.StreamId);
        
        CustomGrouping(new TimeOffBalanceTimeOffRequestedEventGrouper());
        CustomGrouping(new TimeOffBalanceTimeOffRequestCancelledRejectedEventGrouper());
    }

    public TimeOffBalance Create(TimeOffRequestedEvent @event)
    {
        return new TimeOffBalance()
        {
            Id = TimeOffBalance.GetId(@event.EmployeeId, @event.TypeId),
            EmployeeId = @event.EmployeeId,
            TypeId = @event.TypeId,
            Days = -@event.WorkingDaysCount
        };
    }

    public void Apply(TimeOffRequestedEvent @event, TimeOffBalance view)
    {
        view.Days -= @event.WorkingDaysCount;
    }

    public void Apply(TimeOffRequestCancelledEvent @event, TimeOffBalance view)
    {
        view.Days += @event.WorkingDaysCount;
    }

    public void Apply(TimeOffRequestRejectedEvent @event, TimeOffBalance view)
    {
        view.Days += @event.WorkingDaysCount;
    }
    
    public TimeOffBalance Create(TimeOffBalanceAutomaticallyUpdated @event)
    {
        return new TimeOffBalance()
        {
            Id = @event.EntityId,
            Days = @event.Amount
        };
    }

    public void Apply(TimeOffBalanceAutomaticallyUpdated @event, TimeOffBalance view)
    {
        view.Days += @event.Amount;
    }
    
    public TimeOffBalance Create(TimeOffBalanceManuallyUpdated @event)
    {
        return new TimeOffBalance()
        {
            Id = @event.EntityId,
            Days = @event.Amount
        };
    }
    
    public void Apply(TimeOffBalanceManuallyUpdated @event, TimeOffBalance view)
    {
        view.Days += @event.Amount;
    }
}
