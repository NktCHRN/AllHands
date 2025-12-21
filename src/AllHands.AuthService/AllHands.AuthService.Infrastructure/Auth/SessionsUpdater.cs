using AllHands.Shared.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AllHands.AuthService.Infrastructure.Auth;

public sealed class SessionsUpdater(IDbContextFactory<AuthDbContext> dbContextFactory, ITicketModifier ticketModifier, IUserClaimsFactory userClaimsFactory) : ISessionsUpdater
{
    public async Task UpdateAll(Guid companyId, int batchSize, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

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
                    await ticketModifier.UpdateClaimsAsync(
                        dbContext,
                        user.Id,
                        () => userClaimsFactory.CreateClaims(user),
                        takeOnlyNewClaims: true,
                        cancellationToken);
                }
            
            await transaction.CommitAsync(cancellationToken);
        }
    }
    
    public async Task UpdateUser(Guid userId, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var user = await dbContext.Users
            .IgnoreQueryFilters()
            .Include(u => u.Roles.Where(r => r.Role != null && !r.Role.DeletedAt.HasValue))
            .ThenInclude(r => r.Role)
            .ThenInclude(r => r!.Claims)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new EntityNotFoundException("User was not found");

        if (!user.DeactivatedAt.HasValue && !user.DeletedAt.HasValue)
        {
            await ticketModifier.UpdateClaimsAsync(
                dbContext,
                user.Id,
                () => userClaimsFactory.CreateClaims(user),
                takeOnlyNewClaims: true,
                cancellationToken);
        }
        else
        {
            await ticketModifier.ExpireActiveSessionsAsync(
                dbContext, 
                user.Id, 
                cancellationToken);
        }
    }

    public async Task ExpireUser(Guid userId, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var user = await dbContext.Users
                       .AsNoTracking()
                       .IgnoreQueryFilters()
                       .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
                   ?? throw new EntityNotFoundException("User was not found");
        
            await ticketModifier.ExpireActiveSessionsAsync(
                dbContext, 
                user.Id, 
                cancellationToken);
    }
}
