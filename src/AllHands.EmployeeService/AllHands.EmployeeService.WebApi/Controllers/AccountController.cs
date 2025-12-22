using AllHands.EmployeeService.Application.Features.Accounts.Get;
using AllHands.EmployeeService.Application.Features.User.DeleteAvatar;
using AllHands.EmployeeService.Application.Features.User.Get;
using AllHands.EmployeeService.Application.Features.User.GetAvatar;
using AllHands.EmployeeService.Application.Features.User.GetDetails;
using AllHands.EmployeeService.Application.Features.User.Update;
using AllHands.EmployeeService.Application.Features.User.UpdateAvatar;
using AllHands.Shared.Contracts.Rest;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllHands.EmployeeService.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class AccountController(IMediator mediator) : ControllerBase
{
    [Authorize]
    [HttpGet("~/api/v{version:apiVersion}/accounts")]
    [ProducesResponseType(typeof(ApiResponse<GetAccountsResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAccounts(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAccountsQuery(), cancellationToken);
        
        return Ok(ApiResponse.FromResult(result));
    }
    

    [Authorize]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update([FromBody] UpdateUserCommand command, CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        
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

        if (file.Length > Constants.MaxAvatarSize)
        {
            return BadRequest(ApiResponse.FromError(new ErrorResponse("Avatar must be <= 5 MB.")));
        }

        await using var stream = file.OpenReadStream();
        await mediator.Send(new UpdateUserAvatarCommand(stream, file.FileName, file.ContentType), cancellationToken);
        
        return NoContent();
    }
    
    [Authorize]
    [HttpDelete("avatar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAvatar(CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteUserAvatarCommand(), cancellationToken);
        
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
