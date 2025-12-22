using AllHands.EmployeeService.Application.Abstractions;
using AllHands.EmployeeService.Application.Dto;
using AllHands.Shared.Application.Abstractions;
using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.Company.UpdateLogo;

public sealed class UpdateLogoHandler(IFileService fileService, IUserContext userContext, IImageValidator imageValidator) : IRequestHandler<UpdateLogoCommand>
{
    public async Task Handle(UpdateLogoCommand request, CancellationToken cancellationToken)
    {
        var companyId = userContext.CompanyId;
        
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
