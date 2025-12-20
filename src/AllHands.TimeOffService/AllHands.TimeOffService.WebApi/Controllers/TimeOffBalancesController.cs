using AllHands.Shared.Application.Auth;
using AllHands.Shared.Contracts.Rest;
using AllHands.Shared.Domain.UserContext;
using AllHands.Shared.WebApi.Rest;
using AllHands.Shared.WebApi.Rest.Auth;
using AllHands.TimeOffService.Application.Features.TimeOffBalances.GetByEmployeeId;
using AllHands.TimeOffService.Application.Features.TimeOffBalances.GetHistoryByEmployeeId;
using AllHands.TimeOffService.Application.Features.TimeOffBalances.Update;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllHands.TimeOffService.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/time-off")]
public sealed class TimeOffBalancesController(IMediator mediator, IUserContext userContext) : ControllerBase
{
    [Authorize]
    [HttpGet("balances/history")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<TimeOffBalancesHistoryItemDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyHistory([FromQuery] PaginationParametersRequest parameters,
        CancellationToken cancellationToken)
    {
        var query = new GetTimeOffBalancesHistoryQuery(userContext.EmployeeId, parameters.PerPage, parameters.Page);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse.FromResult(PagedResponseMapper.FromDto(result)));
    }
    
    [Authorize]
    [HttpGet("balances")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TimeOffBalanceDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMy(CancellationToken cancellationToken)
    {
        var query = new GetTimeOffBalancesQuery(userContext.EmployeeId);
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
        return Ok(ApiResponse.FromResult(PagedResponseMapper.FromDto(result)));
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
    
    [HasPermission(Permissions.TimeOffBalanceEdit)]
    [HttpPatch("employees/{employeeId:guid}/balances/types/{typeId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateBalance([FromRoute] Guid employeeId, [FromRoute] Guid typeId, [FromBody] UpdateBalanceCommand command, CancellationToken cancellationToken)
    {
        command.EmployeeId = employeeId;
        command.TypeId = typeId;
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
