using System.Security.Claims;
using AuthService.Application.DTOs;
using AuthService.Domain.Repositories;
using DotnetBaseKit.Components.Application.Base;
using DotnetBaseKit.Components.Shared.Notifications;
using Microsoft.Identity.Client;

namespace AuthService.Application.Services.Token.Facade;

public class TokenFacade : BaseServiceApplication, ITokenFacade
{
    private readonly ITokenGeneratorServiceApplication _tokenGenerator;
    private readonly ITokenValidatorServiceApplication _tokenValidator;
    private readonly IRedisRefreshTokenRepository _redisRefreshTokenRepository;
    private readonly TimeSpan _refreshTokenExpiry = TimeSpan.FromDays(7);

    public TokenFacade(
        NotificationContext notificationContext,
        ITokenGeneratorServiceApplication tokenGenerator,
        ITokenValidatorServiceApplication tokenValidator, IRedisRefreshTokenRepository redisRefreshTokenRepository) : base(notificationContext)
    {
        _tokenGenerator = tokenGenerator;
        _tokenValidator = tokenValidator;
        _redisRefreshTokenRepository = redisRefreshTokenRepository;
    }

    public async Task<TokenDto> GenerateAndSaveTokensAsync(Guid userId, string email, string name)
    {
        var tokenDto = _tokenGenerator.GenerateToken(userId, email, name);

        await _redisRefreshTokenRepository.SaveRefreshTokenAsync(userId, tokenDto.RefreshToken, _refreshTokenExpiry);

        return tokenDto;
    }

    public async Task<TokenDto?> RefreshTokenAsync(string expiredAccessToken, string refreshToken)
    {
        var principal = _tokenValidator.GetPrincipalFromExpiredToken(expiredAccessToken);
        if (principal == null)
        {
            _notificationContext.AddNotification("Token", "Invalid access token");
            return default;
        }

        var userIdString = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
        {
            _notificationContext.AddNotification("Token", "Invalid access token (missing ID)");
            return default;
        }

        var savedRefreshToken = await _redisRefreshTokenRepository.GetRefreshTokenAsync(userId);
        if (savedRefreshToken != refreshToken)
        {
            _notificationContext.AddNotification("Token", "Expired or invalid refresh token");
            return default;
        }

        var newTokenDto = _tokenGenerator.GenerateToken(principal.Claims);

        await _redisRefreshTokenRepository.SaveRefreshTokenAsync(userId, newTokenDto.RefreshToken, _refreshTokenExpiry);

        return newTokenDto;
    }

    public Task RevokeRefreshTokenAsync(Guid userId)
    {
        throw new NotImplementedException();
    }
}

internal interface IRefreshTokenRepository
{
}