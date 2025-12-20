using AllHands.AuthService.Application.Dto;
using AllHands.AuthService.Application.Features.Employees.Create;
using AllHands.AuthService.Application.Features.Employees.Update;
using AllHands.AuthService.Application.Features.User.ChangePassword;
using AllHands.AuthService.Application.Features.User.Login;
using AllHands.AuthService.Application.Features.User.RegisterFromInvitation;
using AllHands.AuthService.Application.Features.User.ResetPassword;
using AllHands.AuthService.Application.Features.User.Update;

namespace AllHands.AuthService.Application.Abstractions;

public interface IAccountService
{
    Task<LoginResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<Guid> RegisterFromInvitationAsync(RegisterFromInvitationCommand command, CancellationToken cancellationToken = default);
    Task ReloginAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task GenerateResetPasswordToken(string email, CancellationToken cancellationToken);
    Task ChangePassword(ChangePasswordCommand command, CancellationToken cancellationToken);

    Task<CreateEmployeeAccountResult> CreateAsync(CreateEmployeeCommand command,
        CancellationToken cancellationToken);

    Task RegenerateInvitationAsync(Guid employeeId, CancellationToken cancellationToken);
    Task UpdateAsync(UpdateEmployeeCommand command, Guid userId, CancellationToken cancellationToken);
    Task DeactivateAsync(Guid userId, CancellationToken cancellationToken);
    Task ReactivateAsync(Guid userId, CancellationToken cancellationToken);
    Task DeleteAsync(Guid userId, CancellationToken cancellationToken);
}
