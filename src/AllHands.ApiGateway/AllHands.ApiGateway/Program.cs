using AllHands.ApiGateway;
using AllHands.Shared.WebApi.Rest;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAllHandsVersioning();
builder.Services.AddHttpClient<AuthClient>(opt => opt.BaseAddress = new Uri(builder.Configuration.GetValue<string>("AuthService:http") 
                                                                            ?? throw new InvalidOperationException("AuthService http address is not provided.")));
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseMiddleware<UserContextHeadersSetterMiddleware>();

app.MapReverseProxy();

app.Run();
