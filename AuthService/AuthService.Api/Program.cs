using System.Text;
using AuthService.Api.Extensions;
using AuthService.Application.Services;
using AuthService.Application.Services.Auth;
using AuthService.Application.Services.Token;
using AuthService.Application.Services.User;
using AuthService.Infra.Context;
using AuthService.Infra.Extensions;
using DotnetBaseKit.Components.Api;
using DotnetBaseKit.Components.Application;
using DotnetBaseKit.Components.Infra.Sql;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddScoped<IUserServiceApplication, UserServiceApplication>();
builder.Services.AddScoped<ITokenServiceApplication, TokenServiceApplication>();
builder.Services.AddScoped<IAuthServiceApplication, AuthServiceApplication>();

builder.Services.AddRepositories();
builder.Services.AddCustomSwagger();

// BASEKIT DEPENDENCIES
builder.Services.AddApi();
builder.Services.AddApplication();
builder.Services.AddRepositories();
builder.Services.AddDbContext<AuthContext>(configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpLoggingMiddleware();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();