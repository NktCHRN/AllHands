using MediatR;

namespace AllHands.Application.Features.TimeOffTypes.Delete;

public sealed record DeleteTimeOffTypeCommand(Guid Id) : IRequest;
