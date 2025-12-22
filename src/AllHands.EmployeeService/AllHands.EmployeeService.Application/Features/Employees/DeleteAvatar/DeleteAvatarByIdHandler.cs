using AllHands.EmployeeService.Application.Abstractions;
using AllHands.EmployeeService.Domain.Events.Employee;
using AllHands.EmployeeService.Domain.Models;
using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using Marten;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.Employees.DeleteAvatar;

public sealed class DeleteAvatarByIdHandler(IDocumentSession documentSession, IFileService fileService, IUserContext userContext) : IRequestHandler<DeleteAvatarByIdCommand>
{
    public async Task Handle(DeleteAvatarByIdCommand request, CancellationToken cancellationToken)
    {
        var employeeExists = await documentSession.Query<Employee>()
            .AnyAsync(e => e.Id == request.EmployeeId, cancellationToken);

        if (!employeeExists)
        {
            throw new EntityNotFoundException("Employee was not found.");
        }
        
        await fileService.DeleteAvatarAsync(request.EmployeeId.ToString(), cancellationToken);
        documentSession.Events.Append(request.EmployeeId, new EmployeeAvatarUpdated(request.EmployeeId, userContext.Id));
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
