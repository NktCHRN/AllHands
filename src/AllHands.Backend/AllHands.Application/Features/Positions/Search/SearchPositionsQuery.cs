using AllHands.Application.Dto;
using AllHands.Application.Queries;
using MediatR;

namespace AllHands.Application.Features.Positions.Search;

public sealed record SearchPositionsQuery(int PerPage, int Page, string? Search) : PagedSearchQuery(PerPage, Page, Search), IRequest<PagedDto<PositionDto>>;
