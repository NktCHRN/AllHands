using AllHands.TimeOffService.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.TimeOffService.Application.Features.Companies.Save;

public sealed class SaveCompanyCommandHandler(IDocumentSession documentSession) : IRequestHandler<SaveCompanyCommand>
{
    public async Task Handle(SaveCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await documentSession.Query<Company>()
            .FirstOrDefaultAsync(e => e.Id == request.Id, token: cancellationToken);

        if (company is null)
        {
            company = new Company
            {
                Id = request.Id,
                CreatedAt = request.EventOccurredAt,
                Name = request.Name,
                IanaTimeZone = request.IanaTimeZone,
                WorkDays = request.WorkDays,
                UpdatedAt = request.EventOccurredAt
            };
        }
        else
        {
            company.Name = request.Name;
            company.IanaTimeZone = request.IanaTimeZone;
            company.WorkDays = request.WorkDays;
            company.UpdatedAt = request.EventOccurredAt;
        }
        
        documentSession.Store(company);
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
