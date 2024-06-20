namespace IPInformationRetrieval.Abstractions
{
    public interface ICache
    {
        Task<T?> GetCachedData<T>(string key) where T : class;

        Task SetCachedData<T>(string key, T data, TimeSpan? cacheDuration = null);
        Task InvalidateCacheData(string key, bool invalidateAllCachedData);
    }
}
