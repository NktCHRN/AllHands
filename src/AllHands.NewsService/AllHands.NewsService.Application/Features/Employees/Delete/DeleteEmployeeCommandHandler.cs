using AllHands.NewsService.Domain.Models;
using AllHands.Shared.Domain.Exceptions;
using Marten;
using MediatR;

namespace AllHands.NewsService.Application.Features.Employees.Delete;

public sealed class DeleteEmployeeCommandHandler(IDocumentSession documentSession) : IRequestHandler<DeleteEmployeeCommand>
{
    public async Task Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await documentSession.Query<Employee>()
            .FirstOrDefaultAsync(e => e.Id == request.Id, token: cancellationToken)
            ?? throw new EntityNotFoundException("Employee not found");
        
        documentSession.Delete(employee);
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
