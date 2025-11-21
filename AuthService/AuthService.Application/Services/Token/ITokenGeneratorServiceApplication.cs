using System.Security.Claims;
using AuthService.Application.DTOs;

namespace AuthService.Application.Services.Token;

public interface ITokenGeneratorServiceApplication
{
    TokenDto GenerateToken(Guid id, string email, string name);
    TokenDto GenerateToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
}

