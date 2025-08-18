using Microsoft.Extensions.DependencyInjection;
using UserService.Domain.Repositories;
using UserService.Infra.Repositories;

namespace UserService.Infra.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserProfileWriteRepository, UserProfileWriteRepository>();
        services.AddScoped<IUserProfileReadRepository, UserProfileReadRepository>();
        
        return services;
    }
}