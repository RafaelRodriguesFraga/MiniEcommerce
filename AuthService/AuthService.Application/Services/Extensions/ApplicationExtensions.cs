using AuthService.Application.Services.Auth;
using AuthService.Application.Services.Token;
using AuthService.Application.Services.Token.Facade;
using AuthService.Application.Services.User;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Application.Services.Extensions;

public static class ApplicationExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserServiceApplication, UserServiceApplication>();
        services.AddScoped<IAuthServiceApplication, AuthServiceApplication>();

        services.AddScoped<ITokenGeneratorServiceApplication, TokenGeneratorServiceApplication>();
        services.AddScoped<ITokenValidatorServiceApplication, TokenValidatorServiceApplication>();
        services.AddScoped<IJwkServiceApplication, JwkServiceApplication>();

        services.AddScoped<ITokenFacade, TokenFacade>();
    }
}