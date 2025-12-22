using AllHands.EmployeeService.Application.Dto;
using AllHands.EmployeeService.Domain.Models;
using AllHands.Shared.Domain.Exceptions;
using Marten;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.Employees.GetById;

public sealed class GetEmployeeByIdHandler(IQuerySession querySession) : IRequestHandler<GetEmployeeByIdQuery, EmployeeDetailsDto>
{
    public async Task<EmployeeDetailsDto> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        Position position = null!;
        Employee manager = null!;
        Domain.Models.Company company = null!;
        Role role = null!;
        var employee = await querySession.Query<Employee>()
                           .Include<Position>(x => position = x).On(x => x.PositionId)
                           .Include<Employee>(x => manager = x).On(x => x.ManagerId)
                           .Include<Domain.Models.Company>(x => company = x).On(x => x.CompanyId)
                           .Include<Role>(x => role = x).On(x => x.RoleId)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new EntityNotFoundException("Employee was not found.");

        employee.Position = position;
        employee.Manager = manager;
        employee.Company = company;
        employee.Role = role;

        if (employee.Manager is not null)
        {
            employee.Manager.Position = await querySession.Query<Position>()
                .FirstOrDefaultAsync(x => x.Id == employee.Manager.PositionId,
                    token: cancellationToken);
        }
        
        return EmployeeDetailsDto.FromModel(employee);
    }
}
