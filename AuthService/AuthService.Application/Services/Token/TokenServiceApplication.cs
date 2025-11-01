using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using AuthService.Application.DTOs;
using AuthService.Application.Settings;
using AuthService.Shared;
using DotnetBaseKit.Components.Application.Base;
using DotnetBaseKit.Components.Shared.Notifications;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Application.Services.Token;

public class TokenServiceApplication : BaseServiceApplication, ITokenServiceApplication
{
    private readonly KeySettings _keySettings;

    public TokenServiceApplication(NotificationContext notificationContext, KeySettings keySettings) : base(
        notificationContext)
    {
        _keySettings = keySettings;
    }

    public TokenDto GenerateToken(Guid id, string email, string name)
    {
        var privateKeyText = _keySettings.PrivateKey;
        if (string.IsNullOrEmpty(privateKeyText))
        {
            _notificationContext.AddNotification("Token", "Chave privada não configurada.");
            return null;
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
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
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

        var tokenDto = new TokenDto
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

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var publicKeyText = _keySettings.PublicKey;
        if (string.IsNullOrEmpty(publicKeyText))
        {
            _notificationContext.AddNotification("Token", "Chave pública não configurada.");
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

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }

    public object GetJsonWebKeySet()
    {
        var publicKeyPath = _keySettings.PublicKey;
        if (string.IsNullOrEmpty(publicKeyPath))
        {
            throw new Exception("Caminho do arquivo public_key.pem nao encontrado");
        }

        var publicKeyText = File.ReadAllText(publicKeyPath);
        var rsa = RSA.Create();
        rsa.ImportFromPem(publicKeyText);

        var rsaParameters = rsa.ExportParameters(false);
        var thumbprint = JwkHelper.GetRsaThumbprint(rsa);

        var jwk = new JsonWebKey
        {
            Kty = "RSA",
            Use = "sig",
            Kid = thumbprint,
            E = Base64UrlEncoder.Encode(rsaParameters.Exponent),
            N = Base64UrlEncoder.Encode(rsaParameters.Modulus)
        };

        return new { keys = new[] { jwk } };
    }
}