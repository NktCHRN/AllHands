using MediatR;

namespace AllHands.NewsService.Application.Features.News.GetById;

public sealed record GetNewsPostByIdQuery(Guid Id) : IRequest<NewsPostDto>;
