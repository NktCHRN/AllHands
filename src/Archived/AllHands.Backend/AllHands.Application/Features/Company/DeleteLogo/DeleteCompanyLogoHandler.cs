using AllHands.Application.Abstractions;
using MediatR;

namespace AllHands.Application.Features.Company.DeleteLogo;

public sealed class DeleteCompanyLogoHandler(IFileService fileService, ICurrentUserService currentUserService) : IRequestHandler<DeleteCompanyLogoCommand>
{
    public async Task Handle(DeleteCompanyLogoCommand request, CancellationToken cancellationToken)
    {
        await fileService.DeleteCompanyLogoAsync(currentUserService.GetCompanyId().ToString(), cancellationToken);
    }
}
