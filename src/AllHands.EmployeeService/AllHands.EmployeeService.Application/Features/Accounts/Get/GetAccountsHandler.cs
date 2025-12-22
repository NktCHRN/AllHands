using AllHands.EmployeeService.Application.Dto;
using AllHands.EmployeeService.Domain.Models;
using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using Marten;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.Accounts.Get;

public sealed class GetAccountsHandler(IUserContext userContext, IQuerySession querySession) : IRequestHandler<GetAccountsQuery, GetAccountsResult>
{
    public async Task<GetAccountsResult> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = userContext.Id;
        var currentEmployee = await querySession.Query<Employee>()
            .FirstOrDefaultAsync(e => e.UserId == currentUserId, cancellationToken)
            ?? throw new EntityNotFoundException("User not found");

        var companies = new Dictionary<Guid, Domain.Models.Company>();
        var employees = await querySession.Query<Employee>()
            .Include(companies).On(u => u.CompanyId)
            .Where(e => e.AnyTenant() && currentEmployee.GlobalUserId == e.GlobalUserId)
            .ToListAsync(cancellationToken);

        foreach (var employee in employees)
        {
            _ = companies.TryGetValue(employee.CompanyId, out var company);
            employee.Company = company ?? throw new EntityNotFoundException("Company not found");
        }

        return new GetAccountsResult(
            employees.Select(e => new AccountDto(
                e.Id,
                e.FirstName,
                e.MiddleName,
                e.LastName,
                e.Email,
                new CompanyDto
                {
                    Id = e.CompanyId,
                    Name = e.Company!.Name
                })).ToList());
    }
}
