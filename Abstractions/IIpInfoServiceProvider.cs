using IPInformationRetrieval.Models;

namespace IPInformationRetrieval.Abstractions
{
    public interface IIpInfoServiceProvider
    {
        Task<CountryAndIPAddress> GetAsync(string ip);
    }
}
