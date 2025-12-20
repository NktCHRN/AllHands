using AllHands.Auth.Contracts.Messaging;
using AllHands.AuthService.Infrastructure.Auth;

namespace AllHands.AuthService.SessionRecalculator.Handlers;

public sealed class UserSessionsRecalculationRequestedEventHandler(ISessionsUpdater sessionsUpdater)
{
    public async Task Handle(UserSessionsRecalculationRequestedEvent @event, CancellationToken cancellationToken)
    {
        await sessionsUpdater.UpdateUser(@event.UserId, cancellationToken);
    }
}
