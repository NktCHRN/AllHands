namespace AllHands.AuthService.Infrastructure.Auth;

public interface ISessionsUpdater
{
    Task UpdateUser(Guid userId, CancellationToken cancellationToken = default);
    Task ExpireUser(Guid userId, CancellationToken cancellationToken = default);
    Task UpdateInRole(Guid roleId, int batchSize, CancellationToken cancellationToken = default);
}