using AllHands.Application.Abstractions;
using AllHands.Application.Features.TimeOffBalances.GetByEmployeeId;
using AllHands.Application.Features.TimeOffBalances.GetHistoryByEmployeeId;
using AllHands.WebApi.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllHands.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/time-off")]
public sealed class TimeOffBalancesController(IMediator mediator, ICurrentUserService currentUserService) : ControllerBase
{
    [Authorize]
    [HttpGet("balances/history")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<TimeOffBalancesHistoryItemDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyHistory([FromQuery] PaginationParametersRequest parameters,
        CancellationToken cancellationToken)
    {
        var query = new GetTimeOffBalancesHistoryQuery(currentUserService.GetEmployeeId(), parameters.PerPage, parameters.Page);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse.FromResult(PagedResponse.FromDto(result)));
    }
    
    [Authorize]
    [HttpGet("balances")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TimeOffBalanceDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMy(CancellationToken cancellationToken)
    {
        var query = new GetTimeOffBalancesQuery(currentUserService.GetEmployeeId());
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse.FromResult(result.Balances));
    }
    
    [Authorize]
    [HttpGet("employees/{employeeId:guid}/balances/history")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<TimeOffBalancesHistoryItemDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistory([FromRoute] Guid employeeId, [FromQuery] PaginationParametersRequest parameters,
        CancellationToken cancellationToken)
    {
        var query = new GetTimeOffBalancesHistoryQuery(employeeId, parameters.PerPage, parameters.Page);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse.FromResult(PagedResponse.FromDto(result)));
    }
    
    [Authorize]
    [HttpGet("employees/{employeeId:guid}/balances")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TimeOffBalanceDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBalance([FromRoute] Guid employeeId, CancellationToken cancellationToken)
    {
        var query = new GetTimeOffBalancesQuery(employeeId);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse.FromResult(result.Balances));
    }
}
