using AllHands.AuthService.Application.Abstractions;
using AllHands.Shared.Contracts.Messaging.Events.Companies;

namespace AllHands.AuthService.ConsumersWorker.Handlers;

public sealed class CompanyCreatedEventHandler(IRoleService roleService)
{
    public async Task Handle(CompanyCreatedEvent @event, CancellationToken cancellationToken)
    {
        await roleService.CreateDefaultRolesAsync(@event.Id, cancellationToken);
    }
}
