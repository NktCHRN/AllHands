using AllHands.AuthService.Application.Abstractions;
using AllHands.AuthService.Application.Features.Employees.Create;
using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.Utilities;
using Marten;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AllHands.AuthService.Application.Features.Employees.ResendInvitation;

public sealed class ResendInvitationHandler(IAccountService accountService) : IRequestHandler<ResendInvitationCommand>
{
    public async Task Handle(ResendInvitationCommand request, CancellationToken cancellationToken)
    {
        await accountService.RegenerateInvitationAsync(request.EmployeeId, cancellationToken);
    }
}
