using AllHands.EmployeeService.Application.Features.User.Register;
using AllHands.Shared.Contracts.Messaging.Events.Invitations;
using MediatR;

namespace AllHands.EmployeeService.ConsumersWorker.Handlers;

public sealed class InvitationAcceptedEventHandler(IMediator mediator)
{
    public async Task Handle(InvitationAcceptedEvent @event, CancellationToken cancellationToken)
    {
        await mediator.Send(new RegisterUserCommand(@event.UserId), cancellationToken);
    }
}
