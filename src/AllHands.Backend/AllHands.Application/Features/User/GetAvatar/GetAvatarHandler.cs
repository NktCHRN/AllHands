using AllHands.Application.Abstractions;
using MediatR;

namespace AllHands.Application.Features.User.GetAvatar;

public sealed class GetAvatarHandler(ICurrentUserService currentUserService, IFileService fileService) : IRequestHandler<GetAvatarQuery, GetAvatarResult>
{
    public async Task<GetAvatarResult> Handle(GetAvatarQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.GetId();

        return new GetAvatarResult(await fileService.GetAvatarAsync(currentUserId.ToString(), cancellationToken));
    }
}
