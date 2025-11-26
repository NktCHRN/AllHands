using AllHands.Application.Abstractions;
using MediatR;

namespace AllHands.Application.Features.User.Login;

public sealed class LoginCommandHandler(IAccountService accountService) : IRequestHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await accountService.LoginAsync(request.Email, request.Password, cancellationToken);
    }
}
