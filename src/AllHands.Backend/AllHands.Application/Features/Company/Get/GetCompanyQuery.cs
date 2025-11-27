using MediatR;

namespace AllHands.Application.Features.Company.Get;

public sealed record GetCompanyQuery : IRequest<GetCompanyResult>;
