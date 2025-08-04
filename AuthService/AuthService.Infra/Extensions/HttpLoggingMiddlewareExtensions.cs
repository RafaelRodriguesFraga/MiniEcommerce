using AuthService.Infra.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace AuthService.Infra.Extensions
{
    public static class HttpLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseHttpLoggingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HttpLoggingMiddleware>();
        }
    }
}