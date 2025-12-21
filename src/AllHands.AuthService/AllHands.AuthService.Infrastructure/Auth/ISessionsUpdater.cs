namespace AllHands.AuthService.Infrastructure.Auth;

public interface ISessionsUpdater
{
    Task UpdateAll(Guid companyId, int batchSize, CancellationToken cancellationToken = default);
    Task UpdateUser(Guid userId, CancellationToken cancellationToken = default);
    Task ExpireUser(Guid userId, CancellationToken cancellationToken = default);
}