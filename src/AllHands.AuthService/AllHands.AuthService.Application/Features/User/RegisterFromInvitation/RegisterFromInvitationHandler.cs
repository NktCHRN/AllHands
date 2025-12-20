using AllHands.AuthService.Application.Abstractions;
using AllHands.Shared.Contracts.Messaging.Events.Employees;
using AllHands.Shared.Domain.Exceptions;
using Marten;
using MediatR;

namespace AllHands.AuthService.Application.Features.User.RegisterFromInvitation;

public sealed class RegisterFromInvitationHandler(IAccountService accountService) : IRequestHandler<RegisterFromInvitationCommand>
{
    public async Task Handle(RegisterFromInvitationCommand request, CancellationToken cancellationToken)
    {
        var userId = await accountService.RegisterFromInvitationAsync(request, cancellationToken);
    }
}
