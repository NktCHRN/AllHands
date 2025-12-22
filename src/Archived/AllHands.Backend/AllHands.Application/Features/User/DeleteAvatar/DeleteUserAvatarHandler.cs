using AllHands.Application.Abstractions;
using AllHands.Domain.Events.Employee;
using Marten;
using MediatR;

namespace AllHands.Application.Features.User.DeleteAvatar;

public sealed class DeleteUserAvatarHandler(IFileService fileService, ICurrentUserService currentUserService, IDocumentSession documentSession) : IRequestHandler<DeleteUserAvatarCommand>
{
    public async Task Handle(DeleteUserAvatarCommand request, CancellationToken cancellationToken)
    {
        var employeeId = currentUserService.GetEmployeeId();
        await fileService.DeleteAvatarAsync(employeeId.ToString(), cancellationToken);
        
        documentSession.Events.Append(employeeId, new EmployeeAvatarUpdated(employeeId, currentUserService.GetId()));
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
