using AllHands.EmployeeService.Application.Abstractions;
using AllHands.Shared.Domain.UserContext;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.Company.DeleteLogo;

public sealed class DeleteCompanyLogoHandler(IFileService fileService, IUserContext userContext) : IRequestHandler<DeleteCompanyLogoCommand>
{
    public async Task Handle(DeleteCompanyLogoCommand request, CancellationToken cancellationToken)
    {
        await fileService.DeleteCompanyLogoAsync(userContext.CompanyId.ToString(), cancellationToken);
    }
}
