namespace TraineeManagementApi.Services;

public interface IRedisCacheService
{
    Task<T?> GetAsync<T>(string key);
    void SetAsync<T>(string key, T value, TimeSpan ttl);

    void RemoveAsync(string key);
}