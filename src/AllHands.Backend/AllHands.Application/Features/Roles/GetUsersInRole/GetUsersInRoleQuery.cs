using AllHands.Application.Dto;
using AllHands.Application.Queries;
using MediatR;

namespace AllHands.Application.Features.Roles.GetUsersInRole;

public sealed record GetUsersInRoleQuery(Guid RoleId, int PerPage, int Page) : PagedQuery(PerPage, Page), IRequest<PagedDto<EmployeeDto>>;
