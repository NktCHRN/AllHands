using AllHands.Shared.Domain.Exceptions;
using AllHands.TimeOffService.Application.Features.Employees.Save;
using AllHands.TimeOffService.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.TimeOffService.Application.Features.Employees.UpdateStatus;

public sealed class UpdateEmployeeStatusCommandHandler(IDocumentSession documentSession) : IRequestHandler<UpdateEmployeeStatusCommand>
{
    public async Task Handle(UpdateEmployeeStatusCommand request, CancellationToken cancellationToken)
    {
        var employee = await documentSession.Query<Employee>()
                           .FirstOrDefaultAsync(e => e.Id == request.Id, token: cancellationToken)
                       ?? throw new EntityNotFoundException("Employee not found");
        
        employee.Status = request.Status;
        employee.UpdatedAt = request.EventOccurredAt;
        
        documentSession.Update(employee);
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
