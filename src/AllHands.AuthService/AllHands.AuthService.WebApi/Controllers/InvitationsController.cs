using AllHands.AuthService.Application.Features.User.ResendInvitation;
using AllHands.Shared.Application.Auth;
using AllHands.Shared.WebApi.Rest.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AllHands.AuthService.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}")]
public sealed class InvitationsController(IMediator mediator) : ControllerBase
{
    [HasPermission(Permissions.EmployeeCreate)]
    [HttpPost("employees/{id:guid}/invitations")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ResendInvitationAsync(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new ResendInvitationCommand(id), cancellationToken);
        return NoContent();
    }
}
