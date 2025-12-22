using AllHands.EmployeeService.Application;
using AllHands.EmployeeService.Infrastructure;
using AllHands.EmployeeService.WebApi;
using AllHands.Shared.Contracts.Messaging;
using AllHands.Shared.Contracts.Messaging.Events.Employees;
using AllHands.Shared.Infrastructure.Messaging;
using AllHands.Shared.WebApi.Rest;
using Marten;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAllHandsSystemsManager(builder.Environment, "EmployeeService");

builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddWebApi(builder.Configuration, builder.Environment);

builder.UseAllHandsWolverine(opts =>
{
    var environment = builder.Environment.EnvironmentName;
    
    opts.AddPublisher<EmployeeCreatedEvent>(environment, Topics.Employee);
    opts.AddPublisher<EmployeeUpdatedEvent>(environment, Topics.Employee);
    opts.AddPublisher<EmployeeDeletedEvent>(environment, Topics.Employee);
    opts.AddPublisher<EmployeeStatusUpdated>(environment, Topics.Employee);
    opts.AddPublisher<EmployeeFiredEvent>(environment, Topics.Employee);
    opts.AddPublisher<EmployeeRegisteredEvent>(environment, Topics.Employee);
    opts.AddPublisher<EmployeeRehiredEvent>(environment, Topics.Employee);
    
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

    var documentStore = scope.ServiceProvider.GetRequiredService<IDocumentStore>();
    await documentStore.Storage.ApplyAllConfiguredChangesToDatabaseAsync();
}