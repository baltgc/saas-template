using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace saas_template.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(IDistributedCache distributedCache, ILogger<RedisCacheService> logger)
    {
        _distributedCache = distributedCache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        try
        {
            var cachedValue = await _distributedCache.GetStringAsync(key);
            if (string.IsNullOrEmpty(cachedValue))
                return null;

            return JsonSerializer.Deserialize<T>(cachedValue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cache key: {Key}", key);
            return null;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        try
        {
            var serializedValue = JsonSerializer.Serialize(value);
            var options = new DistributedCacheEntryOptions();

            if (expiration.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = expiration;
            }
            else
            {
                // Default expiration: 5 minutes
                options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            }

            await _distributedCache.SetStringAsync(key, serializedValue, options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await _distributedCache.RemoveAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache key: {Key}", key);
        }
    }

    public async Task RemoveByPatternAsync(string pattern)
    {
        // Note: Redis doesn't support pattern deletion directly in IDistributedCache
        // This would require direct Redis connection. For now, log a warning.
        _logger.LogWarning("Pattern-based cache removal not fully supported. Pattern: {Pattern}", pattern);
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            var value = await _distributedCache.GetStringAsync(key);
            return !string.IsNullOrEmpty(value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking cache key existence: {Key}", key);
            return false;
        }
    }
}

