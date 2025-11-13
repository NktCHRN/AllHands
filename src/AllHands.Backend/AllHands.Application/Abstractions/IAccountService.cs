namespace AllHands.Application.Abstractions;

public interface IAccountService
{
    Task<LoginResult> LoginAsync(string login, string password, CancellationToken cancellationToken = default);
}