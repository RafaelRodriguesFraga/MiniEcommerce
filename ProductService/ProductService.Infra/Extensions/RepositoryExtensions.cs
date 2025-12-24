using Microsoft.Extensions.DependencyInjection;
using ProductService.Domain.Repositories;
using ProductService.Infra.Repositories;

namespace ProductService.Infra.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProductReadRepository, ProductReadRepository>();
        services.AddScoped<IProductWriteRepository, ProductWriteRepository>();
        return services;
    }
}