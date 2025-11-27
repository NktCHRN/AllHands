using AllHands.Application.Features.Employee.Create;
using AllHands.Application.Features.User.ResetPassword;

namespace AllHands.Application.Abstractions;

public interface IEmailSender
{
    Task SendResetPasswordEmailAsync(SendResetPasswordEmailCommand command, CancellationToken cancellationToken);

    Task SendCompleteRegistrationEmailAsync(SendCompleteRegistrationEmailCommand command,
        CancellationToken cancellationToken);
}