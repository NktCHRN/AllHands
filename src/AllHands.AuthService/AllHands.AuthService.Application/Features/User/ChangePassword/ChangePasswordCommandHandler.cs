using AllHands.AuthService.Application.Abstractions;
using MediatR;

namespace AllHands.AuthService.Application.Features.User.ChangePassword;

public sealed class ChangePasswordCommandHandler(IAccountService accountService) : IRequestHandler<ChangePasswordCommand>
{
    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        await accountService.ChangePassword(request, cancellationToken);
    }
}
