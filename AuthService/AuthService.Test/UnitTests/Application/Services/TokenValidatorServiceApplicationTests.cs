using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using AuthService.Application.Services.Token;
using AuthService.Application.Settings;
using DotnetBaseKit.Components.Shared.Notifications;

namespace AuthService.Test.UnitTests.Application.Services;

public class TokenValidatorServiceApplicationApplicationTests
{
    private readonly NotificationContext _notificationContext;
    private readonly TokenValidatorServiceApplication _validatorService;
    private readonly KeySettings _keySettings;
    private readonly TokenGeneratorServiceApplication _generatorService;

    public TokenValidatorServiceApplicationApplicationTests()
    {
        _notificationContext = new NotificationContext();

        using var rsa = RSA.Create(2048);
        var privateKeyPem = rsa.ExportRSAPrivateKeyPem();
        var publicKeyPem = rsa.ExportRSAPublicKeyPem();

        _keySettings = new KeySettings
        {
            PrivateKey = privateKeyPem,
            PublicKey = publicKeyPem
        };

        _validatorService = new TokenValidatorServiceApplication(_notificationContext, _keySettings);

        _generatorService = new TokenGeneratorServiceApplication(_notificationContext, _keySettings);
    }

    [Fact(DisplayName = "Should return principal from a valid (even if expired) token")]
    public void Should_Return_Principal_From_Expired_Token()
    {

        var tokenDto = _generatorService.GenerateToken(Guid.NewGuid(), "test@test.com", "Test User");


        var principal = _validatorService.GetPrincipalFromExpiredToken(tokenDto.Token);

        Assert.NotNull(principal);
        Assert.True(principal.Identity!.IsAuthenticated);
        Assert.False(_notificationContext.HasNotifications);
    }

    [Fact(DisplayName = "Should return null if public key is missing")]
    public void Should_Return_Null_When_Public_Key_Is_Missing()
    {
        var emptyKeySettings = new KeySettings(); // Chave pública está nula
        var invalidValidator = new TokenValidatorServiceApplication(_notificationContext, emptyKeySettings);

        var principal = invalidValidator.GetPrincipalFromExpiredToken("qualquer.token.falso");

        Assert.Null(principal);
        Assert.True(_notificationContext.HasNotifications);
        Assert.Contains(_notificationContext.Notifications, n => n.Key == "Token" && n.Message == "Public key not configured.");
    }

    [Fact(DisplayName = "Should return null for a token with invalid signature")]
    public void Should_Return_Null_For_Invalid_Signature()
    {
        using var invalidRsa = RSA.Create(2048);
        var invalidKeySettings = new KeySettings { PrivateKey = invalidRsa.ExportRSAPrivateKeyPem() };
        var invalidGenerator = new TokenGeneratorServiceApplication(new NotificationContext(), invalidKeySettings);
        var invalidToken = invalidGenerator.GenerateToken(Guid.NewGuid(), "fake", "fake");


        var principal = _validatorService.GetPrincipalFromExpiredToken(invalidToken.Token);

        Assert.Null(principal);
        Assert.True(_notificationContext.HasNotifications);
        Assert.Contains(_notificationContext.Notifications, n => n.Key == "Token" && n.Message.StartsWith("Invalid token:"));
    }
}