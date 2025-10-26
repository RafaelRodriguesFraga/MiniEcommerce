using System.Security.Claims;
using AuthService.Application.DTOs;
using DotnetBaseKit.Components.Application.Base;

namespace AuthService.Application.Services.Token;

public interface ITokenServiceApplication : IBaseServiceApplication
{
    TokenDto GenerateToken(Guid id, string email, string name);
    string GenerateToken(IEnumerable<Claim> claims);

    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

    object GetJsonWebKeySet();

}