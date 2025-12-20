using AllHands.Shared.Application.Auth;
using AllHands.Shared.Application.Dto;
using AllHands.Shared.Contracts.Rest;
using AllHands.Shared.WebApi.Rest.Auth;
using AllHands.TimeOffService.Application.Dto;
using AllHands.TimeOffService.Application.Features.Holiday.Create;
using AllHands.TimeOffService.Application.Features.Holiday.Delete;
using AllHands.TimeOffService.Application.Features.Holiday.Get;
using AllHands.TimeOffService.Application.Features.Holiday.GetById;
using AllHands.TimeOffService.Application.Features.Holiday.Update;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllHands.TimeOffService.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class HolidaysController(IMediator mediator) : ControllerBase
{
    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<HolidayDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] DateOnly start, [FromQuery] DateOnly end,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetHolidaysQuery(start, end), cancellationToken);
        return Ok(ApiResponse.FromResult(result.Holidays));
    }
    
    [Authorize]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<HolidayDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetHolidayByIdQuery(id), cancellationToken);
        return Ok(ApiResponse.FromResult(result));
    }

    [HasPermission(Permissions.HolidayCreate)]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CreatedEntityDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateHolidayCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse.FromResult(result));
    }

    [HasPermission(Permissions.HolidayEdit)]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateHolidayCommand command,
        CancellationToken cancellationToken)
    {
        command.Id = id;
        
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HasPermission(Permissions.HolidayDelete)]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteHolidayCommand(id);
        
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
