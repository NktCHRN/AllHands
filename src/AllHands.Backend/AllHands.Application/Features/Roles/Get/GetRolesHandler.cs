using AllHands.Application.Abstractions;
using MediatR;

namespace AllHands.Application.Features.Roles.Get;

public sealed class GetRolesHandler(IRoleService roleService) : IRequestHandler<GetRolesQuery, GetRolesResult>
{
    public async Task<GetRolesResult> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await roleService.GetRolesAsync(cancellationToken);
        return new GetRolesResult(roles);
    }
}
