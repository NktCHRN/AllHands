using AllHands.Application.Abstractions;
using MediatR;

namespace AllHands.Application.Features.Company.GetLogo;

public sealed class GetCompanyLogoHandler(ICurrentUserService currentUserService, IFileService fileService) : IRequestHandler<GetCompanyLogoQuery, GetCompanyLogoResult>
{
    public async Task<GetCompanyLogoResult> Handle(GetCompanyLogoQuery request, CancellationToken cancellationToken)
    {
        var companyId = currentUserService.GetCompanyId();
        
        var file = await fileService.GetCompanyLogoAsync(companyId.ToString(), cancellationToken);
        
        return new GetCompanyLogoResult(file);
    }
}
