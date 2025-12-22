using AllHands.EmployeeService.Application.Dto;
using AllHands.Shared.Application.Dto;
using AllHands.Shared.Application.Queries;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.Positions.Search;

public sealed record SearchPositionsQuery(int PerPage, int Page, string? Search) : PagedSearchQuery(PerPage, Page, Search), IRequest<PagedDto<PositionDto>>;
