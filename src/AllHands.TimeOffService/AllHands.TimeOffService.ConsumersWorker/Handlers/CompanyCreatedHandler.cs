using AllHands.Shared.Contracts.Messaging.Events.Companies;
using AllHands.TimeOffService.Application.Features.Companies.Save;
using MediatR;

namespace AllHands.TimeOffService.ConsumersWorker.Handlers;

public sealed class CompanyCreatedHandler(ILogger<CompanyCreatedHandler> logger, IMediator mediator)
{
    public async Task Handle(CompanyCreatedEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received CompanyCreatedEvent {Event}", @event);
        
        await mediator.Send(new SaveCompanyCommand(
            @event.Id,
            @event.Name,
            @event.IanaTimeZone,
            @event.WorkDays,
            @event.OccurredAt), cancellationToken);
        
        logger.LogInformation("Processed CompanyCreatedEvent {Event}", @event);
    }
}
