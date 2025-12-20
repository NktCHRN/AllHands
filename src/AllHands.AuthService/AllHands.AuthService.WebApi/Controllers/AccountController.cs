using System.Security.Claims;
using AllHands.Auth.Contracts.Rest;
using AllHands.AuthService.Application.Constants;
using AllHands.AuthService.Application.Features.User.ChangePassword;
using AllHands.AuthService.Application.Features.User.Login;
using AllHands.AuthService.Application.Features.User.RegisterFromInvitation;
using AllHands.AuthService.Application.Features.User.Relogin;
using AllHands.AuthService.Application.Features.User.ResetPassword;
using AllHands.AuthService.Infrastructure.Auth;
using AllHands.Shared.Contracts.Rest;
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
    [ProducesResponseType(typeof(ApiResponse<UserContextResponse>), StatusCodes.Status200OK)]
    public Task<IActionResult> Authenticate(CancellationToken cancellationToken)
    {
        var userContext = new UserContextResponse(
            HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty,
            HttpContext.User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
            HttpContext.User.FindFirst(ClaimTypes.MobilePhone)?.Value,
            HttpContext.User.FindFirst("companyid")?.Value ?? string.Empty,
            User.FindFirst(ClaimTypes.GivenName)?.Value ?? string.Empty,
            User.FindFirst("middlename")?.Value,
            User.FindFirst(ClaimTypes.Surname)?.Value ?? string.Empty,
            User.FindFirst(AllHandsClaimTypes.EmployeeId)?.Value ?? string.Empty,
            User
                .FindAll(ClaimTypes.Role)
                .Where(r => !string.IsNullOrEmpty(r.Value))
                .Select(x => x.Value)
                .ToList(),
            User.FindFirst(AuthConstants.PermissionClaimName)?.Value ?? string.Empty);
        return Task.FromResult((IActionResult) Ok(ApiResponse.FromResult(userContext)));
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
