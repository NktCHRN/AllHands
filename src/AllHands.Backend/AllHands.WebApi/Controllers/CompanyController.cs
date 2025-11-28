using AllHands.Application;
using AllHands.Application.Features.Company.Get;
using AllHands.Application.Features.Company.GetLogo;
using AllHands.Application.Features.Company.Update;
using AllHands.Application.Features.Company.UpdateLogo;
using AllHands.WebApi.Auth;
using AllHands.WebApi.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllHands.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class CompanyController(IMediator mediator) : ControllerBase
{
    private const int MaxLogoSize = 10 * 1024 * 1024;
    
    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<GetCompanyResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCompany()
    {
        var result = await mediator.Send(new GetCompanyQuery());
        
        return Ok(ApiResponse.FromResult(result));
    }
    
    [Authorize]
    [HttpGet("logo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLogo(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCompanyLogoQuery(), cancellationToken);
        return File(result.File.Stream, result.File.ContentType, result.File.OriginalFileName);
    }
    
    [HasPermission(Permissions.CompanyEdit)]
    [HttpPut("logo")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateLogo(IFormFile? file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse.FromError(new ErrorResponse("No file uploaded.")));
        }

        if (file.Length > MaxLogoSize)
        {
            return BadRequest(ApiResponse.FromError(new ErrorResponse("Logo must be <= 10 MB.")));
        }

        await using var stream = file.OpenReadStream();
        await mediator.Send(new UpdateLogoCommand(stream, file.FileName, file.ContentType), cancellationToken);
        
        return NoContent();
    }

    [HasPermission(Permissions.CompanyEdit)]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update([FromBody] UpdateCompanyCommand command, CancellationToken cancellationToken)
    {
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
