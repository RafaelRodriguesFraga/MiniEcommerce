using DotnetBaseKit.Components.Api;
using DotnetBaseKit.Components.Application;
using DotnetBaseKit.Components.Infra.Sql;
using Microsoft.IdentityModel.Tokens;
using UserService.Application.Extensions;
using UserService.Infra.Context;
using UserService.Infra.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();

// BASEKIT DEPENDENCIES
builder.Services.AddApi();
builder.Services.AddApplication();

builder.Services.AddDbContext<UserContext>(configuration);

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.RequireHttpsMetadata = false;
        options.Authority = "http://localhost:5194"; // URL do AuthService
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = true,
            ValidIssuer = "MiniEcommerce.AuthService",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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