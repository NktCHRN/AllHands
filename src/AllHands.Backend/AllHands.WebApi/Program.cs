using AllHands.Application;
using AllHands.Application.Abstractions;
using AllHands.Infrastructure;
using AllHands.Infrastructure.Auth;
using AllHands.Infrastructure.Auth.Entities;
using AllHands.Infrastructure.Data;
using AllHands.WebApi;
using AllHands.WebApi.Auth;
using Marten;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddSystemsManager(opt =>
{
    opt.Path = $"/AllHands/{builder.Environment.EnvironmentName}";
});

builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddWebApi(builder.Configuration, builder.Environment);

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())      // TODO: uncomment. Temporarily available for all environments.
//{
    app.MapOpenApi();
    
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseMiddleware<RemoveCookieOnLoginMiddleware>();

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
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    await dbContext.Database.MigrateAsync();
    
    var documentStore = scope.ServiceProvider.GetRequiredService<IDocumentStore>();
    await documentStore.Storage.ApplyAllConfiguredChangesToDatabaseAsync();

    if (app.Environment.IsDevelopment())
    {
        var seeder = new DevelopmentSeeder(
            documentStore,
            dbContext,
            scope.ServiceProvider.GetRequiredService<UserManager<AllHandsIdentityUser>>(),
            scope.ServiceProvider.GetRequiredService<RoleManager<AllHandsRole>>(),
            scope.ServiceProvider.GetRequiredService<IPermissionsContainer>());
        await seeder.SeedAsync(CancellationToken.None);
    }
}
