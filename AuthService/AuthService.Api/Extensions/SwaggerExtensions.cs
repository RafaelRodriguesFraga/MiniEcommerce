using System.Reflection;
using Commons.Swagger.Configuration;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AuthService.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "MiniEcommerce - AuthServiceApi", Version = "v1" });
    
            c.OperationFilter<SwaggerDocumentationOperationFilter>();

        });

        return services;
    }
}