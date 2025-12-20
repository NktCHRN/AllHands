using AllHands.AuthService.Application.Abstractions;
using MediatR;

namespace AllHands.AuthService.Application.Features.User.Relogin;

public sealed class ReloginCommandHandler(IAccountService accountService) : IRequestHandler<ReloginCommand>
{
    public async Task Handle(ReloginCommand request, CancellationToken cancellationToken)
    {
        await accountService.ReloginAsync(request.CompanyId, cancellationToken);
    }
}
