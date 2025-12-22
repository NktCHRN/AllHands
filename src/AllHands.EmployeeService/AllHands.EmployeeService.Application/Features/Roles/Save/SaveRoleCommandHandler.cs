using AllHands.EmployeeService.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.Roles.Save;

public sealed class SaveRoleCommandHandler(IDocumentSession documentSession) : IRequestHandler<SaveRoleCommand>
{
    public async Task Handle(SaveRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await documentSession.Query<Role>()
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (role is null)
        {
            role = new Role()
            {
                Id = request.Id,
                Name = request.Name,
                IsDefault = request.IsDefault,
                CompanyId = request.CompanyId
            };
        }
        else
        {
            role.Name = request.Name;
            role.IsDefault = request.IsDefault;
        }
        
        documentSession.Store(role);
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
