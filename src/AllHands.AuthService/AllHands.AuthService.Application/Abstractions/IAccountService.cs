using AllHands.AuthService.Application.Features.User.ChangePassword;
using AllHands.AuthService.Application.Features.User.Create;
using AllHands.AuthService.Application.Features.User.Login;
using AllHands.AuthService.Application.Features.User.RegisterFromInvitation;
using AllHands.AuthService.Application.Features.User.Update;

namespace AllHands.AuthService.Application.Abstractions;

public interface IAccountService
{
    Task<LoginResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<Guid> RegisterFromInvitationAsync(RegisterFromInvitationCommand command, CancellationToken cancellationToken = default);
    Task ReloginAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task GenerateResetPasswordToken(string email, CancellationToken cancellationToken);
    Task ChangePassword(ChangePasswordCommand command, CancellationToken cancellationToken);

    Task RegenerateInvitationAsync(Guid employeeId, CancellationToken cancellationToken);
    Task<CreateUserAccountResult> CreateAsync(CreateUserCommand command,
        CancellationToken cancellationToken);
    Task<UpdateUserResult> UpdateAsync(UpdateUserCommand command, CancellationToken cancellationToken);
    Task DeactivateAsync(Guid userId, CancellationToken cancellationToken);
    Task ReactivateAsync(Guid userId, CancellationToken cancellationToken);
    Task DeleteAsync(Guid userId, CancellationToken cancellationToken);
}
