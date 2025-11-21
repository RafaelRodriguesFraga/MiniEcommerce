using AuthService.Application.DTOs;

namespace AuthService.Application.Services.Token.Facade;

public interface ITokenFacade
{
    Task<TokenDto> GenerateAndSaveTokensAsync(Guid userId, string email, string name);
    Task<TokenDto?> RefreshTokenAsync(string expiredAccessToken, string refreshToken);
    Task RevokeRefreshTokenAsync(Guid userId);
}