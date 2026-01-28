using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using System.Security.Cryptography;

namespace ProductService.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var publicKeyPath = configuration["JwtSettings:PublicKeyPath"];
        if (string.IsNullOrEmpty(publicKeyPath) || !File.Exists(publicKeyPath))
        {
            throw new Exception("Caminho da chave pública não encontrado ou inválido na configuração.");
        }

        var publicKeyText = File.ReadAllText(publicKeyPath);
        var rsa = RSA.Create();
        rsa.ImportFromPem(publicKeyText);
        var rsaSecurityKey = new RsaSecurityKey(rsa);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = rsaSecurityKey,
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["JwtSettings:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidAlgorithms = new[] { SecurityAlgorithms.RsaSha256 }
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("--- AUTHENTICATION FAILED ---");
                        Console.WriteLine(context.Exception.ToString());
                        Console.ResetColor();
                        return Task.CompletedTask;
                    }
                };
            });


        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
             {
                 policy.RequireAuthenticatedUser();
                 policy.RequireRole("Admin");
             });
        });

        return services;
    }
}