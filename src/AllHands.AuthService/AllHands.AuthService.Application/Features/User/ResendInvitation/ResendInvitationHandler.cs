using AllHands.AuthService.Application.Abstractions;
using MediatR;

namespace AllHands.AuthService.Application.Features.User.ResendInvitation;

public sealed class ResendInvitationHandler(IAccountService accountService) : IRequestHandler<ResendInvitationCommand>
{
    public async Task Handle(ResendInvitationCommand request, CancellationToken cancellationToken)
    {
        await accountService.RegenerateInvitationAsync(request.EmployeeId, cancellationToken);
    }
}
