using AllHands.Application;
using AllHands.Application.Dto;
using AllHands.Application.Features.Employees.Create;
using AllHands.Application.Features.Employees.DeleteAvatar;
using AllHands.Application.Features.Employees.GetAvatarById;
using AllHands.Application.Features.Employees.GetById;
using AllHands.Application.Features.Employees.GetInTimeOff;
using AllHands.Application.Features.Employees.Search;
using AllHands.Application.Features.Employees.UpdateAvatar;
using AllHands.WebApi.Auth;
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
    
    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<EmployeeDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] SearchEmployeesQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse.FromResult(PagedResponse.FromDto(result)));
    }
    
    [Authorize]
    [HttpGet("{id:guid}/avatar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvatar(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAvatarByIdQuery(id), cancellationToken);
        return File(result.File.Stream, result.File.ContentType, result.File.OriginalFileName);
    }
    
    [HasPermission(Permissions.EmployeeEdit)]
    [HttpPut("{id:guid}/avatar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateAvatar(Guid id, IFormFile? file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse.FromError(new ErrorResponse("No file uploaded.")));
        }

        if (file.Length > Constants.MaxAvatarSize)
        {
            return BadRequest(ApiResponse.FromError(new ErrorResponse("Avatar must be <= 5 MB.")));
        }

        await using var stream = file.OpenReadStream();
        await mediator.Send(new UpdateEmployeeAvatarCommand(id, stream, file.FileName, file.ContentType), cancellationToken);
        
        return NoContent();
    }
    
    [HasPermission(Permissions.EmployeeEdit)]
    [HttpDelete("{id:guid}/avatar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAvatar(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteAvatarByIdCommand(id), cancellationToken);
        
        return NoContent();
    }

    [HasPermission(Permissions.EmployeeCreate)]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CreateEmployeeResult>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateEmployee(CreateEmployeeCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse.FromResult(result));
    }
}
