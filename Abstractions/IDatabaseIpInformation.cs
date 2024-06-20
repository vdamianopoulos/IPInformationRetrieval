using IPInformationRetrieval.Models;

namespace IPInformationRetrieval.Abstractions
{
    public interface IDatabaseIpInformation
    {
        Task SetupAsync();
        Task<System.Data.IDataReader> GetAsync(string query, IEnumerable<QueryParameters> parameters = null);
    }
}