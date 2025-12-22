using AllHands.Application.Abstractions;
using AllHands.Application.Dto;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.Employees.GetById;

public sealed class GetEmployeeByIdHandler(IQuerySession querySession, IAccountService accountService) : IRequestHandler<GetEmployeeByIdQuery, EmployeeDetailsDto>
{
    public async Task<EmployeeDetailsDto> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        Position position = null!;
        Employee manager = null!;
        Domain.Models.Company company = null!;
        var employee = await querySession.Query<Employee>()
                           .Include<Position>(x => position = x).On(x => x.PositionId)
                           .Include<Employee>(x => manager = x).On(x => x.ManagerId)
                           .Include<Domain.Models.Company>(x => company = x).On(x => x.CompanyId)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new EntityNotFoundException("Employee was not found.");

        employee.Position = position;
        employee.Manager = manager;
        employee.Company = company;

        if (employee.Manager is not null)
        {
            employee.Manager.Position = await querySession.Query<Position>()
                .FirstOrDefaultAsync(x => x.Id == employee.Manager.PositionId,
                    token: cancellationToken);
        }
        
        var role = await accountService.GetRoleByUserIdAsync(employee.UserId, cancellationToken);
        
        return EmployeeDetailsDto.FromModel(employee, role);
    }
}
