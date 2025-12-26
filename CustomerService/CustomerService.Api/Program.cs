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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MiniEcommerce - CustomerServiceApi", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description =
            "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below. \r\n\r\nExample: \"Bearer 12345abcdef\"",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    c.OperationFilter<SwaggerDocumentationOperationFilter>();
});

builder.Services.AddControllers();
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();
builder.Services.AddHttpContextAccessor();

// BASEKIT DEPENDENCIES
builder.Services.AddApi();
builder.Services.AddApplication();

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