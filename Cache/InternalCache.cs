using IPInformationRetrieval.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace IPInformationRetrieval.Cache
{
    public class InternalCache : ICache
    {
        private readonly string _defaultCacheDurationConfigurationKey = "DefaultCacheDuration";
        private readonly ILogger<InternalCache> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        public InternalCache(ILogger<InternalCache> logger, IConfiguration configuration, IMemoryCache cache)
        {
            _logger = logger;
            _configuration = configuration;
            _cache = cache;
        }
        public Task<T?> GetCachedData<T>(string key) where T : class
        {
            if (_cache.TryGetValue(key, out object? jsonData))
            {
                if (jsonData != null)
                {
                    return Task.FromResult(JsonSerializer.Deserialize<T>(jsonData.ToString()!));
                }
            }
            return Task.FromResult<T>(null);
        }

        public Task SetCachedData<T>(string key, T data, TimeSpan? cacheDuration = null)
        {
            if (cacheDuration == null && int.TryParse(_configuration[_defaultCacheDurationConfigurationKey], out var cacheDurationFromConfig))
                cacheDuration = TimeSpan.FromHours(cacheDurationFromConfig);

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheDuration
            };
            var jsonData = JsonSerializer.Serialize(data);

            return Task.FromResult(_cache.Set(key, jsonData, options));
        }

        public Task InvalidateCacheData(string key, bool invalidateAllCachedData)
        {
            if (invalidateAllCachedData && _cache is MemoryCache cache)
            {
                cache.Clear();
                return Task.CompletedTask;
            }

            _cache.Remove(key ?? "");
            return Task.CompletedTask;
        }
    }
}