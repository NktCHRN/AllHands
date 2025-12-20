using AllHands.TimeOffService.Application.Dto;

namespace AllHands.TimeOffService.Application.Features.Employees.GetInTimeOff;

public sealed record EmployeeInTimeOffDto(EmployeeTitleDto Employee, DateOnly StartDate, DateOnly EndDate);
