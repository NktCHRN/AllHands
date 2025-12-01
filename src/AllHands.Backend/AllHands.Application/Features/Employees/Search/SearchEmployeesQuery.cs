using AllHands.Application.Dto;
using AllHands.Application.Queries;
using MediatR;

namespace AllHands.Application.Features.Employees.Search;

public sealed record SearchEmployeesQuery(string? Search, Guid? ManagerId = null, int PerPage = 10, int Page = 1) : PagedSearchQuery(PerPage, Page, Search), IRequest<PagedDto<EmployeeDto>>;
