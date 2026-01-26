using System.Security.Claims;
using System.Security.Cryptography;
using AuthService.Application.Services.Token;
using AuthService.Application.Settings;
using AuthService.Domain.Enums;
using DotnetBaseKit.Components.Shared.Notifications;

namespace AuthService.Test.UnitTests.Application.Services;

public class TokenGeneratorServiceApplicationTests
{
    private readonly NotificationContext _notificationContext;
    private readonly TokenGeneratorServiceApplication _service;
    private readonly KeySettings _keySettings;
    private readonly UserRole _role;

    public TokenGeneratorServiceApplicationTests()
    {
        _notificationContext = new NotificationContext();


        using var rsa = RSA.Create(2048);
        var privateKeyPem = rsa.ExportRSAPrivateKeyPem();

        _keySettings = new KeySettings { PrivateKey = privateKeyPem };

        _service = new TokenGeneratorServiceApplication(_notificationContext, _keySettings);
    }

    [Fact(DisplayName = "Should generate token DTO from id, email, and name")]
    public void Should_Generate_Token_From_Id_Email_Name()
    {
        var id = Guid.NewGuid();
        var email = "test@example.com";
        var name = "Test User";

        var result = _service.GenerateToken(id, email, name, _role);

        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.Token));
        Assert.False(string.IsNullOrEmpty(result.RefreshToken));
        Assert.True(result.ExpirationDate > result.IssuedAt);
        Assert.False(_notificationContext.HasNotifications);
    }

    [Fact(DisplayName = "Should generate token DTO from existing claims")]
    public void Should_Generate_Token_From_Claims()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Email, "test@example.com")
        };

        var result = _service.GenerateToken(claims);

        Assert.NotNull(result);
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

    [Fact(DisplayName = "Should return notification when private key is missing")]
    public void Should_Add_Notification_When_Private_Key_Is_Missing()
    {
        var emptyKeySettings = new KeySettings();
        var serviceWithEmptyKeys = new TokenGeneratorServiceApplication(_notificationContext, emptyKeySettings);

        var result = serviceWithEmptyKeys.GenerateToken(Guid.NewGuid(), "test", "test", _role);

        Assert.Null(result);
        Assert.True(_notificationContext.HasNotifications);
        Assert.Contains(_notificationContext.Notifications, n => n.Key == "Token");
    }
}