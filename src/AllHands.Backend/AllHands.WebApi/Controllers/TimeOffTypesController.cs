using AllHands.Application;
using AllHands.Application.Dto;
using AllHands.Application.Features.TimeOffTypes;
using AllHands.Application.Features.TimeOffTypes.Create;
using AllHands.Application.Features.TimeOffTypes.Delete;
using AllHands.Application.Features.TimeOffTypes.Get;
using AllHands.Application.Features.TimeOffTypes.GetAllowedEmoji;
using AllHands.Application.Features.TimeOffTypes.GetById;
using AllHands.WebApi.Auth;
using AllHands.WebApi.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllHands.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/time-off/types")]
public sealed class TimeOffTypesController(IMediator mediator) : ControllerBase
{
    [Authorize]
    [HttpGet("allowed-emojis")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<string>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllowedEmoji(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAllowedEmojiQuery(), cancellationToken);
        return Ok(ApiResponse.FromResult(result));
    }

    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TimeOffTypeDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetTimeOffTypesQuery(), cancellationToken);
        return Ok(ApiResponse.FromResult(result.TimeOffTypes));
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<TimeOffTypeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetTimeOffTypeByIdQuery(id), cancellationToken);
        return Ok(ApiResponse.FromResult(result));
    }

    [HasPermission(Permissions.TimeOffTypeCreate)]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CreatedEntityDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateTimeOffTypeCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse.FromResult(result));
    }

    [HasPermission(Permissions.TimeOffTypeDelete)]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteTimeOffTypeCommand(id), cancellationToken);
        return NoContent();
    }
    
    // TODO: implement update.
}
