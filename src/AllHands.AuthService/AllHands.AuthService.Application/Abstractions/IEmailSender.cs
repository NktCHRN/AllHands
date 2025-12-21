using AllHands.AuthService.Application.Features.User.Create;
using AllHands.AuthService.Application.Features.User.ResetPassword;

namespace AllHands.AuthService.Application.Abstractions;

public interface IEmailSender
{
    Task SendResetPasswordEmailAsync(SendResetPasswordEmailCommand command, CancellationToken cancellationToken);

    Task SendCompleteRegistrationEmailAsync(SendCompleteRegistrationEmailCommand command,
        CancellationToken cancellationToken);
}