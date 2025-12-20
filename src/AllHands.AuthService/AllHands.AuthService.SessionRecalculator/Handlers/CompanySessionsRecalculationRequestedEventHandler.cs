using AllHands.Auth.Contracts.Messaging;
using AllHands.AuthService.Infrastructure.Auth;
using Microsoft.Extensions.Options;

namespace AllHands.AuthService.SessionRecalculator.Handlers;

public sealed class CompanySessionsRecalculationRequestedEventHandler(ISessionsUpdater sessionsUpdater, IOptions<SessionRecalculatorOptions> options)
{
    public async Task Handle(CompanySessionsRecalculationRequestedEvent @event, CancellationToken cancellationToken)
    {
        await sessionsUpdater.UpdateAll(@event.CompanyId, options.Value.BatchSize, cancellationToken);
    }
}
