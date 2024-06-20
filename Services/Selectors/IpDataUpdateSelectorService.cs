using IPInformationRetrieval.Abstractions;
using IPInformationRetrieval.Models;

namespace IPInformationRetrieval.Services.Selectors
{
    public class IpDataUpdateSelectorService : IIpDataUpdateSelectorService
    {
        private readonly ILogger<IpDataUpdateSelectorService> _logger;
        private readonly IIpDataRetrievalSelectorService _ipDataRetrievalSelectorService;
        private readonly IIpDataPersistanceSelectorService _ipDataPersistanceSelectorService;

        public IpDataUpdateSelectorService(
            ILogger<IpDataUpdateSelectorService> logger,
            IIpDataRetrievalSelectorService ipDataRetrievalSelectorService,
            IIpDataPersistanceSelectorService ipDataPersistanceSelectorService)
        {
            _logger = logger;
            _ipDataRetrievalSelectorService = ipDataRetrievalSelectorService;
            _ipDataPersistanceSelectorService = ipDataPersistanceSelectorService;
        }

        public async Task<int?> UpdateDataInBatchesWithTasksAsync()
        {
            int changedData = 0;
            var results = await _ipDataRetrievalSelectorService.GetInBatchesWithTasksAsync();
            foreach (var result in results)
            {
                var success = await GetAndUpdateOnlyChangedData(result);
                if (success)
                    changedData++;
            }
            return changedData;
        }

        public async Task<int?> UpdateDataInBatchesAsAsyncEnumerable()
        {
            int changedData = 0;
            await foreach (var result in _ipDataRetrievalSelectorService.GetInBatchesAsAsyncEnumerable())
            {
                var success = await GetAndUpdateOnlyChangedData(result);
                if (success)
                    changedData++;
            }
            return changedData;
        }

        private async Task<bool> GetAndUpdateOnlyChangedData(CountryAndIPAddress result)
        {
            try
            {
                var externalProviderResult = await _ipDataRetrievalSelectorService.GetFromExternalProviderAsync(result.IPAddress.IP);
                var dbResult = await _ipDataRetrievalSelectorService.GetFromDatabaseAsync(externalProviderResult.IPAddress.IP);
                if (Helpers.Helpers.HasIpInformationChanged(externalProviderResult, dbResult))
                {
                    var success = await _ipDataPersistanceSelectorService.SaveToDatabaseAsync(externalProviderResult);
                    await _ipDataPersistanceSelectorService.InvalidateCacheData(externalProviderResult.IPAddress.IP);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, result.IPAddress.IP);
            }
            return false;
        }
    }
}
