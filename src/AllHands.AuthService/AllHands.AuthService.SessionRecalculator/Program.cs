using AllHands.Auth.Contracts.Messaging;
using AllHands.AuthService.Application;
using AllHands.AuthService.Infrastructure;
using AllHands.AuthService.SessionRecalculator;
using AllHands.Shared.ConsumersWorker;
using AllHands.Shared.Infrastructure.Messaging;
using Wolverine.AmazonSqs;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddAllHandsSystemsManager(builder.Environment, "AuthService");

builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration);
builder.UseAllHandsWolverine(opts =>
{
    var environment = builder.Environment.EnvironmentName;
    
    opts.ListenToSqsQueue($"{environment.ToLower()}_{Queues.CompanySessionsRecalculationRequestedEvent}")
        .ConfigureDeadLetterQueue($"{environment.ToLower()}_{Queues.CompanySessionsRecalculationRequestedEvent}_errors");
    opts.PublishMessage<CompanySessionsRecalculationRequestedEvent>()
        .ToSqsQueue($"{environment.ToLower()}_{Queues.CompanySessionsRecalculationRequestedEvent}");
    
    opts.ListenToSqsQueue($"{environment.ToLower()}_{Queues.UserSessionsRecalculationRequestedEvent}")
        .ConfigureDeadLetterQueue($"{environment.ToLower()}_{Queues.UserSessionsRecalculationRequestedEvent}_errors");
    opts.PublishMessage<UserSessionsRecalculationRequestedEvent>()
        .ToSqsQueue($"{environment.ToLower()}_{Queues.UserSessionsRecalculationRequestedEvent}");
    
    opts.AddIncomingHeadersMiddleware();
    
    opts.Policies.UseDurableOutboxOnAllSendingEndpoints();
    opts.Policies.UseDurableInboxOnAllListeners();
    
    opts.Discovery.IncludeAssembly(typeof(ISessionRecalculatorMarker).Assembly);
});

builder.Services.AddOptions<SessionRecalculatorOptions>()
    .BindConfiguration(nameof(SessionRecalculatorOptions))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var host = builder.Build();
host.Run();
