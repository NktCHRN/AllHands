using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AllHands.Infrastructure.Auth;

public sealed class SessionsUpdater(IDbContextFactory<AuthDbContext> dbContextFactory, ITicketModifier ticketModifier, IUserClaimsFactory userClaimsFactory, Marten.IDocumentStore documentStore) : ISessionsUpdater
{
    public async Task UpdateAll(Guid companyId, int batchSize, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        await using var querySession = documentStore.QuerySession(companyId.ToString());

        var usersQueryable = dbContext.Users
            .AsNoTracking()
            .Include(u => u.Roles)
            .ThenInclude(r => r.Role)
            .ThenInclude(r => r!.Claims)
            .Where(u => u.CompanyId == companyId);
        var totalCount = await usersQueryable.CountAsync(cancellationToken);
        
        for (var skip = 0; skip < totalCount; skip += batchSize)
        {
            var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            
                var batch = await usersQueryable
                    .OrderBy(u => u.Id)
                    .Skip(skip)
                    .Take(batchSize)
                    .ToListAsync(cancellationToken);

                foreach (var user in batch)
                {
                    var employeeId = await querySession.Query<Employee>()
                        .Where(e => e.Id == user.Id)
                        .Select(e => e.Id)
                        .FirstOrDefaultAsync(cancellationToken);
                    if (employeeId == Guid.Empty)
                    {
                        throw new EntityNotFoundException("Employee was not found.");
                    }
                    await ticketModifier.UpdateClaimsAsync(
                        dbContext,
                        user.Id,
                        () => userClaimsFactory.CreateClaims(user, employeeId),
                        takeOnlyNewClaims: true,
                        cancellationToken);
                }
            
            await transaction.CommitAsync(cancellationToken);
        }
    }
    
    public async Task UpdateUser(AuthDbContext dbContext, Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .Include(u => u.Roles)
            .ThenInclude(r => r.Role)
            .ThenInclude(r => r!.Claims)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new EntityNotFoundException("User was not found");
        
        await using var querySession = documentStore.QuerySession(user.CompanyId.ToString());
        var employeeId = await querySession.Query<Employee>()
            .Where(e => e.Id == userId)
            .Select(e => e.Id)
            .FirstOrDefaultAsync(cancellationToken);
        if (employeeId == Guid.Empty)
        {
            throw new EntityNotFoundException("Employee was not found.");
        }
        
        await ticketModifier.UpdateClaimsAsync(
            dbContext, 
            user.Id, 
            () => userClaimsFactory.CreateClaims(user, employeeId), 
            takeOnlyNewClaims: true,
            cancellationToken);
    }
}
