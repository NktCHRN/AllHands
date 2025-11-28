using AllHands.Application.Abstractions;
using AllHands.Application.Dto;
using AllHands.Domain.Events.Employee;
using AllHands.Domain.Exceptions;
using Marten;
using MediatR;

namespace AllHands.Application.Features.User.UpdateAvatar;

public sealed class UpdateUserAvatarHandler(IFileService fileService, ICurrentUserService currentUserService, IImageValidator imageValidator, IDocumentSession documentSession) : IRequestHandler<UpdateUserAvatarCommand>
{
    public async Task Handle(UpdateUserAvatarCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetId();
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
                userId.ToString(),
                request.ContentType) {OriginalFileName = request.Name}, cancellationToken);

        documentSession.Events.Append(employee.Id, new EmployeeAvatarUpdated(employee.Id, userId));
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
