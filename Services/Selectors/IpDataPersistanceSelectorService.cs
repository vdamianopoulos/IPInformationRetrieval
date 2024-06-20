using IPInformationRetrieval.Abstractions;
using IPInformationRetrieval.Models;

namespace IPInformationRetrieval.Services.Selectors
{
    public class IpDataPersistanceSelectorService : IIpDataPersistanceSelectorService
    {
        private readonly ILogger<IpDataPersistanceSelectorService> _logger;
        private readonly ICache _cache;
        private readonly IIpInformationRepository _ipInformationRepository;

        public IpDataPersistanceSelectorService(
            ILogger<IpDataPersistanceSelectorService> logger,
            ICache cache,
            IIpInformationRepository ipInformationRepository)
        {
            _logger = logger;
            _cache = cache;
            _ipInformationRepository = ipInformationRepository;
        }

        public async Task<bool> SetAsync(CountryAndIPAddress countryAndIPAddress)
        {
            await SaveToDatabaseAsync(countryAndIPAddress);
            SaveToCache(countryAndIPAddress);
            return true;
        }
        public void SaveToCache(CountryAndIPAddress countryAndIPAddress)
        {
            _cache.SetCachedData(countryAndIPAddress.IPAddress.IP, countryAndIPAddress);
        }
        public Task InvalidateCacheData(string key, bool invalidateAllCachedData = false)
        {
            _cache.InvalidateCacheData(key, invalidateAllCachedData);
            return Task.CompletedTask;
        }
        public async Task<bool> SaveToDatabaseAsync(CountryAndIPAddress countryAndIPAddress)
        {
            return await _ipInformationRepository.SetAsync(countryAndIPAddress);
        }
    }
}
