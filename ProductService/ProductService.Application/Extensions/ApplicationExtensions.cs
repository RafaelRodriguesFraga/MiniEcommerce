using Microsoft.Extensions.DependencyInjection;
using ProductService.Application.Services;

namespace ProductService.Application.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IProductServiceApplication, ProductServiceApplication>();

            return services;
        }
    }
}