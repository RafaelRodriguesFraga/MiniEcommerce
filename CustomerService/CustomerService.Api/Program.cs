using System.Reflection;
using System.Text;
using DotnetBaseKit.Components.Api;
using DotnetBaseKit.Components.Application;
using DotnetBaseKit.Components.Infra.Sql;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using CustomerService.Application.Extensions;
using CustomerService.Infra.Context;
using CustomerService.Infra.Extensions;
using System.Text.Json;
using CustomerService.Api.Documentation.Configuration;
using System.Security.Cryptography;
using CustomerService.Application.Interfaces;
using CustomerService.Infra.Auth;

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
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {

        var publicKey = File.ReadAllText(builder.Configuration["JwtSettings:PublicKeyPath"])
                        ?? File.ReadAllText("public_key.pem");

        var rsa = RSA.Create();
        rsa.ImportFromPem(publicKey);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(rsa),
            ClockSkew = TimeSpan.Zero
        };
    });

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