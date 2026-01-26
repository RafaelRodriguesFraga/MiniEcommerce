using AuthService.Application.DTOs;
using AuthService.Domain.Enums;

namespace AuthService.Application.Services.Token.Facade;

public interface ITokenFacade
{
    Task<TokenDto> GenerateAndSaveTokensAsync(Guid userId, string email, string name, UserRole role);
    Task<TokenDto?> RefreshTokenAsync(string expiredAccessToken, string refreshToken);
    Task RevokeRefreshTokenAsync(Guid userId);
}