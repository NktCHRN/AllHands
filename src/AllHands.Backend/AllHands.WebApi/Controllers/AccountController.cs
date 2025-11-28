using AllHands.Application.Features.Accounts.Get;
using AllHands.Application.Features.User.ChangePassword;
using AllHands.Application.Features.User.Get;
using AllHands.Application.Features.User.GetAvatar;
using AllHands.Application.Features.User.GetDetails;
using AllHands.Application.Features.User.Login;
using AllHands.Application.Features.User.RegisterFromInvitation;
using AllHands.Application.Features.User.Relogin;
using AllHands.Application.Features.User.ResetPassword;
using AllHands.Application.Features.User.Update;
using AllHands.Application.Features.User.UpdateAvatar;
using AllHands.WebApi.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AllHands.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class AccountController(IMediator mediator) : ControllerBase
{
    private const int MaxAvatarSize = 5 * 1024 * 1024;
    
    [Authorize]
    [HttpGet("~/api/v{version:apiVersion}/accounts")]
    [ProducesResponseType(typeof(ApiResponse<GetAccountsResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAccounts(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAccountsQuery(), cancellationToken);
        
        return Ok(ApiResponse.FromResult(result));
    }
    
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        await HttpContext.SignInAsync(result.ClaimsPrincipal);
        
        return NoContent();
    }
    
    [Authorize]
    [HttpPost("relogin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Relogin([FromBody] ReloginCommand command, CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);

        await HttpContext.SignOutAsync();
        
        return NoContent();
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

    [Authorize]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update([FromBody] UpdateUserCommand command, CancellationToken cancellationToken)
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
    
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        
        return NoContent();
    }
    
    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<GetUserResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUserQuery(), cancellationToken);
        return Ok(ApiResponse.FromResult(result));
    }

    [Authorize]
    [HttpGet("avatar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvatar(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAvatarQuery(), cancellationToken);
        return File(result.File.Stream, result.File.ContentType, result.File.OriginalFileName);
    }

    [Authorize]
    [HttpPut("avatar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateAvatar(IFormFile? file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse.FromError(new ErrorResponse("No file uploaded.")));
        }

        if (file.Length > MaxAvatarSize)
        {
            return BadRequest(ApiResponse.FromError(new ErrorResponse("Avatar must be <= 5 MB.")));
        }

        await using var stream = file.OpenReadStream();
        await mediator.Send(new UpdateUserAvatarCommand(stream, file.FileName, file.ContentType), cancellationToken);
        
        return NoContent();
    }
    
    [Authorize]
    [HttpGet("details")]
    [ProducesResponseType(typeof(ApiResponse<GetUserResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDetails(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUserDetailsQuery(), cancellationToken);
        return Ok(ApiResponse.FromResult(result));
    }
}
