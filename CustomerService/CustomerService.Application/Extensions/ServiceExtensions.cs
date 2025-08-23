using Microsoft.Extensions.DependencyInjection;

namespace CustomerService.Application.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICustomerServiceApplication, CustomerServiceApplication>();
        return services;
    }
}