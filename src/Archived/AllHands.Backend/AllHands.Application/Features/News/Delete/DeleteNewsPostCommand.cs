using MediatR;

namespace AllHands.Application.Features.News.Delete;

public sealed record DeleteNewsPostCommand(Guid Id) : IRequest;