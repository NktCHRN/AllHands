using AllHands.TimeOffBalanceAutoUpdater.Events.TimeOffBalance;
using AllHands.TimeOffBalanceAutoUpdater.Models;
using JasperFx.Events;
using Marten.Events.Projections;

// ReSharper disable UnusedMember.Global

namespace AllHands.TimeOffBalanceAutoUpdater.Projections;

public sealed class EmployeeTimeOffBalanceItemProjection : MultiStreamProjection<TimeOffBalance, Guid>
{
    public EmployeeTimeOffBalanceItemProjection()
    {
        Identity<IEvent<TimeOffBalanceCreatedEvent>>(x => x.Data.EntityId);
        Identity<IEvent<TimeOffBalanceAutomaticallyUpdated>>(x => x.Data.EntityId);
    }

    public TimeOffBalance Create(TimeOffBalanceCreatedEvent @event)
    {
        return new TimeOffBalance
        {
            Id = @event.EntityId,
            EmployeeId = @event.EmployeeId,
            TypeId = @event.TypeId,
            Days = 0,
            DaysPerYear = @event.DaysPerYer
        };
    }

    public void Apply(TimeOffBalanceAutomaticallyUpdated @event, TimeOffBalance view)
    {
        view.Days += @event.Delta;
        view.LastAutoUpdate = @event.OccurredAt;
    }
}
