using AllHands.NewsService.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.NewsService.Application.Features.Employees.Save;

public sealed class SaveEmployeeCommandHandler(IDocumentSession documentSession) : IRequestHandler<SaveEmployeeCommand>
{
    public async Task Handle(SaveEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await documentSession.Query<Employee>()
            .FirstOrDefaultAsync(e => e.Id == request.Id, token: cancellationToken);

        if (employee is null)
        {
            employee = new Employee
            {
                Id = request.Id,
                CreatedAt = request.EventOccurredAt,
                CompanyId = request.CompanyId,
            };
        }
        else
        {
            employee.UpdatedAt = request.EventOccurredAt;
        }
        
        employee.FirstName = request.FirstName;
        employee.MiddleName = request.MiddleName;
        employee.LastName = request.LastName;
        employee.Email = request.Email;
        
        documentSession.Store(employee);
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
