using AllHands.AuthService.Infrastructure.Auth;
using AllHands.Shared.Contracts.Messaging.Events.Users;

namespace AllHands.AuthService.ConsumersWorker.Handlers;

public sealed class UserDeletedEventHandler(ISessionsUpdater sessionsUpdater)
{
    public async Task Handle(UserDeletedEvent @event, CancellationToken cancellationToken)
    {
        await sessionsUpdater.ExpireUser(@event.Id, cancellationToken);
    }
}
