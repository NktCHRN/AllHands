using AllHands.EmployeeService.Application.Features.Roles.Save;
using AllHands.Shared.Contracts.Messaging.Events.Roles;
using MediatR;

namespace AllHands.EmployeeService.ConsumersWorker.Handlers;

public sealed class RoleCreatedEventHandler(IMediator mediator)
{
    public async Task Handle(RoleCreatedEvent @event, CancellationToken cancellationToken)
    {
        await mediator.Send(new SaveRoleCommand(@event.Id, @event.Name, @event.IsDefault, @event.CompanyId), cancellationToken);
    }
}
