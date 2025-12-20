using AllHands.AuthService.Application.Dto;
using AllHands.Shared.Application.Dto;
using AllHands.Shared.Application.Queries;
using MediatR;

namespace AllHands.AuthService.Application.Features.Roles.GetUsersInRole;

public sealed record GetUsersInRoleQuery(Guid RoleId, int PerPage, int Page) : PagedQuery(PerPage, Page), IRequest<PagedDto<EmployeeTitleDto>>;
