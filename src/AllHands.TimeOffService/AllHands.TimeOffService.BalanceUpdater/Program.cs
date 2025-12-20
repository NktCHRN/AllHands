using AllHands.Shared.ConsumersWorker;
using AllHands.TimeOffService.Application;
using AllHands.TimeOffService.Application.Abstractions;
using AllHands.TimeOffService.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Explicitly add appsettings
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile(
        $"appsettings.{builder.Environment.EnvironmentName}.json",
        optional: true,
        reloadOnChange: true
    )
    .AddEnvironmentVariables();

builder.Configuration.AddAllHandsSystemsManager(builder.Environment, "TimeOffService");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, integrateWithWolverine: false);

var host = builder.Build();

using var cts = new CancellationTokenSource();

var timeOffService = host.Services.GetRequiredService<IBatchTimeOffBalanceUpdater>();

var result = await timeOffService.UpdateAllAsync(cts.Token);

Console.WriteLine($"Time off balances were updated successfully. Results: {result}.");
