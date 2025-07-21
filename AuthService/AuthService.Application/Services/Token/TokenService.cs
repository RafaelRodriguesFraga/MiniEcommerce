using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthService.Application.DTOs;
using DotnetBaseKit.Components.Application.Base;
using DotnetBaseKit.Components.Shared.Notifications;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Application.Services.Token;

public class TokenService : BaseServiceApplication, ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(NotificationContext notificationContext, IConfiguration configuration) : base(
        notificationContext)
    {
        _configuration = configuration;
    }

    public TokenDto GenerateTokenAsync(Guid id, string email)
    {
        var secret = _configuration.GetSection("TokenSettings:Secret").Value;
        var expiresToken = _configuration.GetSection("TokenSettings:ExpiresToken").Value;

        VerifyToken(secret, expiresToken);

        var date = DateTime.Now;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            NotBefore = date,
            IssuedAt = date,
            Expires = date.AddHours(Convert.ToInt32(expiresToken) | 2),
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            }),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var createToken = tokenHandler.CreateToken(tokenDescriptor);

        var token = tokenHandler.WriteToken(createToken);
        var refreshToken = GenerateRefreshToken();
        // SaveRefreshToken(userViewModel.Email, refreshToken);

        TokenDto tokenDto = new TokenDto
        {
            Id = id,
            Email = email,
            Token = token,
            RefreshToken = refreshToken,
            ExpirationDate = tokenDescriptor.Expires.Value,
            IssuedAt = tokenDescriptor.IssuedAt.Value,
        };

        return tokenDto;
    }

    public string GenerateToken(IEnumerable<Claim> claims)
    {
        var secret = _configuration.GetSection("TokenSettings:Secret").Value;
        var expiresToken = _configuration.GetSection("TokenSettings:ExpiresToken").Value;

        VerifyToken(secret, expiresToken);

        var date = DateTime.Now;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            NotBefore = date,
            IssuedAt = date,
            Expires = date.AddHours(Convert.ToInt32(expiresToken) | 2),
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var createToken = tokenHandler.CreateToken(tokenDescriptor);
        var token = tokenHandler.WriteToken(createToken);

        return token;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var secret = _configuration.GetSection("TokenSettings:Secret").Value;
        var key = Encoding.ASCII.GetBytes(secret);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            _notificationContext.AddNotification("Token", "Invalid token");
            return default;
        }


        return principal;
    }

    private void VerifyToken(string secret, string expiresToken)
    {
        if (string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(expiresToken))
        {
            _notificationContext.AddNotification("Token", "Token is not configured");
        }
    }
}