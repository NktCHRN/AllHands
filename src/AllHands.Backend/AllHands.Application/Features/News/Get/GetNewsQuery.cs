using AllHands.Application.Dto;
using AllHands.Application.Queries;
using MediatR;

namespace AllHands.Application.Features.News.Get;

public sealed record GetNewsQuery(int PerPage, int Page) : PagedQuery(PerPage, Page), IRequest<PagedDto<NewsPostDto>>;
