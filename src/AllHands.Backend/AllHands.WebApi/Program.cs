using AllHands.Application;
using AllHands.Infrastructure;
using AllHands.WebApi;
using Amazon;
using Amazon.Extensions.NETCore.Setup;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddSystemsManager(opt =>
{
    opt.Path = $"/AllHands/{builder.Environment.EnvironmentName}";
    
    opt.AwsOptions = new AWSOptions
    {
        Region = RegionEndpoint.EUCentral1
    };
});

builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddWebApi(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
