using MediatR;

namespace AllHands.Application.Features.News.GetById;

public sealed record GetNewsPostByIdQuery(Guid Id) : IRequest<NewsPostDto>;
