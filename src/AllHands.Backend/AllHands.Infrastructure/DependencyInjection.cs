using AllHands.Application.Abstractions;
using AllHands.Domain.Models;
using AllHands.Domain.Projections;
using AllHands.Infrastructure.Abstractions;
using AllHands.Infrastructure.Auth;
using AllHands.Infrastructure.Auth.Entities;
using AllHands.Infrastructure.Data;
using AllHands.Infrastructure.Email;
using AllHands.Infrastructure.Files;
using Amazon.S3;
using Amazon.SimpleEmailV2;
using JasperFx.Events.Projections;
using Marten;
using Marten.Storage;
using Microsoft.AspNetCore.Authentication.Cookies;
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
            .AddIdentityServices()
            .AddSingleton<IPermissionsContainer, PermissionsContainer>()
            .AddAwsServices(configuration);
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
    
    private static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordHasher<AllHandsIdentityUser>, BCryptUserPasswordHasher>();
        
        services.AddIdentity<AllHandsIdentityUser, AllHandsRole>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;
        })
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders();

        services.AddSingleton(Microsoft.AspNetCore.Authentication.TicketSerializer.Default);
        services.AddSingleton<ITicketStore, AllHandsTicketStore>();
        
        services.AddOptions<InvitationTokenProviderOptions>()
            .BindConfiguration(nameof(InvitationTokenProviderOptions))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddOptions<PasswordResetTokenProviderOptions>()
            .BindConfiguration(nameof(PasswordResetTokenProviderOptions))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddSingleton<IPasswordResetTokenProvider, PasswordResetTokenProvider>();

        services.AddSingleton<ITicketModifier, AllHandsTicketStore>();
        
        services.AddSingleton<IUserClaimsFactory, UserClaimsFactory>();
        services.AddSingleton<ISessionsUpdater, SessionsUpdater>();
        
        services.AddScoped<IInvitationService, InvitationService>();
        services.AddScoped<IAccountService, AccountService>();
        
        services.AddOptions<SessionRecalculatorBackgroundServiceOptions>()
            .BindConfiguration(nameof(SessionRecalculatorBackgroundServiceOptions))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddHostedService<SessionRecalculatorBackgroundService>();
        
        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddPooledDbContextFactory<AuthDbContext>(options
                => options
                    .UseNpgsql(configuration.GetConnectionString("postgres")));
        
        services.AddScoped(sp => sp.GetRequiredService<IDbContextFactory<AuthDbContext>>().CreateDbContext());
        
        return services;
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
            
            options.Policies.AllDocumentsAreMultiTenanted();
            options.Events.TenancyStyle = TenancyStyle.Conjoined;

            options.Schema.For<Employee>()
                .Index(x => x.NormalizedEmail)
                .Index(x => x.UserId)
                .Index(x => x.CompanyId)
                .Index(x => x.ManagerId)
                .FullTextIndex(x => x.FirstName, x => x.MiddleName!, x => x.LastName, x => x.Email);
            options.Schema.For<Holiday>()
                .Index(x => x.CompanyId);
            options.Schema.For<Position>()
                .Index(x => new { x.CompanyId, x.NormalizedName }, cfg =>
                {
                    cfg.IsUnique = true;
                    cfg.Predicate = $"(data->>'{nameof(Position.DeletedAt)}') IS NULL";
                });
            options.Schema.For<TimeOffRequest>()
                .Duplicate(x => x.StartDate, "timestamp with time zone", notNull: true)
                .Duplicate(x => x.EndDate, "timestamp with time zone", notNull: true)
                .Index(x => new { x.EmployeeId, x.EndDate })
                .Index(x => new { x.CompanyId, x.StartDate });
            options.Schema.For<TimeOffBalance>()
                .Index(x => new { x.EmployeeId, x.TypeId })
                .Duplicate(x => x.LastAutoUpdate, "timestamp with time zone", notNull: false);
            options.Schema.For<TimeOffType>()
                .Index(x => x.CompanyId);
            options.Schema.For<NewsPost>()
                .Index(x => x.CompanyId);
        });
        services.AddSingleton<ISessionFactory, TenantSessionFactory>();
        
        return services;
    }

    private static IServiceCollection AddAwsServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        
        services.AddOptions<EmailSenderOptions>()
            .BindConfiguration("EmailSenderOptions")
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddAWSService<IAmazonSimpleEmailServiceV2>();
        services.AddSingleton<IEmailSender, EmailSender>();
        
        services.AddOptions<S3Options>()
            .BindConfiguration("S3")
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddAWSService<IAmazonS3>();
        services.AddSingleton<IFileService, FileService>();
        
        return services;
    }
}
