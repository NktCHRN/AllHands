using AllHands.AuthService.Contracts.Protos.Grpc;
using AllHands.EmployeeService.Application.Abstractions;
using AllHands.EmployeeService.Application.Projections;
using AllHands.EmployeeService.Domain.Models;
using AllHands.EmployeeService.Infrastructure.Clients;
using AllHands.EmployeeService.Infrastructure.Files;
using AllHands.EmployeeService.Infrastructure.Messaging;
using AllHands.Shared.Infrastructure.Auth;
using AllHands.Shared.Infrastructure.Data;
using AllHands.Shared.Infrastructure.GrpcInfrastructure;
using Amazon.S3;
using JasperFx.Events.Projections;
using Marten.Schema.Indexing.Unique;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Wolverine.Marten;

namespace AllHands.EmployeeService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuth();
        
        services.AddDatabase(configuration);
        
        services.AddFileService(configuration);

        services.AddScoped<IEventService, EventService>();

        services.AddScoped<IUserClient, IdentityUserClient>();

        services.AddGrpcRetryOptions();
        
        services.AddGrpcClient<UserService.UserServiceClient>(opt => opt.Address = new Uri(configuration.GetValue<string>("AuthService:grpc")?? throw new InvalidOperationException("AuthService URL must be provided.")))
            .AddInterceptors()
            .ConfigureChannel((sp, o) => o.ServiceConfig = new Grpc.Net.Client.Configuration.ServiceConfig
            {
                MethodConfigs =
                {
                    new Grpc.Net.Client.Configuration.MethodConfig
                    {
                        Names =
                        {
                            Grpc.Net.Client.Configuration.MethodName.Default
                        },
                        RetryPolicy = GrpcRetryProvider.GetDefaultRetryPolicy(sp.GetRequiredService<IOptions<GrpcRetryOptions>>().Value)
                    }
                }
            });
        
        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAllHandsMarten(configuration, options =>
            {
                options.Projections.Add<EmployeeProjection>(ProjectionLifecycle.Inline);

                options.Schema.For<Company>();
                options.Schema.For<Role>();
                options.Schema.For<Employee>()
                    .Index(x => x.NormalizedEmail, configure: idx =>
                    {
                        idx.TenancyScope = TenancyScope.PerTenant;
                    })
                    .Index(x => x.UserId, configure: idx =>
                    {
                        idx.TenancyScope = TenancyScope.PerTenant;
                    })
                    .Index(x => x.ManagerId, configure: idx =>
                    {
                        idx.TenancyScope = TenancyScope.PerTenant;
                    })
                    .Index(x => x.GlobalUserId, configure: idx =>
                    {
                        idx.TenancyScope = TenancyScope.PerTenant;
                    })
                    .Index(x => x.RoleId, configure: idx =>
                    {
                        idx.TenancyScope = TenancyScope.PerTenant;
                    })
                    .FullTextIndex(x => x.FirstName, x => x.MiddleName!, x => x.LastName, x => x.Email);
                options.Schema.For<Position>()
                    .Index(x => x.NormalizedName, cfg =>
                    {
                        cfg.IsUnique = true;
                        cfg.Predicate = "mt_deleted = false";
                        cfg.TenancyScope = TenancyScope.PerTenant;
                    });
            })
            .IntegrateWithWolverine();
        
        return services;
    }
    
    private static IServiceCollection AddFileService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        
        services.AddOptions<S3Options>()
            .BindConfiguration("S3")
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddAWSService<IAmazonS3>();
        services.AddSingleton<IFileService, FileService>();
        
        return services;
    }
}
