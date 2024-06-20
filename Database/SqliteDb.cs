using IPInformationRetrieval.Abstractions;
using IPInformationRetrieval.Models;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Data.Common;

namespace IPInformationRetrieval.Database
{
    public class SqliteDb : IDatabaseIpInformation
    {
        private readonly ILogger<SqliteDb> _logger;
        private readonly IConfiguration _configuration;

        private SqliteConnection _connection;
        private bool _configured = false;

        public SqliteDb(ILogger<SqliteDb> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        private bool IsDatabaseFileExists()
        {
            var dbFileName = "";
            var connectionString = _configuration.GetConnectionString("SQLiteDb").Split('=');
            if (connectionString?.Length == 2)
                dbFileName = connectionString[1];

            return File.Exists(dbFileName) && new FileInfo(dbFileName).Length > 0;
        }

        public async Task SetupAsync()
        {
            if (IsDatabaseFileExists())
                return;

            if (_configured)
                return;

            var isConnectionOpen = await OpenConnectionAsync();
            if (!isConnectionOpen)
                return;

            DbTransaction? transaction = null;
            try
            {
                transaction = await _connection.BeginTransactionAsync();
                using var command = _connection.CreateCommand();
                command.CommandText = Queries.SeedDataQueryInSQLite.query;

                await command.ExecuteNonQueryAsync();
                await transaction.CommitAsync();
                _configured = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                if (transaction != null)
                    await transaction.RollbackAsync();
            }
            finally
            {
                if (transaction != null)
                    await transaction.DisposeAsync();

                await _connection.CloseAsync();
            }
        }
        public async Task<IDataReader> GetAsync(string query, IEnumerable<QueryParameters> parameters = null)
        {
            SqliteDataReader results = null;
            try
            {
                var isConnectionOpen = await OpenConnectionAsync();
                if (!isConnectionOpen)
                    return results;

                var command = _connection.CreateCommand();
                command.CommandText = query;
                results = await command.ExecuteReaderAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return results;
        }

        private async Task<bool> OpenConnectionAsync()
        {
            _connection ??= new SqliteConnection(_configuration.GetConnectionString("SQLiteDb"));

            try
            {
                if (_connection.State == ConnectionState.Closed)
                    await _connection.OpenAsync();

                if (_connection.State == ConnectionState.Broken)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Could not open connection to the database - " + ex.Message);
            }
            return false;
        }
    }
}
