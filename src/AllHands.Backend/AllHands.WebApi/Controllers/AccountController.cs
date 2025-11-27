using AllHands.Application.Features.User.Login;
using AllHands.Application.Features.User.RegisterFromInvitation;
using AllHands.WebApi.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllHands.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class AccountController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await mediator.Send(command);

        await HttpContext.SignInAsync(result.ClaimsPrincipal);
        
        return NoContent();
    }

    [HttpPost("register/invitation")]
    public async Task<IActionResult> RegisterFromInvitation(
        [FromQuery] Guid invitationId,
        [FromQuery] string invitationToken,
        [FromBody] RegisterFromInvitationRequest request)
    {
        var command = new RegisterFromInvitationCommand(invitationId, invitationToken, request.Password);
        
        await  mediator.Send(command);
        return NoContent();
    }
    
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        
        return NoContent();
    }
}
