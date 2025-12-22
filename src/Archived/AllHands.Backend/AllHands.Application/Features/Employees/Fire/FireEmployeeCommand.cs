using MediatR;

namespace AllHands.Application.Features.Employees.Fire;

public sealed record FireEmployeeCommand(string Reason) : IRequest
{
    public Guid EmployeeId { get; set; }    
}
