using AllHands.Application.Features.User.RegisterFromInvitation;

namespace AllHands.Application.Abstractions;

public interface IAccountService
{
    Task<LoginResult> LoginAsync(string login, string password, CancellationToken cancellationToken = default);
    Task RegisterFromInvitationAsync(RegisterFromInvitationCommand command, CancellationToken cancellationToken = default);
}