using MediatR;

namespace AllHands.EmployeeService.Application.Features.Accounts.Get;

public sealed record GetAccountsQuery() : IRequest<GetAccountsResult>;
