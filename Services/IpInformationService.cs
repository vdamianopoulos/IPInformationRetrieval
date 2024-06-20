using IPInformationRetrieval.Abstractions;
using IPInformationRetrieval.Controllers;
using IPInformationRetrieval.Models;

namespace IPInformationRetrieval.Services
{
    public class IpInformationService : IIpInformationService
    {
        private readonly ILogger<IpInformationController> _logger;
        private readonly IIpDataRetrievalSelectorService _ipDataRetrievalSelectorService;
        private readonly IIpDataPersistanceSelectorService _ipDataPersistanceSelectorService;
        private readonly IIpDataUpdateSelectorService _ipDataUpdateSelectorService;
        public IpInformationService(
            ILogger<IpInformationController> logger,
            IIpDataRetrievalSelectorService ipDataRetrievalSelectorService,
            IIpDataPersistanceSelectorService ipDataPersistanceSelectorService,
            IIpDataUpdateSelectorService ipDataUpdateSelectorService)
        {
            _logger = logger;
            _ipDataRetrievalSelectorService = ipDataRetrievalSelectorService;
            _ipDataPersistanceSelectorService = ipDataPersistanceSelectorService;
            _ipDataUpdateSelectorService = ipDataUpdateSelectorService;
        }

        public async Task<(CountryAndIPAddress countryAndIPAddress, ResultsSource resultsSource)> GetAsync(string ip)
        {
            return await _ipDataRetrievalSelectorService.GetAsync(ip);
        }

        public async Task SetAsync(CountryAndIPAddress countryAndIPAddress)
        {
            await _ipDataPersistanceSelectorService.SetAsync(countryAndIPAddress);
        }

        public async Task<int?> UpdateDataInBatchesWithTasksAsync()
        {
            return await _ipDataUpdateSelectorService.UpdateDataInBatchesWithTasksAsync();
        }

        public async Task<int?> UpdateDataInBatchesAsAsyncEnumerable()
        {
            return await _ipDataUpdateSelectorService.UpdateDataInBatchesAsAsyncEnumerable();
        }

        public async Task InvalidateCache(string key, bool invalidateAllCachedData = false)
        {
            await _ipDataPersistanceSelectorService.InvalidateCacheData(key, invalidateAllCachedData);
        }

        public async Task<IEnumerable<ReportLastAddressStats>> GetReportAsync(string[] parameters)
        {
            return await _ipDataRetrievalSelectorService.GetReportAsync(parameters);
        }
    }
}
