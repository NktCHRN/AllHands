using MediatR;

namespace AllHands.Application.Features.Positions.Update;

public sealed record UpdatePositionCommand(string Name) : PositionCommandBase(Name), IRequest
{
    public Guid Id { get; set; }
}
