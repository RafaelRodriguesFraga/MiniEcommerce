
using DotnetBaseKit.Components.Api;
using DotnetBaseKit.Components.Application;
using DotnetBaseKit.Components.Infra.Sql;
using Microsoft.IdentityModel.Tokens;
using CustomerService.Application.Extensions;
using CustomerService.Infra.Context;
using CustomerService.Infra.Extensions;
using System.Security.Cryptography;
using CustomerService.Application.Interfaces;
using CustomerService.Infra.Auth;
using CustomerService.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();
builder.Services.AddHttpContextAccessor();

// BASEKIT DEPENDENCIES
builder.Services.AddApi();
builder.Services.AddApplication();
builder.Services.AddCustomSwagger();

// PROJECT DEPENDENCIES
builder.Services.AddDbContext<CustomerContext>(configuration);
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddRsaJwtAuthentication(configuration);

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();