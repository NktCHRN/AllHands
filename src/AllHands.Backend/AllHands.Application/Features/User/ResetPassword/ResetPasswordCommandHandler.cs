using AllHands.Application.Abstractions;
using MediatR;

namespace AllHands.Application.Features.User.ResetPassword;

public sealed class ResetPasswordCommandHandler(IAccountService accountService, IEmailSender emailSender) : IRequestHandler<ResetPasswordCommand>
{
    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var result = await accountService.GenerateResetPasswordToken(request.Email, cancellationToken);
        if (!result.IsSuccess)
        {
            return;
        }

        await emailSender.SendResetPasswordEmailAsync(new SendResetPasswordEmailCommand(request.Email, result.FirstName!, result.Token!), cancellationToken);
    }
}
