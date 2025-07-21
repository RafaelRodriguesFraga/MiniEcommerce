using System.Security.Claims;
using AuthService.Application.DTOs;
using DotnetBaseKit.Components.Application.Base;

namespace AuthService.Application.Services.Token;

public interface ITokenService : IBaseServiceApplication
{
    TokenDto GenerateTokenAsync(Guid id, string email);
    string GenerateToken(IEnumerable<Claim> claims);

    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

}