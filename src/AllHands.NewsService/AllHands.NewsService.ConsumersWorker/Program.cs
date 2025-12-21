using AllHands.NewsService.Application;
using AllHands.NewsService.ConsumersWorker;
using AllHands.NewsService.Infrastructure;
using AllHands.Shared.ConsumersWorker;
using AllHands.Shared.Contracts.Messaging;
using AllHands.Shared.Contracts.Messaging.Events.Employees;
using AllHands.Shared.Infrastructure.Messaging;
using Wolverine;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddAllHandsSystemsManager(builder.Environment, "NewsService");

builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration);
builder.UseAllHandsWolverine(opts =>
{
    var environmentName = builder.Environment.EnvironmentName;
    
    opts.AddListener<EmployeeCreatedEvent>(environmentName, Topics.Employee, Services.NewsService);
    opts.AddListener<EmployeeUpdatedEvent>(environmentName, Topics.Employee, Services.NewsService);
    opts.AddListener<EmployeeDeletedEvent>(environmentName, Topics.Employee, Services.NewsService);
    
    opts.AddIncomingHeadersMiddleware();
    
    opts.Policies.UseDurableOutboxOnAllSendingEndpoints();
    opts.Policies.UseDurableInboxOnAllListeners();
    
    opts.Discovery.IncludeAssembly(typeof(IConsumersWorkerMarker).Assembly)
        .IncludeAssembly(typeof(WolverineOptions).Assembly);
});

var app = builder.Build();

app.Run();
