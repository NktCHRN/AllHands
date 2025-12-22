using AllHands.EmployeeService.Application;
using AllHands.EmployeeService.ConsumersWorker;
using AllHands.EmployeeService.Infrastructure;
using AllHands.Shared.ConsumersWorker;
using AllHands.Shared.Contracts.Messaging;
using AllHands.Shared.Contracts.Messaging.Events.Employees;
using AllHands.Shared.Contracts.Messaging.Events.Invitations;
using AllHands.Shared.Contracts.Messaging.Events.Roles;
using AllHands.Shared.Contracts.Messaging.Events.Users;
using AllHands.Shared.Infrastructure.Messaging;
using Wolverine;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddAllHandsSystemsManager(builder.Environment, "EmployeeService");

builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration);
builder.UseAllHandsWolverine(opts =>
{
    var environmentName = builder.Environment.EnvironmentName;
    
    opts.AddPublisher<EmployeeCreatedEvent>(environmentName, Topics.Employee);
    opts.AddPublisher<EmployeeUpdatedEvent>(environmentName, Topics.Employee);
    opts.AddPublisher<EmployeeDeletedEvent>(environmentName, Topics.Employee);
    opts.AddPublisher<EmployeeStatusUpdated>(environmentName, Topics.Employee);
    opts.AddPublisher<EmployeeFiredEvent>(environmentName, Topics.Employee);
    opts.AddPublisher<EmployeeRegisteredEvent>(environmentName, Topics.Employee);
    opts.AddPublisher<EmployeeRehiredEvent>(environmentName, Topics.Employee);
    
    opts.AddListener<InvitationAcceptedEvent>(environmentName, Topics.Invitation, Services.EmployeeService);
    opts.AddListener<UserReactivatedEvent>(environmentName, Topics.User, Services.EmployeeService);
    opts.AddListener<RoleCreatedEvent>(environmentName, Topics.Role, Services.EmployeeService);
    opts.AddListener<RoleUpdatedEvent>(environmentName, Topics.Role, Services.EmployeeService);
    opts.AddListener<RoleDeletedEvent>(environmentName, Topics.Role, Services.EmployeeService);
    
    opts.AddIncomingHeadersMiddleware();
    
    opts.Policies.UseDurableOutboxOnAllSendingEndpoints();
    opts.Policies.UseDurableInboxOnAllListeners();
    
    opts.Discovery.IncludeAssembly(typeof(IConsumersWorkerMarker).Assembly)
        .IncludeAssembly(typeof(WolverineOptions).Assembly);
});

var app = builder.Build();

app.Run();
