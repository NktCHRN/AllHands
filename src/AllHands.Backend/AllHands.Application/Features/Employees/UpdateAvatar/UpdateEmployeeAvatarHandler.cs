using AllHands.Application.Abstractions;
using AllHands.Application.Dto;
using AllHands.Domain.Events.Employee;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.Employees.UpdateAvatar;

public sealed class UpdateEmployeeAvatarHandler(IDocumentSession documentSession, IFileService fileService, ICurrentUserService currentUserService, IImageValidator imageValidator) : IRequestHandler<UpdateEmployeeAvatarCommand>
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

        documentSession.Events.Append(request.EmployeeId, new EmployeeAvatarUpdated(request.EmployeeId, currentUserService.GetId()));
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
