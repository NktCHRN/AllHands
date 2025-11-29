using MediatR;

namespace AllHands.Application.Features.Positions.Create;

public sealed record CreatePositionCommand(string Name) : IRequest;
