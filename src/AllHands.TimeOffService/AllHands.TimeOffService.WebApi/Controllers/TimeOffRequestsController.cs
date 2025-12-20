using AllHands.Shared.Application.Dto;
using AllHands.Shared.Contracts.Rest;
using AllHands.Shared.Domain.UserContext;
using AllHands.Shared.WebApi.Rest;
using AllHands.TimeOffService.Application.Features.TimeOffRequests;
using AllHands.TimeOffService.Application.Features.TimeOffRequests.Approve;
using AllHands.TimeOffService.Application.Features.TimeOffRequests.Cancel;
using AllHands.TimeOffService.Application.Features.TimeOffRequests.Create;
using AllHands.TimeOffService.Application.Features.TimeOffRequests.Get;
using AllHands.TimeOffService.Application.Features.TimeOffRequests.GetById;
using AllHands.TimeOffService.Application.Features.TimeOffRequests.Reject;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllHands.TimeOffService.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/time-off/requests")]
public sealed class TimeOffRequestsController(IMediator mediator, IUserContext userContext) : ControllerBase
{
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CreatedEntityDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateTimeOffRequestCommand command, CancellationToken token)
    {
        var result = await mediator.Send(command, token);
        return StatusCode(StatusCodes.Status201Created, ApiResponse.FromResult(result));
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<TimeOffRequestDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken token)
    {
        var result = await mediator.Send(new GetTimeOffRequestByIdQuery(id), token);
        return Ok(ApiResponse.FromResult(result));
    }
    
    [Authorize]
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CancelAsync([FromRoute] Guid id, CancellationToken token)
    {
        await mediator.Send(new CancelTimeOffRequestCommand(id), token);
        return NoContent();
    }
    
    [Authorize]
    [HttpPost("{id:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ApproveAsync([FromRoute] Guid id, CancellationToken token)
    {
        await mediator.Send(new ApproveTimeOffRequestCommand(id), token);
        return NoContent();
    }
    
    [Authorize]
    [HttpPost("{id:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RejectAsync([FromRoute] Guid id, [FromBody] RejectTimeOffRequestCommand command, CancellationToken token)
    {
        command.Id = id;
        await mediator.Send(command, token);
        return NoContent();
    }
    
    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<TimeOffRequestDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyAsync([FromQuery] PaginationParametersRequest parameters, CancellationToken token)
    {
        var result = await mediator.Send(new GetTimeOffRequestsQuery(parameters.PerPage, parameters.Page, EmployeeId: userContext.EmployeeId), token);
        return Ok(ApiResponse.FromResult(PagedResponseMapper.FromDto(result)));
    }
    
    [Authorize]
    [HttpGet("~/api/v{version:apiVersion}/time-off/employees/requests")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<TimeOffRequestDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync([FromQuery] GetTimeOffRequestsQuery query, CancellationToken token)
    {
        var result = await mediator.Send(query, token);
        return Ok(ApiResponse.FromResult(PagedResponseMapper.FromDto(result)));
    }
}
