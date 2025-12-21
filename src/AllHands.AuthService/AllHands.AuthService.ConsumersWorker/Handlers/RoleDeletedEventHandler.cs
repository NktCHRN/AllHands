using AllHands.AuthService.Application.Abstractions;
using AllHands.Shared.Contracts.Messaging.Events.Roles;

namespace AllHands.AuthService.ConsumersWorker.Handlers;

public sealed class RoleDeletedEventHandler(IRoleService roleService)
{
    public async Task Handle(RoleDeletedEvent @event, CancellationToken cancellationToken)
    {
        await roleService.ResetUsersRoleAsync(@event.Id, cancellationToken);
    }
}
