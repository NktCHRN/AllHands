using MediatR;

namespace AllHands.Application.Features.TimeOffBalances.UpdateInCompany;

public sealed record UpdateTimeOffBalanceInCompanyCommand(Guid CompanyId) : IRequest;
