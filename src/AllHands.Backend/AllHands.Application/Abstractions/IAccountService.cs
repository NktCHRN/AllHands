using AllHands.Application.Features.User.Login;
using AllHands.Application.Features.User.RegisterFromInvitation;
using AllHands.Application.Features.User.Relogin;

namespace AllHands.Application.Abstractions;

public interface IAccountService
{
    Task<LoginResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<Guid> RegisterFromInvitationAsync(RegisterFromInvitationCommand command, CancellationToken cancellationToken = default);
    Task ReloginAsync(Guid companyId, CancellationToken cancellationToken = default);
}
