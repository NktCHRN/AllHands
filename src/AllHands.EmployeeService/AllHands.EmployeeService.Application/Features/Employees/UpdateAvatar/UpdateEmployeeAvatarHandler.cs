using AllHands.EmployeeService.Application.Abstractions;
using AllHands.EmployeeService.Application.Dto;
using AllHands.EmployeeService.Domain.Events.Employee;
using AllHands.EmployeeService.Domain.Models;
using AllHands.Shared.Application.Abstractions;
using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using Marten;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.Employees.UpdateAvatar;

public sealed class UpdateEmployeeAvatarHandler(IDocumentSession documentSession, IFileService fileService, IUserContext userContext, IImageValidator imageValidator) : IRequestHandler<UpdateEmployeeAvatarCommand>
{
    public async Task Handle(UpdateEmployeeAvatarCommand request, CancellationToken cancellationToken)
    {
        var employeeExists = await documentSession.Query<Employee>()
                           .AnyAsync(e => e.Id == request.EmployeeId, cancellationToken);

        if (!employeeExists)
        {
            throw new EntityNotFoundException("Employee was not found.");
        }
        
        var isValid = imageValidator.IsValidImage(request.Stream);
        if (!isValid)
        {
            throw new EntityValidationFailedException("Image has invalid format.");
        }
        
        await fileService.SaveAvatarAsync(
            new AllHandsFile(
                request.Stream,
                request.EmployeeId.ToString(),
                request.ContentType) {OriginalFileName = request.Name}, cancellationToken);

        documentSession.Events.Append(request.EmployeeId, new EmployeeAvatarUpdated(request.EmployeeId, userContext.Id));
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
