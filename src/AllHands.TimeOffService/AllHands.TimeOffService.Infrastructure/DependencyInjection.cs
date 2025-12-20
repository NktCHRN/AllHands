using AllHands.Shared.Infrastructure.Auth;
using AllHands.Shared.Infrastructure.Data;
using AllHands.TimeOffService.Application.Projections;
using AllHands.TimeOffService.Domain.Models;
using JasperFx.Events.Projections;
using Marten.Schema.Indexing.Unique;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolverine.Marten;

namespace AllHands.TimeOffService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, bool integrateWithWolverine = true)
    {
        services.AddAuth();

        var addMartenResult = services.AddAllHandsMarten(configuration, options =>
        {
            options.Projections.Add<EmployeeTimeOffBalanceItemProjection>(ProjectionLifecycle.Inline);
            options.Projections.Add<TimeOffRequestProjection>(ProjectionLifecycle.Inline);

            options.Schema.For<Employee>();
            options.Schema.For<Company>();
            options.Schema.For<Holiday>()
                .Duplicate(x => x.Date, "date", notNull: true, configure: idx =>
                {
                    idx.TenancyScope = TenancyScope.PerTenant;
                });
            options.Schema.For<TimeOffRequest>()
                .Duplicate(x => x.StartDate, "timestamp with time zone", notNull: true, configure: idx =>
                {
                    idx.TenancyScope = TenancyScope.PerTenant;
                })
                .Duplicate(x => x.EndDate, "timestamp with time zone", notNull: true, configure: idx =>
                {
                    idx.TenancyScope = TenancyScope.PerTenant;
                })
                .Index(x => x.EmployeeId, configure: idx =>
                {
                    idx.TenancyScope = TenancyScope.PerTenant;
                });
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
            options.Schema.For<TimeOffType>()
                .Index(x => x.Order, configure: idx =>
                {
                    idx.TenancyScope = TenancyScope.PerTenant;
                });
        });

        if (integrateWithWolverine)
        {
            addMartenResult.IntegrateWithWolverine();
        }
        
        return services;
    }
}
