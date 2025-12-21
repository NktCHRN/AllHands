using AllHands.AuthService.Infrastructure.Auth;
using AllHands.AuthService.SessionRecalculator;
using AllHands.Shared.Contracts.Messaging.Events.Roles;
using Microsoft.Extensions.Options;

namespace AllHands.AuthService.ConsumersWorker.Handlers;

public sealed class RoleUpdatedEventHandler(ISessionsUpdater sessionsUpdater, IOptions<SessionRecalculatorOptions> options)
{
    public async Task Handle(RoleUpdatedEvent @event, CancellationToken cancellationToken)
    {
        await sessionsUpdater.UpdateInRole(@event.Id, options.Value.BatchSize, cancellationToken);
    }
}
