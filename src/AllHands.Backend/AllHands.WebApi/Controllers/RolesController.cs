using AllHands.Application;
using AllHands.Application.Dto;
using AllHands.Application.Features.Roles.Get;
using AllHands.Application.Features.Roles.GetUsersInRole;
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

    [HasPermission(Permissions.RolesView)]
    [HttpGet("{id:guid}/users")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<EmployeeDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsersInRole(Guid id, [FromQuery] PaginationParametersRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetUsersInRoleQuery(id, request.PerPage, request.Page);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse.FromResult(PagedResponse.FromDto(result)));
    }
}
