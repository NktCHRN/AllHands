namespace AllHands.Domain.Models;

public sealed class TimeOffRequest
{
    public Guid Id { get; internal set; } 
    public Guid TypeId { get; internal set; }
    public TimeOffType? Type { get; set; }
    public TimeOffRequestStatus Status { get; internal set; } = TimeOffRequestStatus.Pending;
    public DateOnly StartDate { get; internal set; }
    public DateOnly EndDate { get; internal set; }
    public decimal WorkingDaysCount { get; internal set; }
    public Guid EmployeeId { get; internal set; }
    public Employee? Employee { get; set; }
    public Guid? ApproverId { get; internal set; }
    public Employee? Approver { get; set; }
    public string? RejectionReason { get; internal set; }
    public DateTimeOffset CreatedAt { get; internal set; }

    internal TimeOffRequest() { }
}
