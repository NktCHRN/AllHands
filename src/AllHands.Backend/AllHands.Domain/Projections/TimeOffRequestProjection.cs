using AllHands.Domain.Events.TimeOff;
using AllHands.Domain.Models;
using Marten.Events.Aggregation;

namespace AllHands.Domain.Projections;
// ReSharper disable UnusedMember.Global

public sealed class TimeOffRequestProjection : SingleStreamProjection<TimeOffRequest, Guid>
{
    public TimeOffRequest Create(TimeOffRequestedEvent @event)
    {
        return new TimeOffRequest()
        {
            Id = @event.EntityId,
            EmployeeId = @event.EmployeeId,
            TypeId = @event.TypeId,
            StartDate = @event.StartDate,
            EndDate = @event.EndDate,
            WorkingDaysCount = @event.WorkingDaysCount,     // Could be 0.5, 3.5 etc., like last day is 0.5.
        };
    }

    public void Apply(TimeOffRequestApprovedEvent @event, TimeOffRequest view)
    {
        view.Status = TimeOffRequestStatus.Approved;
        view.ApproverId = @event.PerformedByEmployeeId;
    }

    public void Apply(TimeOffRequestCancelledEvent @event, TimeOffRequest view)
    {
        view.Status = TimeOffRequestStatus.Cancelled;
    }

    public void Apply(TimeOffRequestRejectedEvent @event, TimeOffRequest view)
    {
        view.Status = TimeOffRequestStatus.Rejected;
        view.ApproverId = @event.PerformedByEmployeeId;
        view.RejectionReason = @event.Reason;
    }
}
