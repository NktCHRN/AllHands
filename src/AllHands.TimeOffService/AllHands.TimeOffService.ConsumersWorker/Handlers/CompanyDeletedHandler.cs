using AllHands.Shared.Contracts.Messaging.Events.Companies;
using AllHands.TimeOffService.Application.Features.Companies.Delete;
using MediatR;

namespace AllHands.TimeOffService.ConsumersWorker.Handlers;

public sealed class CompanyDeletedHandler(ILogger<CompanyDeletedHandler> logger, IMediator mediator)
{
    public async Task Handle(CompanyDeletedEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received CompanyDeletedEvent {Event}", @event);
        
        await mediator.Send(new DeleteCompanyCommand(@event.Id), cancellationToken);
        
        logger.LogInformation("Processed CompanyDeletedEvent {Event}", @event);
    }
}
