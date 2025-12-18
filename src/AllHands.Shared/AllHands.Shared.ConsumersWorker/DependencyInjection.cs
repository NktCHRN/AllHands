using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace AllHands.Shared.ConsumersWorker;

public static class DependencyInjection
{
    public static IConfigurationBuilder AddAllHandsSystemsManager(this IConfigurationBuilder configuration, IHostEnvironment environment, string serviceName)
    {
        return configuration.AddSystemsManager(opt =>
        {
            opt.Path = $"/AllHands/{serviceName}/{environment.EnvironmentName}";
        });
    }
}