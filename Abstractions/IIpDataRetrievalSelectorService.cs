using IPInformationRetrieval.Models;

namespace IPInformationRetrieval.Abstractions
{
    public interface IIpDataRetrievalSelectorService
    {
        Task<(CountryAndIPAddress countryAndIPAddress, ResultsSource resultsSource)> GetAsync(string ip);
        Task<List<CountryAndIPAddress>> GetInBatchesWithTasksAsync();
        IAsyncEnumerable<CountryAndIPAddress> GetInBatchesAsAsyncEnumerable();
        Task<CountryAndIPAddress> GetFromCacheAsync(string ip);
        Task<CountryAndIPAddress> GetFromDatabaseAsync(string ip);
        Task<CountryAndIPAddress> GetFromExternalProviderAsync(string ip);
        Task<IEnumerable<ReportLastAddressStats>> GetReportAsync(string[] parameters);
    }
}
