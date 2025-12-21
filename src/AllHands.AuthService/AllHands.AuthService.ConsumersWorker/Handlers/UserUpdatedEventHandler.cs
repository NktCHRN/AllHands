using AllHands.AuthService.Infrastructure.Auth;
using AllHands.Shared.Contracts.Messaging.Events.Users;

namespace AllHands.AuthService.ConsumersWorker.Handlers;

public sealed class UserUpdatedEventHandler(ISessionsUpdater sessionsUpdater)
{
    public async Task Handle(UserUpdatedEvent @event, CancellationToken cancellationToken)
    {
        await sessionsUpdater.UpdateUser(@event.Id, cancellationToken);
    }
}
