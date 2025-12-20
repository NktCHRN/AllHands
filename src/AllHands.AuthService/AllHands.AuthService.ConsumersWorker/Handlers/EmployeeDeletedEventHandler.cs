using AllHands.AuthService.Application.Abstractions;
using AllHands.Shared.Contracts.Messaging.Events.Employees;

namespace AllHands.AuthService.ConsumersWorker.Handlers;

public sealed class EmployeeDeletedEventHandler(IAccountService accountService)
{
    public async Task Handle(EmployeeDeletedEvent @event, CancellationToken cancellationToken)
    {
        await accountService.DeleteAsync(@event.UserId, cancellationToken);
    }
}
