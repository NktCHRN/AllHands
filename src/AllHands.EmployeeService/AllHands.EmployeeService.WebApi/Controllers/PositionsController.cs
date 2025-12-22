using AllHands.EmployeeService.Application.Dto;
using AllHands.EmployeeService.Application.Features.Positions.Create;
using AllHands.EmployeeService.Application.Features.Positions.Delete;
using AllHands.EmployeeService.Application.Features.Positions.GetById;
using AllHands.EmployeeService.Application.Features.Positions.Search;
using AllHands.EmployeeService.Application.Features.Positions.Update;
using AllHands.Shared.Application.Auth;
using AllHands.Shared.Contracts.Rest;
using AllHands.Shared.WebApi.Rest;
using AllHands.Shared.WebApi.Rest.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllHands.EmployeeService.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class PositionsController(IMediator mediator) : ControllerBase
{
    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<PositionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchPositions([FromQuery] SearchPaginationParametersRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new SearchPositionsQuery(request.PerPage, request.Page, request.Search), cancellationToken);
        return Ok(ApiResponse.FromResult(PagedResponseMapper.FromDto(result)));
    }
    
    [Authorize]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PositionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetPositionByIdQuery(id), cancellationToken);
        return Ok(ApiResponse.FromResult(result));
    }

    [HasPermission(Permissions.PositionCreate)]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CreatedEntityDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreatePositionCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse.FromResult(result));
    }

    [HasPermission(Permissions.PositionEdit)]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePositionCommand command,
        CancellationToken cancellationToken)
    {
        command.Id = id;
        
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HasPermission(Permissions.PositionDelete)]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeletePositionCommand(id);
        
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
