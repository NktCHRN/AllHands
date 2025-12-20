using AllHands.Shared.Application.Auth;
using AllHands.Shared.Application.Dto;
using AllHands.Shared.Contracts.Rest;
using AllHands.Shared.WebApi.Rest.Auth;
using AllHands.TimeOffService.Application.Features.TimeOffTypes;
using AllHands.TimeOffService.Application.Features.TimeOffTypes.Create;
using AllHands.TimeOffService.Application.Features.TimeOffTypes.Delete;
using AllHands.TimeOffService.Application.Features.TimeOffTypes.Get;
using AllHands.TimeOffService.Application.Features.TimeOffTypes.GetAllowedEmoji;
using AllHands.TimeOffService.Application.Features.TimeOffTypes.GetById;
using AllHands.TimeOffService.Application.Features.TimeOffTypes.Update;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllHands.TimeOffService.WebApi.Controllers;

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

    [HasPermission(Permissions.TimeOffTypeEdit)]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Edit(Guid id, [FromBody] UpdateTimeOffTypeCommand command,
        CancellationToken cancellationToken)
    {
        command.Id = id;
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HasPermission(Permissions.TimeOffTypeDelete)]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteTimeOffTypeCommand(id), cancellationToken);
        return NoContent();
    }
}
