using AllHands.Application.Abstractions;
using AllHands.Application.Dto;
using AllHands.Domain.Exceptions;
using MediatR;

namespace AllHands.Application.Features.Company.UpdateLogo;

public sealed class UpdateLogoHandler(IFileService fileService, ICurrentUserService currentUserService, IImageValidator imageValidator) : IRequestHandler<UpdateLogoCommand>
{
    public async Task Handle(UpdateLogoCommand request, CancellationToken cancellationToken)
    {
        var companyId = currentUserService.GetCompanyId();
        
        var isValid = imageValidator.IsValidImage(request.Stream);
        if (!isValid)
        {
            throw new EntityValidationFailedException("Image has invalid format.");
        }
        
        await fileService.SaveCompanyLogoAsync(
            new AllHandsFile(
                request.Stream,
                companyId.ToString(),
                request.ContentType) {OriginalFileName = request.Name}, cancellationToken);
    }
}
