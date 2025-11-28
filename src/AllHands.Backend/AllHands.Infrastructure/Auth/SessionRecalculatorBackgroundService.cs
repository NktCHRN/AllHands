using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AllHands.Infrastructure.Auth;

public sealed class SessionRecalculatorBackgroundService(
    IDbContextFactory<AuthDbContext> dbContextFactory, 
    IOptions<SessionRecalculatorBackgroundServiceOptions> optionsContainer, 
    ILogger<SessionRecalculatorBackgroundService> logger,
    ISessionsUpdater sessionsUpdater) : BackgroundService
{
    private SessionRecalculatorBackgroundServiceOptions Options => optionsContainer.Value;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting SessionRecalculatorBackgroundService");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var dbContext = await dbContextFactory.CreateDbContextAsync(stoppingToken);
                
                await using var transaction = await dbContext.Database.BeginTransactionAsync(stoppingToken);

                var currentRow = await dbContext.RecalculateCompanySessionsTasks.FromSql($"""
                                         SELECT *
                                         FROM public."RecalculateCompanySessionsTasks"
                                         WHERE "FailedAttempts" < {Options.MaxFailedAttempts} AND "CompletedAt" IS NULL
                                         ORDER BY "RequestedAt" ASC
                                         LIMIT 1
                                         FOR UPDATE SKIP LOCKED
                     """).FirstOrDefaultAsync(stoppingToken);
                if (currentRow == null)
                {
                    await Task.Delay(Options.PollTimeout, stoppingToken);
                    continue;
                }

                try
                {
                    await sessionsUpdater.UpdateAll(currentRow.CompanyId, Options.BatchSize, stoppingToken);
                    
                    currentRow.CompletedAt = DateTimeOffset.UtcNow;
                    await dbContext.SaveChangesAsync(stoppingToken);
                    
                    await transaction.CommitAsync(stoppingToken);
                }
                catch (OperationCanceledException ex) when (ex.CancellationToken == stoppingToken ||
                                                            ex.CancellationToken.IsCancellationRequested)
                {
                    break;
                }
                catch
                {
                    await transaction.RollbackAsync(stoppingToken);
                    
                    currentRow.FailedAttempts++;
                    await dbContext.SaveChangesAsync(stoppingToken);

                    throw;
                }
            }
            catch (OperationCanceledException ex) when (ex.CancellationToken == stoppingToken ||
                                                        ex.CancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Unhandled exception in SessionRecalculatorBackgroundService");
            }
        }
        
        logger.LogInformation("Stopping SessionRecalculatorBackgroundService");
    }
}
