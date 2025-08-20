using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Application.Services.Token;
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

        var secretSection = new Mock<IConfigurationSection>();
        secretSection.Setup(x => x.Value).Returns("your_secret_key_1234567890123456");

        var expiresSection = new Mock<IConfigurationSection>();
        expiresSection.Setup(x => x.Value).Returns("1");

        _configurationMock.Setup(x => x.GetSection("TokenSettings:Secret")).Returns(secretSection.Object);
        _configurationMock.Setup(x => x.GetSection("TokenSettings:ExpiresToken")).Returns(expiresSection.Object);

        _service = new TokenServiceApplication(_notificationContext, _configurationMock.Object);

        _email = "found@example.com";
        _id = Guid.NewGuid();
        _name = "Teste";
    }

    [Fact(DisplayName = "Should generate token with id and email")]
    public void Should_Generate_Token_With_Id_And_Email()
    {

        var result = _service.GenerateTokenAsync(_id, _email, _name);

        Assert.NotNull(result);
        Assert.Equal(_id, result.Id);
        Assert.Equal(_email, result.Email);
        Assert.False(string.IsNullOrEmpty(result.Token));
        Assert.False(string.IsNullOrEmpty(result.RefreshToken));
        Assert.True(result.ExpirationDate > result.IssuedAt);
        Assert.False(_notificationContext.HasNotifications);
    }

    [Fact(DisplayName = "Should generate token from claims")]
    public void Should_Generate_Token_From_Claims()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, "user@example.com"),
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        };

        var token = _service.GenerateToken(claims);

        Assert.False(string.IsNullOrEmpty(token));
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

        var tokenDto = _service.GenerateTokenAsync(_id, _email, _name);

        var principal = _service.GetPrincipalFromExpiredToken(tokenDto.Token);

        Assert.NotNull(principal);
        Assert.True(principal.Identity!.IsAuthenticated);
    }

    [Fact(DisplayName = "Should return a notification if token is invalid")]
    public void Should_Return_Notification_If_Token_Is_Invalid()
    {
        var invalidToken = GenerateInvalidJwt();

        _configurationMock.Setup(x => x.GetSection("TokenSettings:Secret").Value)
            .Returns("valid-secret-key-12345");

        var service = new TokenServiceApplication(_notificationContext, _configurationMock.Object);

        var result = service.GetPrincipalFromExpiredToken(invalidToken);

        Assert.Null(result);
        Assert.True(_notificationContext.HasNotifications);
        Assert.Contains(_notificationContext.Notifications, n => n.Key == "Token");
    }

    [Fact(DisplayName = "Should add notification when config is missing")]
    public void Should_Add_Notification_When_Config_Is_Missing()
    {
        var emptySection = new Mock<IConfigurationSection>();
        emptySection.Setup(x => x.Value).Returns<string?>(null);

        _configurationMock.Setup(x => x.GetSection("TokenSettings:Secret")).Returns(emptySection.Object);
        _configurationMock.Setup(x => x.GetSection("TokenSettings:ExpiresToken")).Returns(emptySection.Object);

        var result = _service.GenerateTokenAsync(_id, _email, _name);

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