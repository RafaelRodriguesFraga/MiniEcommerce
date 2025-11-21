namespace AuthService.Domain.Repositories;

public interface IRedisRefreshTokenRepository
{
    Task SaveRefreshTokenAsync(Guid userId, string token, TimeSpan expiry);
    Task<string?> GetRefreshTokenAsync(Guid userId);
    Task DeleteRefreshTokenAsync(Guid userId);
}