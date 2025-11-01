using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthService.Application.Services.Token;
using AuthService.Application.Settings;
using DotnetBaseKit.Components.Shared.Notifications;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace AuthService.Test.UnitTests.Application.Services;

public class TokenServiceApplicationTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly NotificationContext _notificationContext;
    private readonly TokenServiceApplication _service;
    private readonly Guid _id;
    private readonly string _name;
    private readonly string _email;

    public TokenServiceApplicationTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        _notificationContext = new NotificationContext();

        using var rsa = RSA.Create(2048);
        var privateKeyPem = rsa.ExportRSAPrivateKeyPem();
        var publicKeyPem = rsa.ExportRSAPublicKeyPem();

        var keySettings = new KeySettings { PrivateKey = privateKeyPem, PublicKey = publicKeyPem };
        _service = new TokenServiceApplication(_notificationContext, keySettings);

        _email = "found@example.com";
        _id = Guid.NewGuid();
        _name = "Teste";
    }

    [Fact(DisplayName = "Should generate token with id and email")]
    public void Should_Generate_Token_With_Id_And_Email()
    {
        var result = _service.GenerateToken(_id, _email, _name);

        Assert.NotNull(result);
        Assert.Equal(_id, result.Id);
        Assert.Equal(_email, result.Email);
        Assert.False(string.IsNullOrEmpty(result.Token));
        Assert.False(string.IsNullOrEmpty(result.RefreshToken));
        Assert.True(result.ExpirationDate > result.IssuedAt);
        Assert.False(_notificationContext.HasNotifications);
    }


    [Fact(DisplayName = "Should generate a secure refresh token")]
    public void Shoud_Generate_Secure_Refresh_Token()
    {
        var refreshToken = _service.GenerateRefreshToken();

        Assert.False(string.IsNullOrEmpty(refreshToken));
        Assert.True(refreshToken.Length > 20);
    }

    [Fact(DisplayName = "Should return principal from expired token")]
    public void Should_Return_Principal_From_Expired_Token()
    {
        var tokenDto = _service.GenerateToken(_id, _email, _name);

        var principal = _service.GetPrincipalFromExpiredToken(tokenDto.Token);

        Assert.NotNull(principal);
        Assert.True(principal.Identity!.IsAuthenticated);
    }

    [Fact(DisplayName = "Should add notification when config is missing")]
    public void Should_Add_Notification_When_Config_Is_Missing()
    {
        var emptyKeySettings = new KeySettings();
        var serviceWithEmptyKeys = new TokenServiceApplication(_notificationContext, emptyKeySettings);

        var result = serviceWithEmptyKeys.GenerateToken(_id, _email, _name);

        Assert.Null(result);
        Assert.True(_notificationContext.HasNotifications);
        Assert.Contains(_notificationContext.Notifications, n => n.Key == "Token");
    }

    private static string GenerateInvalidJwt()
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var invalidKey = Encoding.ASCII.GetBytes("invalid-secret-key-12345");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.NameId, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, "test@invalid.com")
            }),
            Expires = DateTime.UtcNow.AddMinutes(5),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(invalidKey),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        token.Header["kid"] = "invalid-key-id";

        return tokenHandler.WriteToken(token);
    }
}