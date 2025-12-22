using AllHands.EmployeeService.Application.Features.Roles.Delete;
using AllHands.Shared.Contracts.Messaging.Events.Roles;
using MediatR;

namespace AllHands.EmployeeService.ConsumersWorker.Handlers;

public sealed class RoleDeletedEventHandler(IMediator mediator)
{
    public async Task Handle(RoleDeletedEvent @event, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteRoleCommand(@event.Id), cancellationToken);
    }
}
