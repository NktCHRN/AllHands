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
    
            // Default time off types. You can change this setup later.
            var timeOffTypes = new[]
            {
                new TimeOffType()
                {
                    Id = Guid.CreateVersion7(),
                    CompanyId = company.Id,
                    CreatedAt = DateTime.UtcNow,
                    Emoji = "🌴",
                    Name = "Vacation",
                    DaysPerYear = 20,
                    Order = 1
                },
                new TimeOffType()
                {
                    Id = Guid.CreateVersion7(),
                    CompanyId = company.Id,
                    CreatedAt = DateTime.UtcNow,
                    Emoji = "📅",
                    Name = "Unpaid time off",
                    DaysPerYear = 0,
                    Order = 2
                },
                new TimeOffType()
                {
                    Id = Guid.CreateVersion7(),
                    CompanyId = company.Id,
                    CreatedAt = DateTime.UtcNow,
                    Emoji = "🤒",
                    Name = "Sick leave (Undocumented)",
                    DaysPerYear = 0,
                    Order = 3
                },
                new TimeOffType()
                {
                    Id = Guid.CreateVersion7(),
                    CompanyId = company.Id,
                    CreatedAt = DateTime.UtcNow,
                    Emoji = "🏥",
                    Name = "Sick leave (documented)",
                    DaysPerYear = 0,
                    Order = 4
                },
                new TimeOffType()
                {
                    Id = Guid.CreateVersion7(),
                    CompanyId = company.Id,
                    CreatedAt = DateTime.UtcNow,
                    Emoji = "👶",
                    Name = "Parental leave",
                    DaysPerYear = 0,
                    Order = 5
                }
            };
            documentSession.Insert(timeOffTypes);
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
