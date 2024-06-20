using IPInformationRetrieval.Abstractions;
using IPInformationRetrieval.Models;

namespace IPInformationRetrieval.Services.Selectors
{
    public class IpDataRetrievalSelectorService : IIpDataRetrievalSelectorService
    {
        private readonly ILogger<IpDataRetrievalSelectorService> _logger;
        private readonly ICache _cache;
        private readonly IIpInformationRepository _ipInformationRepository;
        private readonly IIpInfoServiceProvider _ipInfoServiceProvider;

        public IpDataRetrievalSelectorService(
            ILogger<IpDataRetrievalSelectorService> logger,
            ICache cache,
            IIpInformationRepository ipInformationRepository,
            IIpInfoServiceProvider ipInfoServiceProvider)
        {
            _logger = logger;
            _cache = cache;
            _ipInformationRepository = ipInformationRepository;
            _ipInfoServiceProvider = ipInfoServiceProvider;
        }

        public async Task<(CountryAndIPAddress countryAndIPAddress, ResultsSource resultsSource)> GetAsync(string ip)
        {
            CountryAndIPAddress result = null;

            result = await GetFromCacheAsync(ip);
            if (result != null)
                return (result, ResultsSource.Cache);

            result = await GetFromDatabaseAsync(ip);
            if (result != null)
                return (result, ResultsSource.Database);

            result = await GetFromExternalProviderAsync(ip);
            if (result != null)
                return (result, ResultsSource.ExternalProvider);

            return (result, ResultsSource.None);
        }
        public async Task<List<CountryAndIPAddress>> GetInBatchesWithTasksAsync()
        {
            return await _ipInformationRepository.GetInBatchesWithTasksAsync();
        }

        public async IAsyncEnumerable<CountryAndIPAddress> GetInBatchesAsAsyncEnumerable()
        {
            await foreach (var result in _ipInformationRepository.GetInBatchesAsAsyncEnumerable())
                yield return result;
        }

        public async Task<CountryAndIPAddress> GetFromCacheAsync(string ip)
        {
            return await _cache.GetCachedData<CountryAndIPAddress>(ip);
        }
        public async Task<CountryAndIPAddress> GetFromDatabaseAsync(string ip)
        {
            return await _ipInformationRepository.GetAsync(ip);
        }
        public async Task<CountryAndIPAddress> GetFromExternalProviderAsync(string ip)
        {
            var result = await _ipInfoServiceProvider.GetAsync(ip);
            result = await FixWrongCountryIdReturnedByExternalProvider(result);
            return result;
        }
        public async Task<IEnumerable<ReportLastAddressStats>> GetReportAsync(string[] parameters)
        {
            return await _ipInformationRepository.GetReportAsync(parameters);
        }

        //This is implemented because the provider gives wrong country ids with it's responses
        private async Task<CountryAndIPAddress> FixWrongCountryIdReturnedByExternalProvider(CountryAndIPAddress result)
        {
            var countryDetails = await _ipInformationRepository.GetCountry(result.Country);

            //If the country is not found on database the put zero in order to upsert later
            //otherwise get the correct id from database that corresponds to the actual country
            if (countryDetails != null)
            {
                result.Country.Id = countryDetails.Id;
                result.IPAddress.CountryId = countryDetails.Id;
            }
            else
            {
                //If the country is not found then calculate the next country id
                var nextCountryId = await _ipInformationRepository.GetNextAvailableCountryId();
                result.Country.Id = nextCountryId;
                result.IPAddress.CountryId = nextCountryId;
            }

            return result;
        }
    }
}
