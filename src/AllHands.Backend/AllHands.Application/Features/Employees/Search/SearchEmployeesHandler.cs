using AllHands.Application.Dto;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.Employees.Search;

public sealed class SearchEmployeesHandler(IQuerySession querySession) : IRequestHandler<SearchEmployeesQuery, PagedDto<EmployeeDto>>
{
    public async Task<PagedDto<EmployeeDto>> Handle(SearchEmployeesQuery request, CancellationToken cancellationToken)
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

        var count = await query.CountAsync(cancellationToken);
        
        var employees = await query
            .Include(positions).On(e => e.PositionId)
            .OrderByDescending(e => e.Id)
            .Skip((request.Page - 1) * request.PerPage)
            .Take(request.PerPage)
            .ToListAsync(token: cancellationToken);

        var employeeDtos = new List<EmployeeDto>();
        foreach (var employee in employees)
        {
            employee.Position = positions.GetValueOrDefault(employee.PositionId);
            employeeDtos.Add(EmployeeDto.FromModel(employee));
        }
        
        return new PagedDto<EmployeeDto>(employeeDtos, count);
    }
}
