using AllHands.Application.Features.User.ChangePassword;
using AllHands.Application.Features.User.Login;
using AllHands.Application.Features.User.RegisterFromInvitation;
using AllHands.Application.Features.User.ResetPassword;

namespace AllHands.Application.Abstractions;

public interface IAccountService
{
    Task<LoginResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<Guid> RegisterFromInvitationAsync(RegisterFromInvitationCommand command, CancellationToken cancellationToken = default);
    Task ReloginAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task<GenerateResetPasswordTokenResult> GenerateResetPasswordToken(string email, CancellationToken cancellationToken);
    Task ChangePassword(ChangePasswordCommand command, CancellationToken cancellationToken);
    Task<IReadOnlyList<Guid>> GetUserIds(Guid currentUserId);
}
