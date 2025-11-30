using AllHands.Application.Abstractions;
using AllHands.Domain.Exceptions;
using Marten;
using MediatR;

namespace AllHands.Application.Features.User.GetAvatar;

public sealed class GetAvatarHandler(ICurrentUserService currentUserService, IQuerySession querySession, IFileService fileService) : IRequestHandler<GetAvatarQuery, GetAvatarResult>
{
    public async Task<GetAvatarResult> Handle(GetAvatarQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.GetId();
        
        var employee = await querySession.Query<Domain.Models.Employee>()
                           .FirstOrDefaultAsync(e => e.UserId == currentUserId, token: cancellationToken)
                       ?? throw new EntityNotFoundException("User was not found");

        return new GetAvatarResult(await fileService.GetAvatarAsync(employee.Id.ToString(), cancellationToken));
    }
}
