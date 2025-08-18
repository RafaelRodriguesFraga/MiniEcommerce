using Microsoft.Extensions.DependencyInjection;

namespace UserService.Application.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserProfileServiceApplication, UserProfileServiceApplication>();
        return services;
    }
}