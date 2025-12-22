namespace AllHands.Infrastructure.Auth;

public interface ISessionsUpdater
{
    Task UpdateAll(Guid companyId, int batchSize, CancellationToken cancellationToken = default);
    Task UpdateUser(AuthDbContext dbContext, Guid userId, CancellationToken cancellationToken = default);
}