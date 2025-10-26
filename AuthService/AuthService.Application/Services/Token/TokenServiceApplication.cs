using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthService.Application.DTOs;
using AuthService.Shared;
using DotnetBaseKit.Components.Application.Base;
using DotnetBaseKit.Components.Shared.Notifications;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Application.Services.Token;

public class TokenServiceApplication : BaseServiceApplication, ITokenServiceApplication
{
    private readonly IConfiguration _configuration;

    public TokenServiceApplication(NotificationContext notificationContext, IConfiguration configuration) : base(
        notificationContext)
    {
        _configuration = configuration;
    }

    public TokenDto GenerateToken(Guid id, string email, string name)
    {
        var privateKeyPath = _configuration["JwtSettings:PrivateKeyPath"];
        if (string.IsNullOrEmpty(privateKeyPath))
        {
            throw new Exception("Caminho do arquivo private_key.pem nao encontrado");
        }
        var privateKeyText = File.ReadAllText(privateKeyPath);

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

    public string GenerateToken(IEnumerable<Claim> claims)
    {
        var secret = _configuration.GetSection("TokenSettings:Secret").Value;
        var expiresToken = _configuration.GetSection("TokenSettings:ExpiresToken").Value;

        if (string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(expiresToken))
        {
            _notificationContext.AddNotification("Token", "Token is not configured");
            return default;
        }

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
            ValidateLifetime = false, // Permitir tokens expirados
            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },
            CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
        };

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var jwtToken = tokenHandler.ReadJwtToken(token);

            if (!jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                _notificationContext.AddNotification("Token", "Invalid token algorithm");
                return null;
            }

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            return principal;
        }
        catch (SecurityTokenSignatureKeyNotFoundException)
        {
            _notificationContext.AddNotification("Token", "Invalid token signature");
            return null;
        }
        catch (SecurityTokenException ex)
        {
            _notificationContext.AddNotification("Token", "Invalid token");
            return null;
        }
        catch (ArgumentException ex)
        {
            _notificationContext.AddNotification("Token", "Malformed token");
            return null;
        }
    }

    public object GetJsonWebKeySet()
    {
        var publicKeyPath = _configuration["JwtSettings:PublicKeyPath"];
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