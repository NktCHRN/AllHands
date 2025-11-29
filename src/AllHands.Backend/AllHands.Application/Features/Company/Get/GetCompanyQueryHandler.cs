using AllHands.Domain.Exceptions;
using Marten;
using MediatR;

namespace AllHands.Application.Features.Company.Get;

public sealed class GetCompanyQueryHandler(IQuerySession querySession) : IRequestHandler<GetCompanyQuery, GetCompanyResult>
{
    public async Task<GetCompanyResult> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
    {
        var company = await querySession.Query<Domain.Models.Company>()
            .FirstOrDefaultAsync(token: cancellationToken)
            ?? throw new EntityNotFoundException("Company was not found.");

        return new GetCompanyResult(
            company.Id,
            company.Name,
            company.Description,
            company.EmailDomain,
            company.IsSameDomainValidationEnforced,
            company.IanaTimeZone,
            company.WorkDays.Order().ToList(),
            company.CreatedAt,
            company.UpdatedAt,
            company.DeletedAt);
    }
}
