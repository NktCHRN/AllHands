using AllHands.AuthService.Application.Abstractions;
using AllHands.AuthService.Application.Dto;
using AllHands.Shared.Application.Dto;
using MediatR;

namespace AllHands.AuthService.Application.Features.Roles.GetUsersInRole;

public sealed class GetUsersInRoleHandler(IRoleService roleService) : IRequestHandler<GetUsersInRoleQuery, PagedDto<EmployeeTitleDto>>
{
    public async Task<PagedDto<EmployeeTitleDto>> Handle(GetUsersInRoleQuery request, CancellationToken cancellationToken)
    {
        var users = await roleService.GetUsersAsync(request, cancellationToken);

        return users;
    }
}
