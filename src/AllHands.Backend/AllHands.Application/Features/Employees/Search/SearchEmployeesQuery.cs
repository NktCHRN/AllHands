using AllHands.Application.Dto;
using AllHands.Application.Queries;
using AllHands.Domain.Models;
using MediatR;

namespace AllHands.Application.Features.Employees.Search;

public sealed record SearchEmployeesQuery(string? Search, Guid? ManagerId = null, EmployeeStatus Status = EmployeeStatus.Undefined, int PerPage = 10, int Page = 1) : PagedSearchQuery(PerPage, Page, Search), IRequest<PagedDto<EmployeeSearchDto>>;
