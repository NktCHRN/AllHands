using AllHands.Shared.Contracts.Rest;
using AllHands.TimeOffService.Application.Features.Employees.GetInTimeOff;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllHands.TimeOffService.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class EmployeesController(IMediator mediator) : ControllerBase
{
    [Authorize]
    [HttpGet("time-off")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<GetEmployeesInTimeOffResultItem>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmployeesInTimeOff([FromQuery] GetEmployeesInTimeOffQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse.FromResult(result.Items));
    }
}
