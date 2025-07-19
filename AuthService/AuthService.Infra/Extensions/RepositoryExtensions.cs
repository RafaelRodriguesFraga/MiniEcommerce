using AuthService.Domain.Repositories;
using AuthService.Infra.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Infra.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserWriteRepository, UserWriteRepository>();

        return services;
    }
}