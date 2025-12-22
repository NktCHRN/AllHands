using MediatR;

namespace AllHands.EmployeeService.Application.Features.Employees.Delete;

public sealed record DeleteEmployeeCommand(string Reason) : IRequest
{
    public Guid EmployeeId { get; set; }
}
