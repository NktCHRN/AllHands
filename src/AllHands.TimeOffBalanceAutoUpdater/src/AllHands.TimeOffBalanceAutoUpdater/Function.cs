using System.Text.Json;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AllHands.TimeOffBalanceAutoUpdater;

public class Function
{
    private readonly TimeSpan _cancellationGraceTimeout = TimeSpan.FromSeconds(3);
    
    /// <summary>
    /// A simple function that takes a string and returns both the upper and lower case version of the string.
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<TimeOffBalancesUpdateResult> FunctionHandler(Input input, ILambdaContext context)
    {
        using var cts = new CancellationTokenSource(
            context.RemainingTime - _cancellationGraceTimeout
        );
        
        var configurationBuilder = new ConfigurationBuilder().AddSystemsManager(opt =>
        {
            opt.Path = $"/AllHands/TimeOffBalanceUpdater/{input.EnvironmentName}";
        });
        var configuration = configurationBuilder.Build();
        
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        
        services.AddOptions<TimeOffBalanceAutoUpdaterOptions>()
            .BindConfiguration("TimeOffBalanceAutoUpdaterOptions")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddMartenDb(configuration);

        services.AddSingleton<TimeOffService>();
        
        var serviceProvider = services.BuildServiceProvider();
        var timeOffService = serviceProvider.GetRequiredService<TimeOffService>();

        var result = await timeOffService.UpdateAllAsync(cts.Token);
        
        context.Logger.LogInformation($"Returning: {JsonSerializer.Serialize(result)}");
        
        return result;
    }
}

public sealed record Input
{
    public string EnvironmentName { get; set; } = "Production";
}
