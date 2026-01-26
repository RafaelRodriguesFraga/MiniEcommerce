using System.Security.Claims;
using AuthService.Application.DTOs;
using AuthService.Domain.Enums;

namespace AuthService.Application.Services.Token;

public interface ITokenGeneratorServiceApplication
{
    TokenDto GenerateToken(Guid id, string email, string name, UserRole role);
    TokenDto GenerateToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
}

