using MediatR;

namespace AllHands.Application.Features.Positions.Delete;

public sealed record DeletePositionCommand(Guid Id) : IRequest;
