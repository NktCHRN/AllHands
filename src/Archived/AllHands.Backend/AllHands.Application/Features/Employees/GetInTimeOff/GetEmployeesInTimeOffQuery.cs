using MediatR;

namespace AllHands.Application.Features.Employees.GetInTimeOff;

public sealed record GetEmployeesInTimeOffQuery(DateOnly Start, DateOnly End) : IRequest<GetEmployeesInTimeOffResult>;
