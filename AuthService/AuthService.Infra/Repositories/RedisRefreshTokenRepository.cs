using AuthService.Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;

namespace AuthService.Infra.Repositories;

public class RedisRefreshTokenRepository : IRedisRefreshTokenRepository
{

    private readonly IDistributedCache _cache;

    private const string Prefix = "refreshToken";

    public RedisRefreshTokenRepository(IDistributedCache cache)
    {
        _cache = cache;
    }

    private string GetKey(Guid userId) => $"{Prefix}{userId}";

    public async Task SaveRefreshTokenAsync(Guid userId, string token, TimeSpan expiry)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry
        };

        var key = GetKey(userId);

        await _cache.SetStringAsync(key, token, options);
    }

    public async Task<string?> GetRefreshTokenAsync(Guid userId)
    {
        var key = GetKey(userId);

        return await _cache.GetStringAsync(key);
    }

    public async Task DeleteRefreshTokenAsync(Guid userId)
    {
        var key = GetKey(userId);
        await _cache.RemoveAsync(key);
    }


}