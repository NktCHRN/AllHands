using AllHands.Auth.Contracts.Messaging;
using AllHands.AuthService.Application;
using AllHands.AuthService.Infrastructure;
using AllHands.AuthService.Infrastructure.Auth;
using AllHands.AuthService.WebApi;
using AllHands.AuthService.WebApi.GrpcServices;
using AllHands.Shared.Contracts.Messaging;
using AllHands.Shared.Contracts.Messaging.Events.Roles;
using AllHands.Shared.Contracts.Messaging.Events.Users;
using AllHands.Shared.Infrastructure.Messaging;
using AllHands.Shared.WebApi.Rest;
using Microsoft.EntityFrameworkCore;
using Wolverine.AmazonSqs;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAllHandsSystemsManager(builder.Environment, "AuthService");

builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddWebApi(builder.Configuration, builder.Environment);

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
    
    opts.PublishMessage<ResetPasswordRequestedEvent>()
        .ToSqsQueue($"{environment.ToLower()}_{Queues.ResetPasswordRequestedEvent}");
    opts.PublishMessage<UserInvitedEvent>()
        .ToSqsQueue($"{environment.ToLower()}_{Queues.UserInvitedEvent}");
    
    opts.PersistMessagesWithPostgresql(builder.Configuration.GetConnectionString("postgres") ?? throw new InvalidOperationException("postgres connection was not provided."));
    
    opts.UseEntityFrameworkCoreTransactions();
    opts.Policies.UseDurableOutboxOnAllSendingEndpoints();
});

var app = builder.Build();

app.UseCors("CORS");

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())      // TODO: uncomment. Temporarily available for all environments.
//{
app.MapOpenApi();
    
app.UseSwagger();
app.UseSwaggerUI();
//}

app.MapHealthChecks("/health");

app.UseExceptionHandlingMiddleware();

app.UseUserContextHeadersMiddleware();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<UserGrpcService>();

var runMigrations = app.Configuration.GetValue<bool>("RUN_MIGRATIONS");
var runMigrationsAndExit =  app.Configuration.GetValue<bool>("RUN_MIGRATIONS_AND_EXIT");
if (runMigrations || runMigrationsAndExit)
{
    await MigrateAsync();
}

if (runMigrationsAndExit)
{
    return;
}

app.Run();
return;

async Task MigrateAsync()
{
    await using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateAsyncScope();

    var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    await dbContext.Database.MigrateAsync();
}
