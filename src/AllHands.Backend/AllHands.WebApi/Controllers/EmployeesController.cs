using AllHands.Application.Dto;
using AllHands.Application.Features.Employees.GetById;
using AllHands.Application.Features.Employees.GetInTimeOff;
using AllHands.WebApi.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllHands.WebApi.Controllers;

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

    [Authorize]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDetailsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetEmployeeByIdQuery(id), cancellationToken);
        return Ok(ApiResponse.FromResult(result));
    }
}
