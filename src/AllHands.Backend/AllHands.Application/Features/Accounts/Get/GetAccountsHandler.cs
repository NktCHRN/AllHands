using AllHands.Application.Abstractions;
using AllHands.Application.Dto;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.Accounts.Get;

public sealed class GetAccountsHandler(ICurrentUserService currentUserService, IQuerySession querySession, IAccountService accountService) : IRequestHandler<GetAccountsQuery, GetAccountsResult>
{
    public async Task<GetAccountsResult> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.GetId();
        var usersIds = await accountService.GetUserIds(currentUserId);

        var companies = new Dictionary<Guid, Domain.Models.Company>();
        var employees = await querySession.Query<Employee>()
            .Include(companies).On(u => u.CompanyId)
            .Where(e => usersIds.Contains(e.UserId))
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
