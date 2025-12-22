using AllHands.Application.Dto;

namespace AllHands.Application.Features.Employees.GetInTimeOff;

public sealed record EmployeeInTimeOffDto(EmployeeTitleDto Employee, DateOnly StartDate, DateOnly EndDate);
