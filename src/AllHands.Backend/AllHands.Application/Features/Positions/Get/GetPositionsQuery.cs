using AllHands.Application.Dto;
using AllHands.Application.Queries;
using MediatR;

namespace AllHands.Application.Features.Positions.Get;

public sealed record GetPositionsQuery(int PerPage, int Page, string? Search) : PagedSearchQuery(PerPage, Page, Search), IRequest<PagedDto<PositionDto>>;
