using AllHands.Application;
using AllHands.Application.Features.Permissions.Get;
using AllHands.WebApi.Auth;
using AllHands.WebApi.Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AllHands.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class PermissionsController(IMediator mediator) : ControllerBase
{
    [HasPermission(Permissions.RolesView)]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<string>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPermissions()
    {
        var result = await mediator.Send(new GetPermissionsQuery());
        
        return Ok(ApiResponse.FromResult(result.Permissions));
    }
}
