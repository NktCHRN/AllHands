using AllHands.AuthService.Application.Abstractions;
using MediatR;

namespace AllHands.AuthService.Application.Features.Roles.Delete;

public sealed class DeleteRoleHandler(IRoleService roleService) : IRequestHandler<DeleteRoleCommand>
{
    public async Task Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        await roleService.DeleteAsync(request.Id, cancellationToken);
    }
}
