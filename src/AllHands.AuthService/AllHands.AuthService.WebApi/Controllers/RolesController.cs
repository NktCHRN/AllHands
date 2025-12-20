using AllHands.AuthService.Application.Dto;
using AllHands.AuthService.Application.Features.Roles.Create;
using AllHands.AuthService.Application.Features.Roles.Delete;
using AllHands.AuthService.Application.Features.Roles.Get;
using AllHands.AuthService.Application.Features.Roles.GetById;
using AllHands.AuthService.Application.Features.Roles.GetUsersInRole;
using AllHands.AuthService.Application.Features.Roles.Update;
using AllHands.Shared.Application.Auth;
using AllHands.Shared.Application.Dto;
using AllHands.Shared.Contracts.Rest;
using AllHands.Shared.WebApi.Rest;
using AllHands.Shared.WebApi.Rest.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AllHands.AuthService.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class RolesController(IMediator mediator) : ControllerBase
{
    [HasPermission(Permissions.RolesView)]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RoleWithUsersCountDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get()
    {
        var result = await mediator.Send(new GetRolesQuery());
        return Ok(ApiResponse.FromResult(result.Roles));
    }

    [HasPermission(Permissions.RolesView)]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<GetRoleByIdResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetRoleByIdQuery(id), cancellationToken);
        return Ok(ApiResponse.FromResult(result));
    }
    
    [HasPermission(Permissions.RolesView)]
    [HttpGet("{id:guid}/users")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<EmployeeTitleDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsersInRole(Guid id, [FromQuery] PaginationParametersRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetUsersInRoleQuery(id, request.PerPage, request.Page);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse.FromResult(PagedResponseMapper.FromDto(result)));
    }

    [HasPermission(Permissions.RolesCreate)]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CreatedEntityDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateRoleCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse.FromResult(result));
    }

    [HasPermission(Permissions.RolesEdit)]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRoleCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        await mediator.Send(command, cancellationToken);
        
        return NoContent();
    }
    
    [HasPermission(Permissions.RolesDelete)]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteRoleCommand(id);
        await mediator.Send(command, cancellationToken);
        
        return NoContent();
    }
}
