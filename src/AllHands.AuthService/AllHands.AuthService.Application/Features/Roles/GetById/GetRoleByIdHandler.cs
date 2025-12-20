using AllHands.AuthService.Application.Abstractions;
using AllHands.Shared.Domain.Exceptions;
using MediatR;

namespace AllHands.AuthService.Application.Features.Roles.GetById;

public sealed class GetRoleByIdHandler(IRoleService roleService) : IRequestHandler<GetRoleByIdQuery, GetRoleByIdResult>
{
    public async Task<GetRoleByIdResult> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await roleService.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new EntityNotFoundException("Role was not found");

        return role;
    }
}
