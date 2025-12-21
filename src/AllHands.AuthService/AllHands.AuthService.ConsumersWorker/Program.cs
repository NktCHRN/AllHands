using AllHands.Auth.Contracts.Messaging;
using AllHands.AuthService.Application;
using AllHands.AuthService.ConsumersWorker;
using AllHands.AuthService.Infrastructure;
using AllHands.AuthService.SessionRecalculator;
using AllHands.Shared.ConsumersWorker;
using AllHands.Shared.Contracts.Messaging;
using AllHands.Shared.Contracts.Messaging.Events.Employees;
using AllHands.Shared.Contracts.Messaging.Events.Roles;
using AllHands.Shared.Contracts.Messaging.Events.Users;
using AllHands.Shared.Infrastructure.Messaging;
using Wolverine;
using Wolverine.AmazonSqs;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddAllHandsSystemsManager(builder.Environment, "AuthService");

builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration);
builder.UseAllHandsWolverine(opts =>
{
    var environment = builder.Environment.EnvironmentName;
    opts.AddPublisher<UserCreatedEvent>(environment, Topics.User);
    opts.AddPublisher<UserUpdatedEvent>(environment, Topics.User);
    opts.AddPublisher<UserReactivatedEvent>(environment, Topics.User);
    opts.AddPublisher<UserDeletedEvent>(environment, Topics.User);
    
    opts.AddPublisher<RoleCreatedEvent>(environment, Topics.Role);
    opts.AddPublisher<RoleUpdatedEvent>(environment, Topics.Role);
    opts.AddPublisher<RoleDeletedEvent>(environment, Topics.Role);
    
    opts.ListenToSqsQueue($"{environment.ToLower()}_{Queues.ResetPasswordRequestedEvent}")
        .ConfigureDeadLetterQueue($"{environment.ToLower()}_{Queues.ResetPasswordRequestedEvent}_errors");
    opts.PublishMessage<ResetPasswordRequestedEvent>()
        .ToSqsQueue($"{environment.ToLower()}_{Queues.ResetPasswordRequestedEvent}");
    
    opts.ListenToSqsQueue($"{environment.ToLower()}_{Queues.UserInvitedEvent}")
        .ConfigureDeadLetterQueue($"{environment.ToLower()}_{Queues.UserInvitedEvent}_errors");
    opts.PublishMessage<UserInvitedEvent>()
        .ToSqsQueue($"{environment.ToLower()}_{Queues.UserInvitedEvent}");
    
    opts.AddListener<EmployeeDeletedEvent>(environment, Topics.Employee, Services.AuthService);
    opts.AddListener<EmployeeFiredEvent>(environment, Topics.Employee, Services.AuthService);
    opts.AddListener<EmployeeRehiredEvent>(environment, Topics.Employee, Services.AuthService);
    
    opts.AddListener<RoleUpdatedEvent>(environment, Topics.Role, Services.AuthService);
    opts.AddListener<RoleDeletedEvent>(environment, Topics.Role, Services.AuthService);
    opts.AddListener<UserUpdatedEvent>(environment, Topics.User, Services.AuthService);
    opts.AddListener<UserDeletedEvent>(environment, Topics.User, Services.AuthService);
    
    opts.AddIncomingHeadersMiddleware();
    
    opts.PersistMessagesWithPostgresql(builder.Configuration.GetConnectionString("postgres") ?? throw new InvalidOperationException("postgres connection was not provided."));
    
    opts.UseEntityFrameworkCoreTransactions();
    
    opts.Policies.UseDurableOutboxOnAllSendingEndpoints();
    opts.Policies.UseDurableInboxOnAllListeners();
    
    opts.Discovery.IncludeAssembly(typeof(IConsumerWorkerMarker).Assembly)
        .IncludeAssembly(typeof(WolverineOptions).Assembly);
});

builder.Services.AddOptions<SessionRecalculatorOptions>()
    .BindConfiguration(nameof(SessionRecalculatorOptions))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var host = builder.Build();
host.Run();
