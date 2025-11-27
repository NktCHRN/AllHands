using MediatR;

namespace AllHands.Application.Features.Accounts.Get;

public sealed record GetAccountsQuery() : IRequest<GetAccountsResult>;
