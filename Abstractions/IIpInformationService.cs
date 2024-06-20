using IPInformationRetrieval.Models;

namespace IPInformationRetrieval.Abstractions
{
    public interface IIpInformationService
    {
        Task<(CountryAndIPAddress countryAndIPAddress, ResultsSource resultsSource)> GetAsync(string ip);
        Task SetAsync(CountryAndIPAddress countryAndIPAddress);
        Task<int?> UpdateDataInBatchesWithTasksAsync();
        Task<int?> UpdateDataInBatchesAsAsyncEnumerable();
        Task InvalidateCache(string key, bool invalidateAllCachedData = false);
        Task<IEnumerable<ReportLastAddressStats>> GetReportAsync(string[] parameters);
    }
}
