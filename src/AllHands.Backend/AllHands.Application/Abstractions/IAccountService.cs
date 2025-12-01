using AllHands.Application.Dto;
using AllHands.Application.Features.Employees.Create;
using AllHands.Application.Features.Employees.Update;
using AllHands.Application.Features.User.ChangePassword;
using AllHands.Application.Features.User.Login;
using AllHands.Application.Features.User.RegisterFromInvitation;
using AllHands.Application.Features.User.ResetPassword;
using AllHands.Application.Features.User.Update;

namespace AllHands.Application.Abstractions;

public interface IAccountService
{
    Task<LoginResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<Guid> RegisterFromInvitationAsync(RegisterFromInvitationCommand command, CancellationToken cancellationToken = default);
    Task ReloginAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task<GenerateResetPasswordTokenResult> GenerateResetPasswordToken(string email, CancellationToken cancellationToken);
    Task ChangePassword(ChangePasswordCommand command, CancellationToken cancellationToken);
    Task<IReadOnlyList<Guid>> GetUserIds(Guid currentUserId);
    Task UpdateAsync(UpdateUserCommand command, Guid userId, CancellationToken cancellationToken);
    Task<RoleDto?> GetRoleByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    Task<CreateEmployeeAccountResult> CreateAsync(CreateEmployeeCommand command,
        CancellationToken cancellationToken);

    Task<InvitationCreationResult> RegenerateInvitationAsync(Guid userId, CancellationToken cancellationToken);
    Task UpdateAsync(UpdateEmployeeCommand command, Guid userId, CancellationToken cancellationToken);
    Task DeactivateAsync(Guid userId, CancellationToken cancellationToken);
    Task ReactivateAsync(Guid userId, CancellationToken cancellationToken);
}
