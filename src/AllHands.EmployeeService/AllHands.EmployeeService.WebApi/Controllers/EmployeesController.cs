using AllHands.EmployeeService.Application.Dto;
using AllHands.EmployeeService.Application.Features.Employees.Create;
using AllHands.EmployeeService.Application.Features.Employees.Delete;
using AllHands.EmployeeService.Application.Features.Employees.DeleteAvatar;
using AllHands.EmployeeService.Application.Features.Employees.Fire;
using AllHands.EmployeeService.Application.Features.Employees.GetAvatarById;
using AllHands.EmployeeService.Application.Features.Employees.GetById;
using AllHands.EmployeeService.Application.Features.Employees.Rehire;
using AllHands.EmployeeService.Application.Features.Employees.Search;
using AllHands.EmployeeService.Application.Features.Employees.Update;
using AllHands.EmployeeService.Application.Features.Employees.UpdateAvatar;
using AllHands.Shared.Application.Auth;
using AllHands.Shared.Contracts.Rest;
using AllHands.Shared.WebApi.Rest;
using AllHands.Shared.WebApi.Rest.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace AllHands.EmployeeService.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class EmployeesController(IMediator mediator) : ControllerBase
{
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
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<EmployeeSearchDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] SearchEmployeesQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(ApiResponse.FromResult(PagedResponseMapper.FromDto(result)));
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
    public async Task<IActionResult> Create(CreateEmployeeCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse.FromResult(result));
    }
    
    [HasPermission(Permissions.EmployeeEdit)]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(Guid id, UpdateEmployeeCommand command, CancellationToken cancellationToken)
    {
        command.EmployeeId = id;
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
    
    [Authorize]
    [HttpPut("{id:guid}/fire")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Fire(Guid id, [FromBody] FireEmployeeCommand command, CancellationToken cancellationToken)
    {
        command.EmployeeId = id;
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
    
    [HasPermission($"{Permissions.EmployeeCreate},{Permissions.EmployeeEdit}")]
    [HttpPut("{id:guid}/rehire")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Rehire(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new RehireEmployeeCommand(id), cancellationToken);
        return NoContent();
    }
    
    [HasPermission(Permissions.EmployeeDelete)]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, [FromBody] DeleteEmployeeCommand command, CancellationToken cancellationToken)
    {
        command.EmployeeId = id;
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
