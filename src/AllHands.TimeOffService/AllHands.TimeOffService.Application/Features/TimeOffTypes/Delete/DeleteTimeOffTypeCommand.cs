using MediatR;

namespace AllHands.TimeOffService.Application.Features.TimeOffTypes.Delete;

public sealed record DeleteTimeOffTypeCommand(Guid Id) : IRequest;
