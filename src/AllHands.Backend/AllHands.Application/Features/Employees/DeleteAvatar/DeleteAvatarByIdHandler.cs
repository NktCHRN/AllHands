using AllHands.Application.Abstractions;
using AllHands.Domain.Events.Employee;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.Employees.DeleteAvatar;

public sealed class DeleteAvatarByIdHandler(IDocumentSession documentSession, IFileService fileService, ICurrentUserService currentUserService) : IRequestHandler<DeleteAvatarByIdCommand>
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
        documentSession.Events.Append(request.EmployeeId, new EmployeeAvatarUpdated(request.EmployeeId, currentUserService.GetId()));
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
