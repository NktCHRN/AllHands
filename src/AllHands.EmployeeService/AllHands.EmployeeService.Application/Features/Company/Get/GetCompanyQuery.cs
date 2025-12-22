using MediatR;

namespace AllHands.EmployeeService.Application.Features.Company.Get;

public sealed record GetCompanyQuery : IRequest<GetCompanyResult>;
