using System.Security.Claims;

namespace AuthService.Application.Services.Token;

public interface ITokenValidatorServiceApplication
{
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}