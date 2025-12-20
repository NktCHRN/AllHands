using AllHands.Shared.Application.Dto;
using MediatR;

namespace AllHands.TimeOffService.Application.Features.TimeOffTypes.Create;

public sealed record CreateTimeOffTypeCommand(
    string Name,
    string Emoji,
    decimal DaysPerYear) : TimeOffTypeBaseCommand(Name, Emoji, DaysPerYear), IRequest<CreatedEntityDto>;
    