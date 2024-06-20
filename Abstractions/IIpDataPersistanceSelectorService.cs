using IPInformationRetrieval.Models;

namespace IPInformationRetrieval.Abstractions
{
    public interface IIpDataPersistanceSelectorService
    {
        Task<bool> SetAsync(CountryAndIPAddress countryAndIPAddress);
        void SaveToCache(CountryAndIPAddress countryAndIPAddress);
        Task InvalidateCacheData(string key, bool invalidateAllCachedData = false);
        Task<bool> SaveToDatabaseAsync(CountryAndIPAddress countryAndIPAddress);
    }
}
