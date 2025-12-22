using AllHands.Application.Dto;
using AllHands.Application.Features.TimeOffTypes;
using AllHands.Domain.Models;

namespace AllHands.Application.Features.TimeOffRequests;

public sealed record TimeOffRequestDto(Guid Id, DateOnly StartDate, DateOnly EndDate, TimeOffTypeDto Type, TimeOffRequestStatus Status, decimal WorkingDaysCount, EmployeeTitleDto? Approver, string? RejectionReason)
{
    public EmployeeTitleDto? Employee {get; set;}

    public static TimeOffRequestDto FromModel(TimeOffRequest model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return new TimeOffRequestDto(
            model.Id,
            model.StartDate,
            model.EndDate,
            model.Type != null
            ? TimeOffTypeDto.FromModel(model.Type)
            : null!,
            model.Status,
            model.WorkingDaysCount,
            model.Approver is not null 
            ? EmployeeTitleDto.FromModel(model.Approver)
            : null,
            model.RejectionReason)
        {
            Employee = model.Employee is not null
            ? EmployeeTitleDto.FromModel(model.Employee)
            : null
        };
    }
}
