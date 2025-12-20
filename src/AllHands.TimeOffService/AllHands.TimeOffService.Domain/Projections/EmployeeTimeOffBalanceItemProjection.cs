using AllHands.TimeOffService.Domain.Events.TimeOffBalance;
using AllHands.TimeOffService.Domain.Models;
using JasperFx.Events;
using Marten.Events.Projections;

// ReSharper disable UnusedMember.Global

namespace AllHands.TimeOffService.Domain.Projections;

public sealed class EmployeeTimeOffBalanceItemProjection : MultiStreamProjection<TimeOffBalance, Guid>
{
    public EmployeeTimeOffBalanceItemProjection()
    {
        Identity<IEvent<TimeOffBalanceCreatedEvent>>(x => x.Data.EntityId);
        Identity<IEvent<TimeOffBalanceAutomaticallyUpdated>>(x => x.Data.EntityId);
        Identity<IEvent<TimeOffBalanceManuallyUpdated>>(x => x.Data.EntityId);
        Identity<IEvent<TimeOffBalancePerYearUpdatedEvent>>(x => x.Data.EntityId);
        Identity<IEvent<TimeOffBalanceRequestChangeEvent>>(x => x.Data.EntityId);
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

    public void Apply(TimeOffBalanceManuallyUpdated @event, TimeOffBalance view)
    {
        view.Days += @event.Delta;
    }

    public void Apply(TimeOffBalancePerYearUpdatedEvent @event, TimeOffBalance view)
    {
        if (@event.UpdateType == TimeOffPerYearUpdateType.Reset)
        {
            view.DaysPerYear = @event.Amount.GetValueOrDefault();
        }

        if (@event.UpdateType == TimeOffPerYearUpdateType.Update)
        {
            view.DaysPerYear += @event.Delta.GetValueOrDefault();
            if (view.DaysPerYear < 0)
            {
                view.DaysPerYear = 0;
            }
        }
    }

    public void Apply(TimeOffBalanceRequestChangeEvent @event, TimeOffBalance view)
    {
        view.Days += @event.Delta;
    }
}
