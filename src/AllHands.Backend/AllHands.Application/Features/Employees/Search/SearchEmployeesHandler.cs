using AllHands.Application.Dto;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.Employees.Search;

public sealed class SearchEmployeesHandler(IQuerySession querySession) : IRequestHandler<SearchEmployeesQuery, PagedDto<EmployeeSearchDto>>
{
    public async Task<PagedDto<EmployeeSearchDto>> Handle(SearchEmployeesQuery request, CancellationToken cancellationToken)
    {
        var positions = new Dictionary<Guid, Position>();
        IQueryable<Employee> query = querySession.Query<Employee>();

        if (request.ManagerId.HasValue)
        {
            query = query.Where(e => e.ManagerId == request.ManagerId.Value);
        }

        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(x => x.PlainTextSearch(request.Search));
        }

        if (request.Status != EmployeeStatus.Undefined)
        {
            query = query.Where(x => x.Status == request.Status);
        }

        var count = await query.CountAsync(cancellationToken);
        
        var employees = await query
            .Include(positions).On(e => e.PositionId)
            .OrderByDescending(e => e.Id)
            .Skip((request.Page - 1) * request.PerPage)
            .Take(request.PerPage)
            .ToListAsync(token: cancellationToken);

        var employeeDtos = new List<EmployeeSearchDto>();
        foreach (var employee in employees)
        {
            employee.Position = positions.GetValueOrDefault(employee.PositionId);
            var dto = new EmployeeSearchDto(
                employee.Id,
                employee.FirstName,
                employee.MiddleName,
                employee.LastName,
                employee.Email,
                employee.PhoneNumber,
                employee.Status,
                employee.Position is not null 
                ? new PositionDto()
                {
                    Id = employee.Position.Id,
                    Name = employee.Position.Name
                } : null);
            employeeDtos.Add(dto);
        }
        
        return new PagedDto<EmployeeSearchDto>(employeeDtos, count);
    }
}
