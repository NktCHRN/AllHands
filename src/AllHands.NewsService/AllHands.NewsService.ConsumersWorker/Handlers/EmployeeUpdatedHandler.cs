using AllHands.NewsService.Application.Features.Employees.Save;
using AllHands.Shared.Contracts.Messaging.Events.Employees;
using MediatR;

namespace AllHands.NewsService.ConsumersWorker.Handlers;

public sealed class EmployeeUpdatedHandler(ILogger<EmployeeUpdatedHandler> logger, IMediator mediator)
{
    public async Task Handle(EmployeeUpdatedEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received EmployeeUpdatedEvent {Event}", @event);
        
        await mediator.Send(new SaveEmployeeCommand(
            @event.Id,
            @event.FirstName,
            @event.MiddleName,
            @event.LastName,
            @event.Email,
            @event.CompanyId,
            @event.OccurredAt), cancellationToken);
        
        logger.LogInformation("Processed EmployeeUpdatedEvent {Event}", @event);
    }
}
