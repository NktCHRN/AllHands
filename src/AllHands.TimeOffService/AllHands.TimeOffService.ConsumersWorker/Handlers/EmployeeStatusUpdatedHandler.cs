using AllHands.Shared.Contracts.Messaging.Events.Employees;
using AllHands.TimeOffService.Application.Features.Employees.UpdateStatus;
using MediatR;

namespace AllHands.TimeOffService.ConsumersWorker.Handlers;

public sealed class EmployeeStatusUpdatedHandler(ILogger<EmployeeStatusUpdatedHandler> logger, IMediator mediator)
{
    public async Task Handle(EmployeeStatusUpdated @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received EmployeeUpdatedEvent {Event}", @event);
        
        await mediator.Send(new UpdateEmployeeStatusCommand(
            @event.Id,
            @event.Status,
            @event.OccurredAt), cancellationToken);
        
        logger.LogInformation("Processed EmployeeUpdatedEvent {Event}", @event);
    }
}
