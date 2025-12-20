namespace AllHands.TimeOffService.Application.Features.Employees.GetInTimeOff;

public sealed record GetEmployeesInTimeOffResultItem(DateOnly Date, IReadOnlyList<EmployeeInTimeOffDto> Employee);
