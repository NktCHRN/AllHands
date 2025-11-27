namespace AllHands.Application.Features.Accounts.Get;

public sealed record GetAccountsResult(IReadOnlyList<AccountDto> Accounts);
