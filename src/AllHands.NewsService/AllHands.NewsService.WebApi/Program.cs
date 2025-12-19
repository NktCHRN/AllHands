using AllHands.NewsService.Application;
using AllHands.NewsService.Infrastructure;
using AllHands.NewsService.WebApi;
using AllHands.Shared.Infrastructure.Messaging;
using AllHands.Shared.WebApi.Rest;
using Marten;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAllHandsSystemsManager(builder.Environment, "NewsService");

builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddWebApi(builder.Configuration, builder.Environment);

builder.UseAllHandsWolverine(opts =>
{
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
