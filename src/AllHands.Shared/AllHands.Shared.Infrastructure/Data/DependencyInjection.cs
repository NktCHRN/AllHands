using Marten;
using Marten.Exceptions;
using Marten.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Polly;
using StackExchange.Redis;

namespace AllHands.Shared.Infrastructure.Data;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public MartenServiceCollectionExtensions.MartenConfigurationExpression AddAllHandsMarten(IConfiguration configuration,
            Action<StoreOptions> configure
        )
        {
            services.AddSingleton<ISessionFactory, TenantSessionFactory>();
            return services.AddMarten(options =>
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
            
                options.Policies.AllDocumentsAreMultiTenanted();
                options.Events.TenancyStyle = TenancyStyle.Conjoined;
            
                configure(options);
            });
        }

        public IServiceCollection AddRedis(IConfiguration configuration, string serviceName)
        {
            var redisConnectionString = configuration.GetConnectionString("redis");
            services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(redisConnectionString!));
            services.AddScoped(cfg => 
                cfg.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = $"{serviceName}:";
            });
            return services;
        }
    }
}
