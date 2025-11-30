using AllHands.Application.Abstractions;
using AllHands.Application.Behaviors;
using AllHands.Application.Features.TimeOffBalances.UpdateInCompany;
using AllHands.Application.Utilities;
using AllHands.Application.Validation;
using AllHands.Domain.Abstractions;
using AllHands.Domain.Services;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhoneNumbers;

namespace AllHands.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(typeof(IApplicationMarker).Assembly);
        services.AddMediatR(opt =>
        {
            opt.RegisterServicesFromAssemblyContaining<IApplicationMarker>()
                .AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        services.AddSingleton(_ => PhoneNumberUtil.GetInstance());

        services.AddSingleton<IImageValidator, ImageValidator>();
        services.AddSingleton<ITimeOffEmojiValidator, TimeOffEmojiValidator>();
        
        services.AddOptions<TimeOffBalanceAutoUpdaterOptions>()
            .BindConfiguration("TimeOffBalanceAutoUpdaterOptions")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IWorkDaysCalculator, WorkDaysCalculator>();
        
        return services.AddSingleton(TimeProvider.System);
    }
}
