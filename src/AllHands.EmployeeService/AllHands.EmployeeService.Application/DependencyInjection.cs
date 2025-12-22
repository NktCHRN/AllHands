using AllHands.EmployeeService.Application.Features.Roles;
using AllHands.Shared.Application;
using AllHands.Shared.Application.Behaviors;
using AllHands.Shared.Domain;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PhoneNumbers;

namespace AllHands.EmployeeService.Application;

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
        services.AddSingleton(_ => PhoneNumberUtil.GetInstance());

        services.AddOptions<UserRoleUpdaterOptions>()
            .BindConfiguration(nameof(UserRoleUpdaterOptions));
        
        return services.AddSingleton(TimeProvider.System);
    }
}
