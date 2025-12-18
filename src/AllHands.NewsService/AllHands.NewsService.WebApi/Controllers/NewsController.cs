using AllHands.NewsService.Application.Features.News;
using AllHands.NewsService.Application.Features.News.Create;
using AllHands.NewsService.Application.Features.News.Delete;
using AllHands.NewsService.Application.Features.News.Get;
using AllHands.NewsService.Application.Features.News.GetById;
using AllHands.NewsService.Application.Features.News.Update;
using AllHands.Shared.Application.Auth;
using AllHands.Shared.Application.Dto;
using AllHands.Shared.Contracts.Rest;
using AllHands.Shared.WebApi.Rest;
using AllHands.Shared.WebApi.Rest.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllHands.NewsService.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class NewsController(IMediator mediator) : ControllerBase
{
    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<NewsPostDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] PaginationParametersRequest parameters, CancellationToken cancellationToken)
    {
        var query = new GetNewsQuery(parameters.PerPage, parameters.Page);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse.FromResult(PagedResponseMapper.FromDto(result)));
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<NewsPostDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetNewsPostByIdQuery(id), cancellationToken);
        return Ok(ApiResponse.FromResult(result));
    }

    [HasPermission(Permissions.NewsCreate)]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CreatedEntityDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateNewsPostCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse.FromResult(result));
    }

    [HasPermission(Permissions.NewsEdit)]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateNewsPostCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        await mediator.Send(command, cancellationToken);
        
        return NoContent();
    }

    [HasPermission(Permissions.NewsDelete)]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteNewsPostCommand(id), cancellationToken);
        return NoContent();
    }
}
