using AllHands.AuthService.Application.Features.Permissions.Get;
using AllHands.Shared.Application.Auth;
using AllHands.Shared.Contracts.Rest;
using AllHands.Shared.WebApi.Rest.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AllHands.AuthService.WebApi.Controllers;

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
