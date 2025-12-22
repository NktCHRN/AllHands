using AllHands.Application.Features.TimeOffBalances.UpdateInCompany;
using AllHands.Domain.Models;
using Marten;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AllHands.Infrastructure.TimeOffBalanceAutoUpdater;

public sealed class TimeOffBalanceAutoUpdaterBackgroundService(
    IOptions<TimeOffBalanceAutoUpdaterOptions> options, 
    ILogger<TimeOffBalanceAutoUpdaterBackgroundService> logger, 
    IDocumentStore documentStore,
    IMediator mediator,
    IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting TimeOffBalanceAutoUpdaterBackgroundService");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(options.Value.Timeout, stoppingToken);
            
            try
            {
                await using var session = documentStore.QuerySession();
                
                var companyIds = await session.Query<Company>()
                    .Where(x => x.AnyTenant())
                    .Select(x => x.Id)
                    .ToListAsync(stoppingToken);

                foreach (var companyId in companyIds)
                {
                    try
                    {
                        await using var scope = serviceProvider.CreateAsyncScope();
                        await mediator.Send(new UpdateTimeOffBalanceInCompanyCommand(companyId), stoppingToken);
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
        
        logger.LogInformation("Stopping TimeOffBalanceAutoUpdaterBackgroundService.");
    }
}
