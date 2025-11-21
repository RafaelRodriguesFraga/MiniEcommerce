using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using AuthService.Application.Settings;
using DotnetBaseKit.Components.Application.Base;
using DotnetBaseKit.Components.Shared.Notifications;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Application.Services.Token;

public class TokenValidatorServiceApplication : BaseServiceApplication, ITokenValidatorServiceApplication
{
    private readonly KeySettings _keySettings;
    public TokenValidatorServiceApplication(NotificationContext notificationContext, KeySettings keySettings) : base(notificationContext)
    {
        _keySettings = keySettings;
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var publicKeyText = _keySettings.PublicKey;
        if (string.IsNullOrEmpty(publicKeyText))
        {
            _notificationContext.AddNotification("Token", "Public key not configured.");
            return null;
        }

        var rsa = RSA.Create();
        rsa.ImportFromPem(publicKeyText);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(rsa),
            ValidateLifetime = false,
        };

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.RsaSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                _notificationContext.AddNotification("Token", "Invalid token algorithm");
                return null;
            }
            return principal;
        }
        catch (Exception ex)
        {
            _notificationContext.AddNotification("Token", $"Invalid token: {ex.Message}");
            return null;
        }
    }
}