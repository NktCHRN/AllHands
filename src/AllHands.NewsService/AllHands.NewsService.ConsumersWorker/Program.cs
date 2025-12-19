using AllHands.Shared.ConsumersWorker;
using AllHands.Shared.Contracts.Messaging;
using AllHands.Shared.Contracts.Messaging.Events.Employees;
using AllHands.Shared.Infrastructure.Messaging;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddAllHandsSystemsManager(builder.Environment, "NewsService");

builder.UseAllHandsWolverine(opts =>
{
    var environmentName = builder.Environment.EnvironmentName;
    
    opts.AddListener<EmployeeCreatedEvent>(environmentName, Topics.Employee, Services.NewsService);
    opts.AddListener<EmployeeUpdatedEvent>(environmentName, Topics.Employee, Services.NewsService);
    opts.AddListener<EmployeeDeletedEvent>(environmentName, Topics.Employee, Services.NewsService);
    
    opts.AddIncomingHeadersMiddleware();
    
    opts.Policies.UseDurableInboxOnAllListeners();
});

var app = builder.Build();

app.Run();
