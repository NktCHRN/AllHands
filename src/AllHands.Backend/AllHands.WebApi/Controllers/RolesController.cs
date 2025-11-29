using AllHands.Application;
using AllHands.Application.Features.Roles.Get;
using AllHands.WebApi.Auth;
using AllHands.WebApi.Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AllHands.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class RolesController(IMediator mediator) : ControllerBase
{
    [HasPermission(Permissions.RolesView)]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RoleWithUsersCountDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoles()
    {
        var result = await mediator.Send(new GetRolesQuery());
        return Ok(ApiResponse.FromResult(result.Roles));
    }
}
