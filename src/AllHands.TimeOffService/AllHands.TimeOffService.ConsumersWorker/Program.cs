using AllHands.Shared.ConsumersWorker;
using AllHands.Shared.Contracts.Messaging;
using AllHands.Shared.Contracts.Messaging.Events.Companies;
using AllHands.Shared.Contracts.Messaging.Events.Employees;
using AllHands.Shared.Infrastructure.Messaging;
using AllHands.TimeOffService.Application;
using AllHands.TimeOffService.ConsumersWorker;
using AllHands.TimeOffService.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddAllHandsSystemsManager(builder.Environment, "TimeOffService");

builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration);
builder.UseAllHandsWolverine(opts =>
{
    var environmentName = builder.Environment.EnvironmentName;
    
    opts.AddListener<EmployeeCreatedEvent>(environmentName, Topics.Employee, Services.TimeOffService);
    opts.AddListener<EmployeeUpdatedEvent>(environmentName, Topics.Employee, Services.TimeOffService);
    opts.AddListener<EmployeeDeletedEvent>(environmentName, Topics.Employee, Services.TimeOffService);
    opts.AddListener<EmployeeStatusUpdated>(environmentName, Topics.Employee, Services.TimeOffService);
    
    opts.AddListener<CompanyCreatedEvent>(environmentName, Topics.Company, Services.TimeOffService);
    opts.AddListener<CompanyUpdatedEvent>(environmentName, Topics.Company, Services.TimeOffService);
    opts.AddListener<CompanyDeletedEvent>(environmentName, Topics.Company, Services.TimeOffService);
    
    opts.AddIncomingHeadersMiddleware();
    
    opts.Policies.UseDurableOutboxOnAllSendingEndpoints();
    opts.Policies.UseDurableInboxOnAllListeners();
    
    opts.Discovery.IncludeAssembly(typeof(IConsumersWorkerMarker).Assembly);
});

var app = builder.Build();

app.Run();
