using System.Runtime.InteropServices;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace TraineeManagementApi.Services;

public class RedisCacheService: IRedisCacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        string? cachedData = await _cache.GetStringAsync(key);
        if(cachedData == null)
        {
            _logger.LogWarning("Value not found for key: {key} in redis", key);
            return default;
        }

        return JsonSerializer.Deserialize<T>(cachedData);
    }
    public async void SetAsync<T>(string key, T value, TimeSpan ttl)
    {
        var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(ttl);
        string jsonValue = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, jsonValue, options);
    }

    public async void RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }
}