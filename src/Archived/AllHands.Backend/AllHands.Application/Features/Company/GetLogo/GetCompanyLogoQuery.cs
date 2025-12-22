using MediatR;

namespace AllHands.Application.Features.Company.GetLogo;

public sealed record GetCompanyLogoQuery() : IRequest<GetCompanyLogoResult>;
