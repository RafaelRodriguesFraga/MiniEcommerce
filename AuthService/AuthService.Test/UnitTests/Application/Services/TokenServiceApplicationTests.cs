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
    }

    [Fact(DisplayName = "Should generate token with id and email")]
    public void GenerateTokenAsync_WithValidInputs_ShouldReturnTokenDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var email = "user@example.com";

        // Act
        var result = _service.GenerateTokenAsync(id, email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(email, result.Email);
        Assert.False(string.IsNullOrEmpty(result.Token));
        Assert.False(string.IsNullOrEmpty(result.RefreshToken));
        Assert.True(result.ExpirationDate > result.IssuedAt);
        Assert.False(_notificationContext.HasNotifications);
    }

    [Fact(DisplayName = "Should generate token from claims")]
    public void GenerateToken_WithClaims_ShouldReturnToken()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, "user@example.com"),
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        };

        // Act
        var token = _service.GenerateToken(claims);

        // Assert
        Assert.False(string.IsNullOrEmpty(token));
    }

    [Fact(DisplayName = "Should generate a secure refresh token")]
    public void GenerateRefreshToken_ShouldReturnSecureString()
    {
        // Act
        var refreshToken = _service.GenerateRefreshToken();

        // Assert
        Assert.False(string.IsNullOrEmpty(refreshToken));
        Assert.True(refreshToken.Length > 20); // base64 of 32 bytes ≈ 44 chars
    }

    [Fact(DisplayName = "Should return principal from expired token")]
    public void GetPrincipalFromExpiredToken_WithValidToken_ShouldReturnClaimsPrincipal()
    {
        // Arrange
        var id = Guid.NewGuid();
        var email = "user@example.com";
        var tokenDto = _service.GenerateTokenAsync(id, email);

        // Act
        var principal = _service.GetPrincipalFromExpiredToken(tokenDto.Token);

        // Assert
        Assert.NotNull(principal);
        Assert.True(principal.Identity!.IsAuthenticated);
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_InvalidToken_ShouldNotify()
    {
        // Arrange
        var invalidToken = GenerateInvalidJwt();

        _configurationMock.Setup(x => x.GetSection("TokenSettings:Secret").Value)
            .Returns("valid-secret-key-12345"); // Diferente da chave usada para gerar o token

        var service = new TokenServiceApplication(_notificationContext, _configurationMock.Object);

        // Act
        var result = service.GetPrincipalFromExpiredToken(invalidToken);

        // Assert
        Assert.Null(result);
        Assert.True(_notificationContext.HasNotifications);
        Assert.Contains(_notificationContext.Notifications, n => n.Key == "Token");
    }

    [Fact(DisplayName = "Should add notification when config is missing")]
    public void GenerateTokenAsync_MissingConfig_ShouldNotify()
    {
        var emptySection = new Mock<IConfigurationSection>();
        emptySection.Setup(x => x.Value).Returns<string?>(null);

        _configurationMock.Setup(x => x.GetSection("TokenSettings:Secret")).Returns(emptySection.Object);
        _configurationMock.Setup(x => x.GetSection("TokenSettings:ExpiresToken")).Returns(emptySection.Object); 
        
        var result = _service.GenerateTokenAsync(Guid.NewGuid(), "email@test.com");
        
        Assert.Null(result);
        Assert.True(_notificationContext.HasNotifications);
        Assert.Contains(_notificationContext.Notifications, n => n.Key == "Token");
    }
    private static string GenerateInvalidJwt()
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var invalidKey = Encoding.ASCII.GetBytes("invalid-secret-key-12345");
        var validKey = Encoding.ASCII.GetBytes("valid-secret-key-12345");

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

        // Força um kid específico
        var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        token.Header["kid"] = "invalid-key-id";
    
        return tokenHandler.WriteToken(token);
    }
}

