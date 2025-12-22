using AllHands.EmployeeService.Application.Abstractions;
using AllHands.EmployeeService.Domain.Models;
using AllHands.Shared.Domain.Exceptions;
using Marten;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.Employees.GetAvatarById;

public sealed class GetAvatarByIdHandler(IQuerySession querySession, IFileService fileService) : IRequestHandler<GetAvatarByIdQuery, GetAvatarByIdResult>
{
    public async Task<GetAvatarByIdResult> Handle(GetAvatarByIdQuery request, CancellationToken cancellationToken)
    {
        var employeeExists = await querySession.Query<Employee>()
            .AnyAsync(e => e.Id == request.EmployeeId, cancellationToken);

        if (!employeeExists)
        {
            throw new EntityNotFoundException("Employee was not found.");
        }
        
        return new GetAvatarByIdResult(await fileService.GetAvatarAsync(request.EmployeeId.ToString(), cancellationToken));
    }
}
