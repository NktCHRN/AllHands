using AllHands.TimeOffBalanceAutoUpdater.Models;
using AllHands.TimeOffBalanceAutoUpdater.Projections;
using JasperFx;
using JasperFx.Events.Projections;
using Marten;
using Marten.Exceptions;
using Marten.Schema.Indexing.Unique;
using Marten.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Polly;

namespace AllHands.TimeOffBalanceAutoUpdater;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMartenDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMarten(options =>
        {
            // Establish the connection string to your Marten database
            options.Connection(configuration.GetConnectionString("postgres")!);
            
            options.ConfigurePolly(builder =>
            {
                builder.AddRetry(new()
                {
                    ShouldHandle = new PredicateBuilder().Handle<NpgsqlException>().Handle<MartenCommandException>(),
                    MaxRetryAttempts = configuration.GetValue<int>("Marten:MaxRetryAttempts"),
                    Delay = configuration.GetValue<TimeSpan>("Marten:Delay"),
                    BackoffType = DelayBackoffType.Linear
                });
            });
            
            options.UseSystemTextJsonForSerialization();

            options.Projections.Add<EmployeeTimeOffBalanceItemProjection>(ProjectionLifecycle.Inline);

            options.Policies.AllDocumentsAreMultiTenanted();
            options.Events.TenancyStyle = TenancyStyle.Conjoined;
            
            options.Schema.For<TimeOffBalance>()
                .Index(x => new { x.EmployeeId, x.TypeId }, configure: idx =>
                {
                    idx.IsUnique = true;
                    idx.TenancyScope = TenancyScope.PerTenant;
                })
                .Index(x => x.TypeId, configure: idx =>
                {
                    idx.TenancyScope = TenancyScope.PerTenant;
                })
                .Duplicate(x => x.LastAutoUpdate, "timestamp with time zone", notNull: false);
            
            options.AutoCreateSchemaObjects = AutoCreate.None;
        });
        
        return services;
    }
}
