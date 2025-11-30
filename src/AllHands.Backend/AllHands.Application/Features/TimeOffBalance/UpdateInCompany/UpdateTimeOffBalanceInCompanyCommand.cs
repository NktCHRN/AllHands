using MediatR;

namespace AllHands.Application.Features.TimeOffBalance.UpdateInCompany;

public sealed record UpdateTimeOffBalanceInCompanyCommand(Guid CompanyId) : IRequest;
