using AllHands.Domain.Events.TimeOff;

namespace AllHands.Domain.Models;

public sealed class TimeOffRequest
{
    public Guid Id { get; private set; } 
    public Guid TypeId { get; private set; }
    public TimeOffType? Type { get; set; }
    public TimeOffRequestStatus Status { get; private set; } = TimeOffRequestStatus.Pending;
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public decimal WorkingDaysCount { get; private set; }
    public Guid EmployeeId { get; private set; }
    public Employee? Employee { get; set; }
    public Guid? ApproverId { get; private set; }
    public Employee? Approver { get; set; }
    public string? RejectionReason { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private TimeOffRequest() { }
    
    public static TimeOffRequest Create(TimeOffRequestedEvent @event)
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

    public void Apply(TimeOffRequestApprovedEvent @event)
    {
        Status = TimeOffRequestStatus.Approved;
        ApproverId = @event.PerformedByEmployeeId;
    }

    public void Apply(TimeOffRequestCancelledEvent @event)
    {
        Status = TimeOffRequestStatus.Cancelled;
    }

    public void Apply(TimeOffRequestRejectedEvent @event)
    {
        Status = TimeOffRequestStatus.Rejected;
        ApproverId = @event.PerformedByEmployeeId;
        RejectionReason = @event.Reason;
    }
}
