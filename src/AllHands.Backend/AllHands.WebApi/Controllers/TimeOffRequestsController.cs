using AllHands.Application.Dto;
using AllHands.Application.Features.TimeOffRequests.Create;
using AllHands.WebApi.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllHands.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/time-off/requests")]
public sealed class TimeOffRequestsController(IMediator mediator) : ControllerBase
{
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CreatedEntityDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateTimeOffRequestCommand command, CancellationToken token)
    {
        var result = await mediator.Send(command, token);
        return Ok(result);
    }
}
