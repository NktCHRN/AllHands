using MediatR;

namespace AllHands.EmployeeService.Application.Features.Positions.Update;

public sealed record UpdatePositionCommand(string Name) : PositionCommandBase(Name), IRequest
{
    public Guid Id { get; set; }
}
