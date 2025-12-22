using AllHands.EmployeeService.Application.Abstractions;
using AllHands.Shared.Domain.UserContext;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.Company.GetLogo;

public sealed class GetCompanyLogoHandler(IUserContext userContext, IFileService fileService) : IRequestHandler<GetCompanyLogoQuery, GetCompanyLogoResult>
{
    public async Task<GetCompanyLogoResult> Handle(GetCompanyLogoQuery request, CancellationToken cancellationToken)
    {
        var companyId = userContext.CompanyId;
        
        var file = await fileService.GetCompanyLogoAsync(companyId.ToString(), cancellationToken);
        
        return new GetCompanyLogoResult(file);
    }
}
