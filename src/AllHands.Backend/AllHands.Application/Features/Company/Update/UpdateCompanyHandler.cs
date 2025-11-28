using AllHands.Application.Abstractions;
using AllHands.Domain.Exceptions;
using Marten;
using MediatR;

namespace AllHands.Application.Features.Company.Update;

public sealed class UpdateCompanyHandler(ICurrentUserService currentUserService, IDocumentSession documentSession) : IRequestHandler<UpdateCompanyCommand>
{
    public async Task Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var companyId = currentUserService.GetCompanyId();
        
        var company = await documentSession.Query<Domain.Models.Company>()
            .FirstOrDefaultAsync(c => c.Id == companyId, token: cancellationToken)
            ?? throw new EntityNotFoundException("Company was not found");
        
        company.Name = request.Name;
        company.Description = request.Description;
        company.EmailDomain = request.EmailDomain;
        company.IsSameDomainValidationEnforced = request.IsSameDomainValidationEnforced;
        company.IanaTimeZone = request.IanaTimeZone;
        company.WorkDays = request.WorkDays.ToHashSet();
        
        documentSession.Update(company);
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
