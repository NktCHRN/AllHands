using AllHands.Domain.EventGroupers;
using AllHands.Domain.Events.TimeOff;
using AllHands.Domain.Events.TimeOffBalance;
using AllHands.Domain.Models;
using JasperFx.Events;
using Marten.Events.Projections;
// ReSharper disable UnusedMember.Global

namespace AllHands.Domain.Projections;

public sealed class EmployeeTimeOffBalanceItemProjection : MultiStreamProjection<TimeOffBalance, Guid>
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
