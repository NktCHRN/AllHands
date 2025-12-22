using AllHands.EmployeeService.Application.Dto;
using AllHands.EmployeeService.Domain.Models;
using AllHands.Shared.Application.Dto;
using AllHands.Shared.Application.Queries;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.Employees.Search;

public sealed record SearchEmployeesQuery(string? Search, Guid? ManagerId = null, EmployeeStatus Status = EmployeeStatus.Undefined, int PerPage = 10, int Page = 1) : PagedSearchQuery(PerPage, Page, Search), IRequest<PagedDto<EmployeeSearchDto>>;
