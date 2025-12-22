using AllHands.EmployeeService.Application.Abstractions;
using AllHands.EmployeeService.Domain.Events.Employee;
using AllHands.Shared.Domain.UserContext;
using Marten;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.User.DeleteAvatar;

public sealed class DeleteUserAvatarHandler(IFileService fileService, IUserContext userContext, IDocumentSession documentSession) : IRequestHandler<DeleteUserAvatarCommand>
{
    public async Task Handle(DeleteUserAvatarCommand request, CancellationToken cancellationToken)
    {
        var employeeId = userContext.EmployeeId;
        await fileService.DeleteAvatarAsync(employeeId.ToString(), cancellationToken);
        
        documentSession.Events.Append(employeeId, new EmployeeAvatarUpdated(employeeId, userContext.Id));
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
