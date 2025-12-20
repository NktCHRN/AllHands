using AllHands.AuthService.Application.Abstractions;
using AllHands.Shared.Contracts.Messaging.Events.Employees;

namespace AllHands.AuthService.ConsumersWorker.Handlers;

public sealed class EmployeeFiredEventHandler(IAccountService accountService)
{
    public async Task Handle(EmployeeFiredEvent @event, CancellationToken cancellationToken)
    {
        await accountService.DeactivateAsync(@event.UserId, cancellationToken);
    }
}
