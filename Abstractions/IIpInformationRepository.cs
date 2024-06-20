using IPInformationRetrieval.Models;

namespace IPInformationRetrieval.Abstractions
{
    public interface IIpInformationRepository
    {
        Task<CountryAndIPAddress> GetAsync(string ip);
        Task<List<CountryAndIPAddress>> GetInBatchesWithTasksAsync(int batchSize = 100);
        IAsyncEnumerable<CountryAndIPAddress> GetInBatchesAsAsyncEnumerable(int batchSize = 100);
        Task<IEnumerable<ReportLastAddressStats>> GetReportAsync(string[] parameters);
        Task<Country> GetCountry(Country country);
        Task<int> GetNextAvailableCountryId();
        Task<bool> SetAsync(CountryAndIPAddress ipAddress);
    }
}
