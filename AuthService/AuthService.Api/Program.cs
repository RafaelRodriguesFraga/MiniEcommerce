using AuthService.Api.Extensions;
using AuthService.Application.Services.Extensions;
using AuthService.Application.Services.Token;
using AuthService.Application.Settings;
using AuthService.Infra.Context;
using AuthService.Infra.Extensions;
using DotnetBaseKit.Components.Api;
using DotnetBaseKit.Components.Application;
using DotnetBaseKit.Components.Infra.Sql;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

// BASEKIT DEPENDENCIES
builder.Services.AddApi();
builder.Services.AddApplication();

// PROJECT DEPENDENCIES
builder.Services.AddRepositories();
builder.Services.AddDbContext<AuthContext>(configuration);
builder.Services.AddCustomAuthentication(configuration);
builder.Services.AddServices();
builder.Services.AddCustomSwagger();

var keySettings = new KeySettings
{
    PrivateKey = File.ReadAllText("private_key.pem"),
    PublicKey = File.ReadAllText("public_key.pem")
};

builder.Services.AddSingleton(keySettings);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection") ?? "authservice-cache:6379";
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.UseHttpLoggingMiddleware();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();