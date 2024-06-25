using IPInformationRetrieval.Abstractions;
using IPInformationRetrieval.Database.DatabaseContext;
using IPInformationRetrieval.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.Common;

namespace IPInformationRetrieval.Database
{
    public class SqliteDb : IDatabaseIpInformation
    {
        private readonly ILogger<SqliteDb> _logger;
        private readonly IpInformationDbContext _db;

        private bool _configured = false;

        public SqliteDb(ILogger<SqliteDb> logger, IpInformationDbContext db)
        {
            _logger = logger;
            _db = db;
        }
        private bool IsDatabaseFileExists()
        {
            var dbFileNameConnectionString = GetConnectionString();

            var connectionString = dbFileNameConnectionString.Split('=');
            if (connectionString == null)
                throw new SqliteException("No db connection could be established.", 1);

            if (connectionString.Length != 2)
                throw new SqliteException("Invalid db file name.", 2);

            var connectionStringAsDbFile = connectionString[1];

            return File.Exists(connectionStringAsDbFile) && new FileInfo(connectionStringAsDbFile).Length > 0;
        }

        public async Task SetupAsync()
        {
            DbTransaction? transaction = null;

            if (_configured)
                return;

            if (IsDatabaseFileExists())
                return;

            var connection = await GetConnectionAsync();

            try
            {
                transaction = await connection.BeginTransactionAsync();
                using var command = connection.CreateCommand();
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

                await connection.CloseAsync();
            }
        }
        public async Task<IDataReader> GetAsync(string query, IEnumerable<QueryParameters> parameters = null)
        {
            var connection = await GetConnectionAsync();
            var command = connection.CreateCommand();
            command.CommandText = query;

            var results = await command.ExecuteReaderAsync();

            return results;
        }

        private async Task<SqliteConnection> GetConnectionAsync()
        {
            var connectionString = GetConnectionString();

            var connection = new SqliteConnection(connectionString);

            if (connection.State == ConnectionState.Closed)
                await connection.OpenAsync();

            if (connection.State == ConnectionState.Broken)
                throw new SqliteException("Connection state is broken.", 3);

            return connection;
        }

        private string GetConnectionString()
        {
            if (_db?.Database?.GetDbConnection()?.ConnectionString == null || _db.Database.GetDbConnection().ConnectionString.IsNullOrEmpty())
            {
                _logger.LogCritical("Could not get connection string."); 
                throw new SqliteException("Failed to instantiate a connection.", 4);
            }

            return _db.Database.GetDbConnection().ConnectionString;
        }
    }
}
