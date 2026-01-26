using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthService.Application.DTOs;
using AuthService.Application.Services.Token;
using AuthService.Application.Services.Token.Facade;
using AuthService.Domain.Enums;
using AuthService.Domain.Repositories;
using DotnetBaseKit.Components.Shared.Notifications;
using Moq;

namespace AuthService.Test.UnitTests.Application.Services.Token;

public class TokenFacadeTests
{
    private readonly NotificationContext _notificationContext;
    private readonly Mock<ITokenGeneratorServiceApplication> _generatorMock;
    private readonly Mock<ITokenValidatorServiceApplication> _validatorMock;
    private readonly Mock<IRedisRefreshTokenRepository> _repositoryMock;
    private readonly TokenFacade _facade;

    private readonly Guid _userId = Guid.NewGuid();
    private readonly string _email = "test@example.com";
    private readonly string _name = "Test User";
    private readonly UserRole _role = UserRole.Admin;

    public TokenFacadeTests()
    {
        _notificationContext = new NotificationContext();
        _generatorMock = new Mock<ITokenGeneratorServiceApplication>();
        _validatorMock = new Mock<ITokenValidatorServiceApplication>();
        _repositoryMock = new Mock<IRedisRefreshTokenRepository>();

        _facade = new TokenFacade(
            _notificationContext,
            _generatorMock.Object,
            _validatorMock.Object,
            _repositoryMock.Object
        );
    }

    [Fact(DisplayName = "GenerateAndSaveTokensAsync should call generator and repository")]
    public async Task GenerateAndSaveTokensAsync_Should_Call_Generator_And_Repository()
    {
        var fakeTokenDto = new TokenDto { RefreshToken = "fake-refresh-token" };
        _generatorMock.Setup(g => g.GenerateToken(_userId, _email, _name, _role))
            .Returns(fakeTokenDto);

        var result = await _facade.GenerateAndSaveTokensAsync(_userId, _email, _name, _role);

        Assert.Equal(fakeTokenDto, result);

        _generatorMock.Verify(g => g.GenerateToken(_userId, _email, _name, _role), Times.Once);

        _repositoryMock.Verify(r => r.SaveRefreshTokenAsync(
            _userId,
            "fake-refresh-token",
            It.IsAny<TimeSpan>()),
            Times.Once);
    }

    [Fact(DisplayName = "RefreshTokenAsync should return new tokens on success")]
    public async Task RefreshTokenAsync_Should_Return_New_Tokens_On_Success()
    {

        var expiredToken = "expired-token-string";
        var oldRefreshToken = "old-refresh-token-string";

        var fakeClaims = new[] { new Claim(ClaimTypes.NameIdentifier, _userId.ToString()), new Claim(JwtRegisteredClaimNames.Name, _name) };
        var fakePrincipal = new ClaimsPrincipal(new ClaimsIdentity(fakeClaims, "TestAuth"));

        var newFakeTokenDto = new TokenDto { Token = "new-token", RefreshToken = "new-refresh" };

        _validatorMock.Setup(v => v.GetPrincipalFromExpiredToken(expiredToken)).Returns(fakePrincipal);
        _repositoryMock.Setup(r => r.GetRefreshTokenAsync(_userId)).ReturnsAsync(oldRefreshToken);
        _generatorMock.Setup(g => g.GenerateToken(fakePrincipal.Claims)).Returns(newFakeTokenDto);

        var result = await _facade.RefreshTokenAsync(expiredToken, oldRefreshToken);

        Assert.NotNull(result);
        Assert.Equal("new-token", result.Token);

        _validatorMock.Verify(v => v.GetPrincipalFromExpiredToken(expiredToken), Times.Once);

        _repositoryMock.Verify(r => r.GetRefreshTokenAsync(_userId), Times.Once);

        _repositoryMock.Verify(r => r.SaveRefreshTokenAsync(_userId, "new-refresh", It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact(DisplayName = "RefreshTokenAsync should return null if access token is invalid")]
    public async Task RefreshTokenAsync_Should_Return_Null_If_Access_Token_Invalid()
    {

        _validatorMock.Setup(v => v.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns((ClaimsPrincipal?)null);


        var result = await _facade.RefreshTokenAsync("invalid-token", "any-refresh-token");


        Assert.Null(result);
        Assert.True(_notificationContext.HasNotifications);

        _repositoryMock.Verify(r => r.GetRefreshTokenAsync(It.IsAny<Guid>()), Times.Never);
        _generatorMock.Verify(g => g.GenerateToken(It.IsAny<IEnumerable<Claim>>()), Times.Never);
    }

    [Fact(DisplayName = "RefreshTokenAsync should return null if refresh token doesn't match")]
    public async Task RefreshTokenAsync_Should_Return_Null_If_Refresh_Token_Doesnt_Match()
    {
        var expiredToken = "valid-expired-token";
        var providedRefreshToken = "provided-token-from-client";
        var savedRefreshToken = "DIFFERENT-token-in-redis";

        var fakeClaims = new[] { new Claim(ClaimTypes.NameIdentifier, _userId.ToString()) };
        var fakePrincipal = new ClaimsPrincipal(new ClaimsIdentity(fakeClaims, "TestAuth"));

        _validatorMock.Setup(v => v.GetPrincipalFromExpiredToken(expiredToken)).Returns(fakePrincipal);
        _repositoryMock.Setup(r => r.GetRefreshTokenAsync(_userId)).ReturnsAsync(savedRefreshToken);

        var result = await _facade.RefreshTokenAsync(expiredToken, providedRefreshToken);

        Assert.Null(result);
        Assert.True(_notificationContext.HasNotifications);
        Assert.Contains(_notificationContext.Notifications, n => n.Message == "Expired or invalid refresh token");

        _generatorMock.Verify(g => g.GenerateToken(It.IsAny<IEnumerable<Claim>>()), Times.Never);
    }
}