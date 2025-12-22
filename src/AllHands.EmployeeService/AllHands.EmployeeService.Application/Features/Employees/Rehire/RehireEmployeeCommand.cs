using MediatR;

namespace AllHands.EmployeeService.Application.Features.Employees.Rehire;

public sealed record RehireEmployeeCommand(Guid EmployeeId) : IRequest;
