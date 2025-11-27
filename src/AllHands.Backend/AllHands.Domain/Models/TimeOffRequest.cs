using Newtonsoft.Json;

namespace AllHands.Domain.Models;

public sealed class TimeOffRequest
{
    public Guid Id { get; set; } 
    public Guid TypeId { get; set; }
    [JsonIgnore]
    public TimeOffType? Type { get; set; }
    public TimeOffRequestStatus Status { get; set; } = TimeOffRequestStatus.Pending;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal WorkingDaysCount { get; set; }
    public Guid EmployeeId { get; set; }
    [JsonIgnore]
    public Employee? Employee { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? ApproverId { get; set; }
    [JsonIgnore]
    public Employee? Approver { get; set; }
    public string? RejectionReason { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public TimeOffRequest() { }
}
