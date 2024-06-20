using IPInformationRetrieval.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace IPInformationRetrieval.Cache
{
    //Not used, only for demonstration purposes
    [Obsolete]
    public class RedisCache : ICache
    {
        private readonly string _defaultCacheDurationConfigurationKey = "DefaultCacheDuration";
        private readonly ILogger<RedisCache> _logger;
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _cache;
        public RedisCache(ILogger<RedisCache> logger, IConfiguration configuration, IDistributedCache cache)
        {
            _logger = logger;
            _configuration = configuration;
            _cache = cache;
        }
        public async Task<T?> GetCachedData<T>(string key) where T : class
        {
            var result = await _cache.GetStringAsync(key);

            if (result == null)
                return (T)null;

            return JsonSerializer.Deserialize<T>(result.ToString());
        }

        public async Task SetCachedData<T>(string key, T data, TimeSpan? cacheDuration = null)
        {
            if (cacheDuration == null && int.TryParse(_configuration[_defaultCacheDurationConfigurationKey], out var cacheDurationFromConfig))
                cacheDuration = TimeSpan.FromHours(cacheDurationFromConfig);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheDuration
            };
            var jsonData = JsonSerializer.Serialize(data);

            await _cache.SetStringAsync(key, jsonData, options);
        }

        public async Task InvalidateCacheData(string key, bool invalidateAllCachedData)
        {
            if (invalidateAllCachedData && _cache is Microsoft.Extensions.Caching.Memory.MemoryCache cache)
            {
                cache.Clear();
                return;
            }

            await _cache.RemoveAsync(key ?? "");
        }
    }
}
