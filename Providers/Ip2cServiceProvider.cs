using IPInformationRetrieval.Abstractions;
using IPInformationRetrieval.Models;
using Polly;

namespace IPInformationRetrieval.Providers
{
    public class Ip2cServiceProvider : IIpInfoServiceProvider
    {
        private readonly ILogger<Ip2cServiceProvider> _logger;
        private readonly IIp2cEndpoints _client;

        public Ip2cServiceProvider(ILogger<Ip2cServiceProvider> logger, IIp2cEndpoints client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task<CountryAndIPAddress> GetAsync(string ip)
        {
            CountryAndIPAddress? data = null;
            try
            {
                //Implementing the polly retry policy here as well besides program.cs until to make sure that the centralized works
                var retryPolicy = Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

                if (retryPolicy == null)
                    return data;

                data = await retryPolicy.ExecuteAsync(async () =>
                {
                    var results = await _client.GetAsync(ip);
                    if (string.IsNullOrWhiteSpace(results))
                        return data;

                    return Mappers.Mappers.MapToModel(results, ip);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return data;
        }
    }
}