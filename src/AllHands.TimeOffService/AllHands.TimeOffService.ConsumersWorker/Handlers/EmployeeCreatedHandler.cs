using AllHands.Shared.Contracts.Messaging.Events.Employees;
using AllHands.TimeOffService.Application.Features.Employees.Save;
using MediatR;

namespace AllHands.TimeOffService.ConsumersWorker.Handlers;

public sealed class EmployeeCreatedHandler(ILogger<EmployeeCreatedHandler> logger, IMediator mediator)
{
    public async Task Handle(EmployeeCreatedEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received EmployeeCreatedEvent {Event}", @event);
        
        await mediator.Send(new SaveEmployeeCommand(
            @event.Id,
            @event.FirstName,
            @event.MiddleName,
            @event.LastName,
            @event.Email,
            @event.CompanyId,
            @event.OccurredAt,
            @event.ManagerId,
            @event.Status,
            @event.WorkStartDate), cancellationToken);
        
        logger.LogInformation("Processed EmployeeCreatedEvent {Event}", @event);
    }
}
