using AllHands.Application.Dto;
using AllHands.Domain.Models;

namespace AllHands.Application.Features.TimeOffRequests;

public sealed record TimeOffRequestDto(Guid Id, DateTime Start, DateTime End, Guid TypeId, TimeOffRequestStatus Status, decimal WorkingDaysCount, EmployeeTitleDto? Approver)
{
    public EmployeeTitleDto? Employee {get; set;}    
}
