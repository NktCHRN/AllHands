using AllHands.EmployeeService.Application.Abstractions;
using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using Marten;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.User.GetAvatar;

public sealed class GetAvatarHandler(IUserContext userContext, IQuerySession querySession, IFileService fileService) : IRequestHandler<GetAvatarQuery, GetAvatarResult>
{
    public async Task<GetAvatarResult> Handle(GetAvatarQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = userContext.Id;
        
        var employee = await querySession.Query<Domain.Models.Employee>()
                           .FirstOrDefaultAsync(e => e.UserId == currentUserId, token: cancellationToken)
                       ?? throw new EntityNotFoundException("User was not found");

        return new GetAvatarResult(await fileService.GetAvatarAsync(employee.Id.ToString(), cancellationToken));
    }
}
