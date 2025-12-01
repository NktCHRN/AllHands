namespace AllHands.Application.Features.Employees.GetInTimeOff;

public sealed record GetEmployeesInTimeOffResultItem(DateOnly Date, IReadOnlyList<EmployeeInTimeOffDto> Employee);
