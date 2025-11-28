using AllHands.Application.Dto;
using AllHands.Application.Features.Positions.Search;
using AllHands.WebApi.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllHands.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class PositionsController(IMediator mediator) : ControllerBase
{
    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<PositionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchPositions([FromQuery] SearchPaginationParametersRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new SearchPositionsQuery(request.PerPage, request.Page, request.Search), cancellationToken);
        return Ok(ApiResponse.FromResult(PagedResponse.FromDto(result)));
    }
}
