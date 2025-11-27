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
        Identity<IEvent<TimeOffBalanceCreatedEvent>>(x => x.Data.EntityId);
        Identity<IEvent<TimeOffBalanceAutomaticallyUpdated>>(x => x.Data.EntityId);
        Identity<IEvent<TimeOffBalanceManuallyUpdated>>(x => x.Data.EntityId);

        Identity<IEvent<TimeOffRequestedEvent>>(x => x.Data.TimeOffBalanceId);
        Identity<IEvent<TimeOffRequestCancelledEvent>>(x => x.Data.TimeOffBalanceId);
        Identity<IEvent<TimeOffRequestRejectedEvent>>(x => x.Data.TimeOffBalanceId);
    }

    public TimeOffBalance Create(TimeOffBalanceCreatedEvent @event)
    {
        return new TimeOffBalance
        {
            Id = @event.EntityId,
            EmployeeId = @event.EmployeeId,
            TypeId = @event.TypeId,
            Days = 0
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

    public void Apply(TimeOffBalanceAutomaticallyUpdated @event, TimeOffBalance view)
    {
        view.Days += @event.Amount;
    }

    public void Apply(TimeOffBalanceManuallyUpdated @event, TimeOffBalance view)
    {
        view.Days += @event.Amount;
    }
}
