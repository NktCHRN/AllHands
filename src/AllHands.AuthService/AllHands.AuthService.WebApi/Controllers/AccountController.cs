using System.Security.Claims;
using System.Text.Json;
using AllHands.AuthService.Application.Constants;
using AllHands.AuthService.Application.Features.User.ChangePassword;
using AllHands.AuthService.Application.Features.User.Login;
using AllHands.AuthService.Application.Features.User.RegisterFromInvitation;
using AllHands.AuthService.Application.Features.User.Relogin;
using AllHands.AuthService.Application.Features.User.ResetPassword;
using AllHands.AuthService.Infrastructure.Auth;
using AllHands.Shared.Infrastructure.UserContext;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AllHands.AuthService.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class AccountController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        await HttpContext.SignInAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme, principal: result.ClaimsPrincipal);
        
        return NoContent();
    }
    
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [HttpPost("relogin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Relogin([FromBody] ReloginCommand command, CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);

        await HttpContext.SignOutAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme);
        
        return NoContent();
    }
    
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [HttpPost("authenticate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public Task<IActionResult> Authenticate(CancellationToken cancellationToken)
    {
        Response.Headers[UserContextHeaders.Id] = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        Response.Headers[UserContextHeaders.Email] = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        Response.Headers[UserContextHeaders.CompanyId] = HttpContext.User.FindFirst(AllHandsClaimTypes.CompanyId)?.Value ?? string.Empty;
        Response.Headers[UserContextHeaders.FirstName] = User.FindFirst(ClaimTypes.GivenName)?.Value ?? string.Empty;
        Response.Headers[UserContextHeaders.LastName] = User.FindFirst(ClaimTypes.Surname)?.Value ?? string.Empty;
        Response.Headers[UserContextHeaders.Roles] = JsonSerializer.Serialize(User
            .FindAll(ClaimTypes.Role)
            .Where(r => !string.IsNullOrEmpty(r.Value))
            .Select(x => x.Value)
            .ToList());
        Response.Headers[UserContextHeaders.EmployeeId] = User.FindFirst(AllHandsClaimTypes.EmployeeId)?.Value ?? string.Empty;
        Response.Headers[UserContextHeaders.Permissions] = User.FindFirst(AuthConstants.PermissionClaimName)?.Value ?? string.Empty;

        
        if (!string.IsNullOrEmpty(HttpContext.User.FindFirst(ClaimTypes.MobilePhone)?.Value))
        {
            Response.Headers[UserContextHeaders.PhoneNumber] = HttpContext.User.FindFirst(ClaimTypes.MobilePhone)?.Value;
        }
        if (!string.IsNullOrEmpty(User.FindFirst(AllHandsClaimTypes.MiddleName)?.Value))
        {
            Response.Headers[UserContextHeaders.MiddleName] = User.FindFirst(AllHandsClaimTypes.MiddleName)?.Value;
        }
        
        return Task.FromResult<IActionResult>(NoContent());
    }

    [HttpPost("register/invitation")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RegisterFromInvitation(
        [FromBody] RegisterFromInvitationCommand command, CancellationToken cancellationToken)
    {
        await  mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("reset-password")]
    [EnableRateLimiting("ResetPasswordLimiter")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        
        return NoContent();
    }
    
    [HttpPut("password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        
        return NoContent();
    }
    
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme);
        
        return NoContent();
    }
}
