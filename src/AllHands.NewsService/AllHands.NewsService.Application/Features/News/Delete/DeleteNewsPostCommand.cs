using MediatR;

namespace AllHands.NewsService.Application.Features.News.Delete;

public sealed record DeleteNewsPostCommand(Guid Id) : IRequest;