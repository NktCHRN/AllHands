using AllHands.Application.Dto;
using MediatR;

namespace AllHands.Application.Features.Employees.GetById;

public sealed record GetEmployeeByIdQuery(Guid Id) : IRequest<EmployeeDetailsDto>;
