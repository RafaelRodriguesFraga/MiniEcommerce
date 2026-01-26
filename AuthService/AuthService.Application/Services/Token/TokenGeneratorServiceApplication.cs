using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using AuthService.Application.DTOs;
using AuthService.Application.Settings;
using AuthService.Domain.Enums;
using AuthService.Shared;
using DotnetBaseKit.Components.Application.Base;
using DotnetBaseKit.Components.Shared.Notifications;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Application.Services.Token;

public class TokenGeneratorServiceApplication : BaseServiceApplication, ITokenGeneratorServiceApplication
{
    private readonly KeySettings _keySettings;
    public TokenGeneratorServiceApplication(NotificationContext notificationContext, KeySettings keySettings) : base(notificationContext)
    {
        _keySettings = keySettings;
    }

    public TokenDto GenerateToken(Guid id, string email, string name, UserRole role)
    {
        var privateKeyText = _keySettings.PrivateKey;
        if (string.IsNullOrEmpty(privateKeyText))
        {
            _notificationContext.AddNotification("Token", "Private key not configured.");
            return default;
        }

        var rsaKey = RSA.Create();
        rsaKey.ImportFromPem(privateKeyText);

        var signingCredentials = new SigningCredentials(
            new RsaSecurityKey(rsaKey),
            SecurityAlgorithms.RsaSha256
        );

        var date = DateTime.UtcNow;

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Name, name),
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Role, role.ToString())
            }),

            Expires = date.AddHours(2),
            IssuedAt = date,
            NotBefore = date,
            Issuer = "auth-service-issuer",
            Audience = "ecommerce-api-audience",
            SigningCredentials = signingCredentials
        };

        var thumbprint = JwkHelper.GetRsaThumbprint(rsaKey);

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        ((JwtSecurityToken)securityToken).Header.Add("kid", thumbprint);

        var token = tokenHandler.WriteToken(securityToken);
        var refreshToken = GenerateRefreshToken();

        return new TokenDto
        {
            Id = id,
            Email = email,
            Name = name,
            Token = token,
            RefreshToken = refreshToken,
            Role = role.ToString(),
            ExpirationDate = tokenDescriptor.Expires.Value,
            IssuedAt = tokenDescriptor.IssuedAt.Value,
        };
    }

    public TokenDto GenerateToken(IEnumerable<Claim> claims)
    {
        var privateKeyText = _keySettings.PrivateKey;
        var rsaKey = RSA.Create();
        rsaKey.ImportFromPem(privateKeyText);

        var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsaKey), SecurityAlgorithms.RsaSha256);
        var thumbprint = JwkHelper.GetRsaThumbprint(rsaKey);

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            IssuedAt = DateTime.UtcNow,
            NotBefore = DateTime.UtcNow,
            Issuer = "auth-service-issuer",
            Audience = "ecommerce-api-audience",
            SigningCredentials = signingCredentials
        };

        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        ((JwtSecurityToken)securityToken).Header.Add("kid", thumbprint);

        var token = tokenHandler.WriteToken(securityToken);
        var refreshToken = GenerateRefreshToken();

        return new TokenDto
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpirationDate = tokenDescriptor.Expires.Value,
            IssuedAt = tokenDescriptor.IssuedAt.Value,
        };
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}