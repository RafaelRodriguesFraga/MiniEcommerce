using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CustomerService.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CustomerService.Infra.Auth
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User =>
            _httpContextAccessor.HttpContext?.User;

        public string? UserId =>
            User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public string? Name =>
            User?.FindFirst(JwtRegisteredClaimNames.Name)?.Value;

        public string? Email =>
            User?.FindFirst(ClaimTypes.Email)?.Value;
    }
}