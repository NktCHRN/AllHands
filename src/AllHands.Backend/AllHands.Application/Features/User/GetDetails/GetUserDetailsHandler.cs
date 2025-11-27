using AllHands.Application.Abstractions;
using AllHands.Application.Dto;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.User.GetDetails;

public sealed class GetUserDetailsHandler(ICurrentUserService currentUserService, IQuerySession querySession) : IRequestHandler<GetUserDetailsQuery, GetUserDetailsResult>
{
    public async Task<GetUserDetailsResult> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
    {
        var currentUser = currentUserService.GetCurrentUser();

        var employee = await querySession.Query<Domain.Models.Employee>()
                           .FirstOrDefaultAsync(e => e.UserId == currentUser.Id, token: cancellationToken)
                       ?? throw new EntityNotFoundException("User was not found");

        employee.Position = await querySession.Query<Position>()
                                .FirstOrDefaultAsync(x => x.Id == employee.PositionId, cancellationToken)
                            ?? throw new EntityNotFoundException("Position was not found");

        employee.Manager = await querySession.Query<Domain.Models.Employee>()
                               .FirstOrDefaultAsync(x => x.Id == employee.ManagerId, token: cancellationToken)
                           ?? throw new EntityNotFoundException("Manager was not found");
        employee.Manager.Position = await querySession.Query<Position>()
            .FirstOrDefaultAsync(x => x.Id == employee.Manager.PositionId, token: cancellationToken)
                                    ?? throw new EntityNotFoundException("Manager position was not found");

        employee.Company = await querySession.Query<Domain.Models.Company>()
                               .FirstOrDefaultAsync(x => x.Id == employee.CompanyId, token: cancellationToken)
                           ?? throw new EntityNotFoundException("Company was not found");

        return new GetUserDetailsResult(
            employee.Id,
            employee.FirstName,
            employee.MiddleName,
            employee.LastName,
            employee.Email,
            employee.PhoneNumber,
            employee.WorkStartDate,
            new EmployeeDto{
        Id = employee.Manager.Id,
        FirstName = employee.Manager.FirstName,
        MiddleName = employee.Manager.MiddleName,
        LastName = employee.Manager.LastName,
        Email = employee.Manager.Email,
        PhoneNumber = employee.Manager.PhoneNumber,
        Position = new PositionDto
        {
            Id = employee.Manager.PositionId,
            Name = employee.Manager.Position.Name
        }
    },
    new PositionDto
            {
                Id = employee.PositionId, 
                Name = employee.Position.Name
            },
            new CompanyDto()
            {
                Id = employee.Company.Id,
                Name = employee.Company.Name
            });
    }
}
