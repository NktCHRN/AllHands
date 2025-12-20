using AllHands.AuthService.SessionRecalculator;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOptions<SessionRecalculatorOptions>()
    .BindConfiguration(nameof(SessionRecalculatorOptions))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var host = builder.Build();
host.Run();
