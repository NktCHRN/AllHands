using AllHands.Application.Features.Company.Get;
using AllHands.WebApi.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllHands.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class CompanyController(IMediator mediator) : ControllerBase
{
    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<GetCompanyResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCompany()
    {
        var result = await mediator.Send(new GetCompanyQuery());
        
        return Ok(ApiResponse.FromResult(result));
    }
}
