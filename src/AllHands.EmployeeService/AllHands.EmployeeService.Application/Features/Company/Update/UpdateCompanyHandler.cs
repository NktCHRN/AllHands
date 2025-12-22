using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using Marten;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.Company.Update;

public sealed class UpdateCompanyHandler(IUserContext userContext, IDocumentSession documentSession) : IRequestHandler<UpdateCompanyCommand>
{
    public async Task Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var companyId = userContext.CompanyId;
        
        var company = await documentSession.LoadAsync<Domain.Models.Company>(companyId, cancellationToken)
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
