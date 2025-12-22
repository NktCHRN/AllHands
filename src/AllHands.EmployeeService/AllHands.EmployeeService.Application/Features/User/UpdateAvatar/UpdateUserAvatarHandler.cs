using AllHands.EmployeeService.Application.Abstractions;
using AllHands.EmployeeService.Application.Dto;
using AllHands.EmployeeService.Domain.Events.Employee;
using AllHands.Shared.Application.Abstractions;
using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using Marten;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.User.UpdateAvatar;

public sealed class UpdateUserAvatarHandler(IFileService fileService, IUserContext userContext, IImageValidator imageValidator, IDocumentSession documentSession) : IRequestHandler<UpdateUserAvatarCommand>
{
    public async Task Handle(UpdateUserAvatarCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.Id;
        var employee = await documentSession.Query<Domain.Models.Employee>()
                           .FirstOrDefaultAsync(e => e.UserId == userId, token: cancellationToken)
                       ?? throw new EntityNotFoundException("User was not found");
        
        var isValid = imageValidator.IsValidImage(request.Stream);
        if (!isValid)
        {
            throw new EntityValidationFailedException("Image has invalid format.");
        }
        
        await fileService.SaveAvatarAsync(
            new AllHandsFile(
                request.Stream,
                employee.Id.ToString(),
                request.ContentType) {OriginalFileName = request.Name}, cancellationToken);

        documentSession.Events.Append(employee.Id, new EmployeeAvatarUpdated(employee.Id, userId));
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
