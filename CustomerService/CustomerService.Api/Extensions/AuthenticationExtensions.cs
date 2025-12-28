using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace CustomerService.Api.Extensions
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddRsaJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var keyPath = configuration["JwtSettings:PublicKeyPath"] ?? "public_key.pem";

            if (!File.Exists(keyPath))
            {
                throw new FileNotFoundException($"Public key file not found in: {keyPath}");
            }

            var publicKey = File.ReadAllText(keyPath);
            var rsa = RSA.Create();

            rsa.ImportFromPem(publicKey);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
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

            return services;
        }
    }
}