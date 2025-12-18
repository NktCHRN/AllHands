using AllHands.Shared.Application.Dto;
using AllHands.Shared.Application.Queries;
using MediatR;

namespace AllHands.NewsService.Application.Features.News.Get;

public sealed record GetNewsQuery(int PerPage, int Page) : PagedQuery(PerPage, Page), IRequest<PagedDto<NewsPostDto>>;
