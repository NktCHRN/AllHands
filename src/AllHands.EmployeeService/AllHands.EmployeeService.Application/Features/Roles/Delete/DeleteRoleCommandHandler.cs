using AllHands.EmployeeService.Domain.Models;
using AllHands.Shared.Domain.Exceptions;
using Marten;
using MediatR;
using Microsoft.Extensions.Options;

namespace AllHands.EmployeeService.Application.Features.Roles.Delete;

public sealed class DeleteRoleCommandHandler(IDocumentSession documentSession, IOptions<UserRoleUpdaterOptions> options) : IRequestHandler<DeleteRoleCommand>
{
    public async Task Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await documentSession.Query<Role>()
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
            ?? throw new EntityNotFoundException("Role was not found");
        
        var employeesInRoleCount = await documentSession.Query<Employee>().CountAsync(e => e.RoleId == role.Id, token: cancellationToken);
        for (var i = 0; i < employeesInRoleCount; i += options.Value.BatchSize)
        {
            var users = await documentSession.Query<Employee>()
                .Where(e => e.RoleId == role.Id)
                .Skip(i)
                .Take(options.Value.BatchSize)
                .ToListAsync(cancellationToken);

            foreach (var e in users)
            {
                e.RoleId = role.Id;
                documentSession.Update(e);
            }
            
            await documentSession.SaveChangesAsync(cancellationToken);
        }
        
        documentSession.Delete(role);
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
