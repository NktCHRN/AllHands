using AllHands.AuthService.Application.Abstractions;
using MediatR;

namespace AllHands.AuthService.Application.Features.User.ResetPassword;

public sealed class ResetPasswordCommandHandler(IAccountService accountService, IEmailSender emailSender) : IRequestHandler<ResetPasswordCommand>
{
    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        await accountService.GenerateResetPasswordToken(request.Email, cancellationToken);
    }
}
