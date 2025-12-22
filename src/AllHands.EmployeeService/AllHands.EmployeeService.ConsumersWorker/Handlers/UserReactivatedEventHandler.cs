using AllHands.EmployeeService.Application.Features.User.Reactivate;
using AllHands.Shared.Contracts.Messaging.Events.Users;
using MediatR;

namespace AllHands.EmployeeService.ConsumersWorker.Handlers;

public sealed class UserReactivatedEventHandler(IMediator mediator)
{
    public async Task Handle(UserReactivatedEvent @event, CancellationToken cancellationToken)
    {
        await mediator.Send(new ReactivateUserCommand(@event.Id, @event.GlobalUserId, @event.RoleIds), cancellationToken);
    }
}
