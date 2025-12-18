using AllHands.NewsService.Application;
using AllHands.NewsService.Infrastructure;
using AllHands.NewsService.WebApi;
using AllHands.Shared.WebApi.Rest;
using Marten;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAllHandsSystemsManager(builder.Environment, "NewsService");

builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddWebApi(builder.Configuration, builder.Environment);

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

app.Run();
return;

async Task MigrateAsync()
{
    await using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateAsyncScope();

    var documentStore = scope.ServiceProvider.GetRequiredService<IDocumentStore>();
    await documentStore.Storage.ApplyAllConfiguredChangesToDatabaseAsync();
}
