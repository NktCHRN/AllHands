using AllHands.Application;
using AllHands.Application.Dto;
using AllHands.Application.Features.Holiday.Create;
using AllHands.Application.Features.Holiday.Delete;
using AllHands.Application.Features.Holiday.Get;
using AllHands.Application.Features.Holiday.GetById;
using AllHands.Application.Features.Holiday.Update;
using AllHands.WebApi.Auth;
using AllHands.WebApi.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllHands.WebApi.Controllers;

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
