using AllHands.Application.Abstractions;
using AllHands.Domain.Exceptions;
using Marten;
using MediatR;

namespace AllHands.Application.Features.Company.Get;

public sealed class GetCompanyQueryHandler(ICurrentUserService currentUserService, IQuerySession querySession) : IRequestHandler<GetCompanyQuery, GetCompanyResult>
{
    public async Task<GetCompanyResult> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
    {
        var companyId = currentUserService.GetCompanyId();

        var company = await querySession.Query<Domain.Models.Company>()
            .FirstOrDefaultAsync(x => x.Id == companyId, token: cancellationToken)
            ?? throw new EntityNotFoundException("Company was not found.");

        return new GetCompanyResult(
            company.Id,
            company.Name,
            company.EmailDomain,
            company.IanaTimeZone,
            company.CreatedAt,
            company.UpdatedAt,
            company.DeletedAt);
    }
}
