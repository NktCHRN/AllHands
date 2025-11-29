using AllHands.Application.Abstractions;
using MediatR;

namespace AllHands.Application.Features.Roles.Update;

public sealed class UpdateRoleHandler(IRoleService roleService) : IRequestHandler<UpdateRoleCommand>
{
    public async Task Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        await roleService.UpdateAsync(request, cancellationToken);
    }
}
