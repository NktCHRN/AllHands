using MediatR;

namespace AllHands.Application.Features.Employees.Rehire;

public sealed record RehireEmployeeCommand(Guid EmployeeId) : IRequest;
