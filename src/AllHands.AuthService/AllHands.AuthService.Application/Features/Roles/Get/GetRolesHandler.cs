using AllHands.AuthService.Application.Abstractions;
using MediatR;

namespace AllHands.AuthService.Application.Features.Roles.Get;

public sealed class GetRolesHandler(IRoleService roleService) : IRequestHandler<GetRolesQuery, GetRolesResult>
{
    public async Task<GetRolesResult> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await roleService.GetAsync(cancellationToken);
        return new GetRolesResult(roles);
    }
}
