using AllHands.Shared.Application;
using AllHands.Shared.Application.Behaviors;
using AllHands.Shared.Domain;
using AllHands.TimeOffService.Application.Abstractions;
using AllHands.TimeOffService.Application.Features.TimeOffBalances.UpdateAll;
using AllHands.TimeOffService.Application.Utilities;
using AllHands.TimeOffService.Domain.Abstractions;
using AllHands.TimeOffService.Domain.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AllHands.TimeOffService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddUserContext();
        services.AddBaseValidation();
        
        services.AddValidatorsFromAssembly(typeof(IApplicationMarker).Assembly);
        services.AddMediatR(opt =>
        {
            opt.RegisterServicesFromAssemblyContaining<IApplicationMarker>()
                .AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddSingleton<IWorkDaysCalculator, WorkDaysCalculator>();
        services.AddSingleton<ITimeOffEmojiValidator, TimeOffEmojiValidator>();

        services.AddOptions<TimeOffBalanceAutoUpdaterOptions>()
            .BindConfiguration("TimeOffBalanceAutoUpdaterOptions");
        services.AddTransient<IBatchTimeOffBalanceUpdater, BatchTimeOffBalanceUpdater>();
        
        return services.AddSingleton(TimeProvider.System);
    }
}
