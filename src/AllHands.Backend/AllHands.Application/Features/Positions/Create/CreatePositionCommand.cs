using AllHands.Application.Dto;
using MediatR;

namespace AllHands.Application.Features.Positions.Create;

public sealed record CreatePositionCommand(string Name) : PositionCommandBase(Name), IRequest<CreatedEntityDto>;
