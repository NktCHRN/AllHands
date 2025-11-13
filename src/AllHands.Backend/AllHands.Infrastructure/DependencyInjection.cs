using AllHands.Domain.Projections;
using AllHands.Infrastructure.Auth;
using JasperFx.Events.Projections;
using Marten;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace AllHands.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddRedis(configuration)
            .AddDatabase(configuration)
            .AddMartenDb(configuration)
            .AddIdentityServices(configuration);
    }

    private static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("redis");
        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(redisConnectionString!));
        services.AddScoped(cfg => 
            cfg.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = "AllHands:";
        });
        return services;
    }
    
    private static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<AllHandsIdentityUser, IdentityRole<Guid>>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;

            //options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";
            //options.Tokens.PasswordResetTokenProvider = "CustomPasswordReset";
        })
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddDbContext<AuthDbContext>((sp, options)
                => options
                    .UseNpgsql(configuration.GetConnectionString("postgres")));
    }
    
    private static IServiceCollection AddMartenDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMarten(options =>
            {
                // Establish the connection string to your Marten database
                options.Connection(configuration.GetConnectionString("postgres")!);
                
                options.UseSystemTextJsonForSerialization();

                options.Projections.Add<EmployeeProjection>(ProjectionLifecycle.Inline);
                options.Projections.Add<EmployeeTimeOffBalanceItemProjection>(ProjectionLifecycle.Inline);
                options.Projections.Add<TimeOffRequestProjection>(ProjectionLifecycle.Inline);
            })
            .UseLightweightSessions();
        
        return services;
    }
}
