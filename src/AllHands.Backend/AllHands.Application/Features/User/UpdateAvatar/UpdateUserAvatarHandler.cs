using AllHands.Application.Abstractions;
using AllHands.Application.Dto;
using AllHands.Domain.Exceptions;
using MediatR;

namespace AllHands.Application.Features.User.UpdateAvatar;

public sealed class UpdateUserAvatarHandler(IFileService fileService, ICurrentUserService currentUserService, IImageValidator imageValidator) : IRequestHandler<UpdateUserAvatarCommand>
{
    public async Task Handle(UpdateUserAvatarCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetId();
        
        var isValid = imageValidator.IsValidImage(request.Stream);
        if (!isValid)
        {
            throw new EntityValidationFailedException("Image has invalid format.");
        }
        
        await fileService.SaveAvatarAsync(
            new AllHandsFile(
                request.Stream,
                userId.ToString(),
                request.ContentType) {OriginalFileName = request.Name}, cancellationToken);
    }
}
