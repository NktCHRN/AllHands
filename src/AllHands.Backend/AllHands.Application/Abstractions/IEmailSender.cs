using AllHands.Application.Features.Employee.Create;
using AllHands.Application.Features.User.ForgotPassword;

namespace AllHands.Application.Abstractions;

public interface IEmailSender
{
    Task SendResetPasswordEmailAsync(SendResetPasswordEmailCommand command, CancellationToken cancellationToken);

    Task SendCompleteRegistrationEmailAsync(SendCompleteRegistrationEmailCommand command,
        CancellationToken cancellationToken);
}