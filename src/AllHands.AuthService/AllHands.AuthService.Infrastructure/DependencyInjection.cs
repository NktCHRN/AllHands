using AllHands.AuthService.Application.Abstractions;
using AllHands.AuthService.Domain.Models;
using AllHands.AuthService.Infrastructure.Abstractions;
using AllHands.AuthService.Infrastructure.Auth;
using AllHands.AuthService.Infrastructure.Email;
using AllHands.Shared.Infrastructure.Auth;
using AllHands.Shared.Infrastructure.Data;
using Amazon.SimpleEmailV2;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AllHands.AuthService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddAuth()
            .AddIdentityServices()
            .AddDatabase(configuration)
            .AddEmailService(configuration)
            .AddRedis(configuration, "AuthService");
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
        
        services.AddScoped<IRoleService, RoleService>();
        
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
    
    private static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        
        services.AddOptions<EmailSenderOptions>()
            .BindConfiguration("EmailSenderOptions")
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddAWSService<IAmazonSimpleEmailServiceV2>();
        services.AddSingleton<IEmailSender, EmailSender>();
        
        return services;
    }
}
