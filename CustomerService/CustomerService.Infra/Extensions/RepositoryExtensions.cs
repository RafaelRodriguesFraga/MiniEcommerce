using Microsoft.Extensions.DependencyInjection;
using CustomerService.Domain.Repositories;
using CustomerService.Infra.Repositories;

namespace CustomerService.Infra.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICustomerWriteRepository, CustomerWriteRepository>();
        services.AddScoped<ICustomerReadRepository, CustomerReadRepository>();

        return services;
    }
}