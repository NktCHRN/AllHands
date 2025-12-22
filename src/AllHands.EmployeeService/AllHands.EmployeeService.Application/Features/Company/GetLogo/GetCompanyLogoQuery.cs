using MediatR;

namespace AllHands.EmployeeService.Application.Features.Company.GetLogo;

public sealed record GetCompanyLogoQuery() : IRequest<GetCompanyLogoResult>;
