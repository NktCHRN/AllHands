using MediatR;

namespace AllHands.Application.Features.Employees.Delete;

public sealed record DeleteEmployeeCommand(string Reason) : IRequest
{
    public Guid EmployeeId { get; set; }
}
