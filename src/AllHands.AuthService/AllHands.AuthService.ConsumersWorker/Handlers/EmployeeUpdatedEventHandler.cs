using AllHands.AuthService.Application.Abstractions;
using AllHands.AuthService.Application.Features.Employees.Update;
using AllHands.Shared.Contracts.Messaging.Events.Employees;

namespace AllHands.AuthService.ConsumersWorker.Handlers;

public sealed class EmployeeUpdatedEventHandler(IAccountService accountService)
{
    public async Task Handle(EmployeeUpdatedEvent @event, CancellationToken cancellationToken)
    {
        await accountService.UpdateAsync(
            new UpdateEmployeeCommand(
                @event.UserId,
                @event.PositionId,
                @event.ManagerId,
                @event.Email,
                @event.FirstName,
                @event.MiddleName,
                @event.LastName,
                @event.PhoneNumber,
                @event.WorkStartDate), 
            cancellationToken);
    }
}
