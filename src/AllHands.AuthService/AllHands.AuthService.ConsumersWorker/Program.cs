using AllHands.Auth.Contracts.Messaging;
using AllHands.AuthService.Application;
using AllHands.AuthService.ConsumersWorker;
using AllHands.AuthService.Infrastructure;
using AllHands.Shared.ConsumersWorker;
using AllHands.Shared.Contracts.Messaging;
using AllHands.Shared.Contracts.Messaging.Events.Employees;
using AllHands.Shared.Contracts.Messaging.Events.Users;
using AllHands.Shared.Infrastructure.Messaging;
using Wolverine.AmazonSqs;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddAllHandsSystemsManager(builder.Environment, "AuthService");

builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration);
builder.UseAllHandsWolverine(opts =>
{
    var environment = builder.Environment.EnvironmentName;
    
    opts.ListenToSqsQueue($"{environment.ToLower()}_{Queues.ResetPasswordRequestedEvent}")
        .ConfigureDeadLetterQueue($"{environment.ToLower()}_{Queues.ResetPasswordRequestedEvent}_errors");
    opts.PublishMessage<ResetPasswordRequestedEvent>()
        .ToSqsQueue($"{environment.ToLower()}_{Queues.ResetPasswordRequestedEvent}");
    
    opts.ListenToSqsQueue($"{environment.ToLower()}_{Queues.UserInvitedEvent}")
        .ConfigureDeadLetterQueue($"{environment.ToLower()}_{Queues.UserInvitedEvent}_errors");
    opts.PublishMessage<UserInvitedEvent>()
        .ToSqsQueue($"{environment.ToLower()}_{Queues.UserInvitedEvent}");
    
    opts.AddListener<EmployeeUpdatedEvent>(environment, Topics.Employee, Services.AuthService);
    opts.AddListener<EmployeeDeletedEvent>(environment, Topics.Employee, Services.AuthService);
    opts.AddListener<EmployeeFiredEvent>(environment, Topics.Employee, Services.AuthService);
    opts.AddListener<EmployeeRehiredEvent>(environment, Topics.Employee, Services.AuthService);
    
    opts.AddIncomingHeadersMiddleware();
    
    opts.Policies.UseDurableOutboxOnAllSendingEndpoints();
    opts.Policies.UseDurableInboxOnAllListeners();
    
    opts.Discovery.IncludeAssembly(typeof(IConsumerWorkerMarker).Assembly);
});

var host = builder.Build();
host.Run();
