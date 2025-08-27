using CustomerService.Application.Address;
using CustomerService.Application.Services.Address;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerService.Application.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICustomerServiceApplication, CustomerServiceApplication>();
        services.AddScoped<IAddressServiceApplication, AddressServiceApplication>();

        return services;
    }
}