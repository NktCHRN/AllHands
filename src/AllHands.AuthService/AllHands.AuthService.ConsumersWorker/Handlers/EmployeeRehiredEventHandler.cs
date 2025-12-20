using AllHands.AuthService.Application.Abstractions;
using AllHands.Shared.Contracts.Messaging.Events.Employees;

namespace AllHands.AuthService.ConsumersWorker.Handlers;

public sealed class EmployeeRehiredEventHandler(IAccountService accountService)
{
    public async Task Handle(EmployeeRehiredEvent @event, CancellationToken cancellationToken)
    {
        await accountService.ReactivateAsync(@event.UserId, cancellationToken);
    }
}
