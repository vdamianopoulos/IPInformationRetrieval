using IPInformationRetrieval.Abstractions;
using IPInformationRetrieval.Database.DatabaseContext;
using IPInformationRetrieval.Helpers;
using IPInformationRetrieval.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Polly;

namespace IPInformationRetrieval.Repositories
{
    public class IpInformationRepository : IIpInformationRepository
    {
        private readonly ILogger<IpInformationRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly IDatabaseIpInformation _database;
        private readonly IpInformationDbContext _db;

        public IpInformationRepository(
            ILogger<IpInformationRepository> logger,
            IDatabaseIpInformation database,
            IConfiguration configuration,
            IpInformationDbContext db)
        {
            _logger = logger;
            _database = database;
            _configuration = configuration;
            _db = db;
        }

        #region Get 
        public async Task<CountryAndIPAddress> GetAsync(string ip)
        {
            CountryAndIPAddress? countryAndIPAddress = null;

            try
            {
                //Implementing the polly retry policy here as well besides program.cs until to make sure that the centralized works
                var retryPolicy = Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

                if (retryPolicy == null)
                    return countryAndIPAddress;

                countryAndIPAddress = await retryPolicy.ExecuteAsync(async () =>
                {

                    var ipAddress = await _db.IpAddresses.FirstOrDefaultAsync(x => x.IP == ip);
                    if (ipAddress == null)
                        return countryAndIPAddress;

                    var country = await _db.Countries.FirstOrDefaultAsync(x => x.Id == ipAddress.CountryId);
                    if (country == null)
                    {
                        _logger.LogInformation("Something went wrong here and they are is the ip but not the country");
                        return countryAndIPAddress;
                    }

                    return new CountryAndIPAddress()
                    {
                        Country = new Country(country.Id, country.TwoLetterCode, country.ThreeLetterCode, country.Name),
                        IPAddress = new IpAddress(ipAddress.Id, ipAddress.CountryId, ipAddress.IP)
                    };
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ip);
            }

            return countryAndIPAddress;
        }

        public async Task<List<CountryAndIPAddress>> GetInBatchesWithTasksAsync(int batchSize = 100)
        {
            var tasks = new List<Task<List<CountryAndIPAddress>>>();

            try
            {
                //Implementing the polly retry policy here as well besides program.cs until to make sure that the centralized works
                var retryPolicy = Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

                if (retryPolicy == null)
                    return [];

                await retryPolicy.ExecuteAsync(async () =>
                {

                    if (int.TryParse(_configuration["BatchSize"], out var batchSizeFromConfiguration))
                        batchSize = batchSizeFromConfiguration;

                    var addressesCount = await _db.IpAddresses.CountAsync();

                    int skipCount = 0;
                    int takeCount = 0;
                    while (skipCount < addressesCount)
                    {
                        takeCount = Math.Min(batchSize, addressesCount - skipCount);

                        tasks.Add(_db.IpAddresses
                            .OrderBy(x => x.IP)
                            .Skip(skipCount)
                            .Take(takeCount)
                            .Join(_db.Countries,
                                innerJoin => innerJoin.CountryId,
                                outerJoin => outerJoin.Id,
                                (innerJoin, outerJoin) => new
                                {
                                    countryId = innerJoin.CountryId,
                                    ip = innerJoin.IP,
                                    name = outerJoin.Name,
                                    twoLetterCode = outerJoin.TwoLetterCode,
                                    threeLetterCode = outerJoin.ThreeLetterCode
                                })
                            .Select(x => new CountryAndIPAddress()
                            {
                                Country = new Country(x.countryId, x.twoLetterCode, x.threeLetterCode, x.name),
                                IPAddress = new IpAddress(x.countryId, x.ip)
                            }
                            )
                            .ToListAsync());

                        skipCount += batchSize;
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return (await Task.WhenAll(tasks)).SelectMany(x => x).ToList();
        }

        public async IAsyncEnumerable<CountryAndIPAddress> GetInBatchesAsAsyncEnumerable(int batchSize = 100)
        {
            if (int.TryParse(_configuration["BatchSize"], out var batchSizeFromConfiguration))
                batchSize = batchSizeFromConfiguration;

            var addressesCount = await _db.IpAddresses.CountAsync();

            int skipCount = 0;
            int takeCount = 0;
            while (skipCount < addressesCount)
            {
                takeCount = Math.Min(batchSize, addressesCount - skipCount);

                var result = _db.IpAddresses
                    .OrderBy(x => x.IP)
                    .Skip(skipCount)
                    .Take(takeCount)
                    .Join(_db.Countries,
                        innerJoin => innerJoin.CountryId,
                        outerJoin => outerJoin.Id,
                        (innerJoin, outerJoin) => new
                        {
                            countryId = innerJoin.CountryId,
                            ip = innerJoin.IP,
                            name = outerJoin.Name,
                            twoLetterCode = outerJoin.TwoLetterCode,
                            threeLetterCode = outerJoin.ThreeLetterCode
                        })
                    .Select(x => new CountryAndIPAddress()
                    {
                        Country = new Country(x.countryId, x.twoLetterCode, x.threeLetterCode, x.name),
                        IPAddress = new IpAddress(x.countryId, x.ip)
                    }
                    )
                    .AsAsyncEnumerable();

                await foreach (var data in result)
                {
                    yield return data;
                }
                skipCount += batchSize;
            }
        }

        public async Task<IEnumerable<ReportLastAddressStats>> GetReportAsync(string[] parameters)
        {
            var result = new List<ReportLastAddressStats>();
            try
            {
                //Implementing the polly retry policy here as well besides program.cs until to make sure that the centralized works
                var retryPolicy = Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

                if (retryPolicy == null)
                    return [];

                await retryPolicy.ExecuteAsync(async () =>
                {

                    System.Data.IDataReader reader;

                    //reader = await FirstWayWithParameters_NotWorkingCurrenly(parameters);
                    reader = await SecondWayWithInterpolation_WithExploits(parameters);
                    result = Helpers.Helpers.GetReportFromReader(reader);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return result;
        }

        public async Task<Country> GetCountry(Country country)
        {
            //Implementing the polly retry policy here as well besides program.cs until to make sure that the centralized works
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            if (retryPolicy == null)
                return null;

            return await retryPolicy.ExecuteAsync(async () =>
            {
                return await _db.Countries.FirstOrDefaultAsync(x =>
                            x.TwoLetterCode == country.TwoLetterCode &&
                            x.ThreeLetterCode == country.ThreeLetterCode &&
                            x.Name == country.Name
                          );
            });
        }

        public async Task<int> GetNextAvailableCountryId()
        {
            //Implementing the polly retry policy here as well besides program.cs until to make sure that the centralized works
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            if (retryPolicy == null)
                return -1;

            return await retryPolicy.ExecuteAsync(async () =>
            {
                return await _db.Countries.MaxAsync(x => x.Id) + 1;
            });
        }
        #endregion

        #region Set-Update
        public async Task<bool> SetAsync(CountryAndIPAddress countryAndIPAddress)
        {
            await UpdateCountryIfNotExists(countryAndIPAddress.Country);
            return await UpsertIpAddress(countryAndIPAddress.IPAddress);
        }

        private async Task<bool> UpdateCountryIfNotExists(Country country)
        {
            IDbContextTransaction? transaction = null;
            try
            {
                //Implementing the polly retry policy here as well besides program.cs until to make sure that the centralized works
                var retryPolicy = Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

                if (retryPolicy == null)
                    return false;

                await retryPolicy.ExecuteAsync(async () =>
                {
                    transaction = await _db.Database.BeginTransactionAsync();

                    var found = await _db.Countries.AnyAsync(x => x.Id == country.Id);
                    if (!found)
                    {
                        await _db.Countries.AddAsync(country);
                        await _db.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                if (transaction != null)
                    await transaction.RollbackAsync();

                return false;
            }
            finally
            {
                if (transaction != null)
                    await transaction.DisposeAsync();
            }
            return true;
        }

        private async Task<bool> UpsertIpAddress(IpAddress newData)
        {
            IDbContextTransaction? transaction = null;
            try
            {
                //Implementing the polly retry policy here as well besides program.cs until to make sure that the centralized works
                var retryPolicy = Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

                if (retryPolicy == null)
                    return false;

                await retryPolicy.ExecuteAsync(async () =>
                {
                    transaction = await _db.Database.BeginTransactionAsync();

                    //Check and fetch for existing data
                    var existingData = await _db.IpAddresses.FirstOrDefaultAsync(x => x.IP == newData.IP);

                    //If there are no existing data "update" db (it is an insert really)
                    //Otherwise merge existing data object with new data and update db
                    if (existingData == null)
                    {
                        _db.IpAddresses.Update(newData);
                    }
                    else
                    {
                        existingData.MergeExistingDataWithNewData(newData, _db);
                        _db.IpAddresses.Update(existingData);
                    }
                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                if (transaction != null)
                    await transaction.RollbackAsync();

                return false;
            }
            finally
            {
                if (transaction != null)
                    await transaction.DisposeAsync();
            }
            return true;
        }
        #endregion

        #region Helpers
        //Unused, only for demonstration purposes
        private async Task<System.Data.IDataReader> FirstWayWithParameters_NotWorkingCurrenly(string[] parameters)
        {
            var queryParameters = new List<QueryParameters>()
                {
                    new (_configuration["ReportParameter"], parameters)
                };
            return await _database.GetAsync(Queries.ReportLastAddressStatsQuery.query, queryParameters);
        }
        private async Task<System.Data.IDataReader> SecondWayWithInterpolation_WithExploits(string[] parameters)
        {
            if (parameters.Length == 0)
                return null;

            var values = $"'{string.Join("','", parameters)}'";

            return await _database.GetAsync(
                Queries.ReportLastAddressStatsQuery.GetQueryWithInterpolation(_configuration["ReportParameter"], values));
        }
        #endregion
    }
}
