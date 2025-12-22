using AllHands.EmployeeService.Application.Dto;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.Employees.GetById;

public sealed record GetEmployeeByIdQuery(Guid Id) : IRequest<EmployeeDetailsDto>;
