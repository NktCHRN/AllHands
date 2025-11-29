using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AllHands.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class HolidaysController(IMediator mediator) : ControllerBase
{
    
}
