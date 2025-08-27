using CustomerService.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using CustomerService.Infra.Repositories;
using CustomerService.Infra.Repositories.Address;

namespace CustomerService.Infra.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICustomerWriteRepository, CustomerWriteRepository>();
        services.AddScoped<ICustomerReadRepository, CustomerReadRepository>();
        services.AddScoped<IAddressWriteRepository, AddressWriteRepository>();
        services.AddScoped<IAddressReadRepository, AddressReadRepository>();

        return services;
    }
}