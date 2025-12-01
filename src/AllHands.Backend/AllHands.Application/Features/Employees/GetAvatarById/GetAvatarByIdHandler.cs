using AllHands.Application.Abstractions;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.Employees.GetAvatarById;

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
