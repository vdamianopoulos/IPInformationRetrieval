using Refit;

namespace IPInformationRetrieval.Abstractions
{
    public interface IIp2cEndpoints
    {
        [Get("/{ip}")]
        Task<string> GetAsync(string ip);
    }
}
