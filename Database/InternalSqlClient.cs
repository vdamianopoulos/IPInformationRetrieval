using IPInformationRetrieval.Abstractions;
using IPInformationRetrieval.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace IPInformationRetrieval.Database
{
    public class InternalSqlClient : IDatabaseIpInformation
    {
        private SqlConnection _connection;
        private readonly ILogger<SqliteDb> _logger;
        private readonly IConfiguration _configuration;

        public InternalSqlClient(ILogger<SqliteDb> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task SetupAsync()
        {
            try
            {
                using SqlConnection connection = new(_configuration.GetConnectionString("DbConnection"));
                await connection.OpenAsync();

                using var command = new SqlCommand(Queries.SeedDataQuery.query, connection);
                await command.ExecuteNonQueryAsync();

            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error connecting to database: " + ex.Message);
            }
        }

        public async Task<IDataReader> GetAsync(string query, IEnumerable<QueryParameters> parameters)
        {
            SqlDataReader? results = null;

            try
            {
                if (!await OpenConnectionAsync())
                    return results;

                using var command = new SqlCommand(query, _connection);

                if (parameters.Any())
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.AddWithValue(parameter.ParameterName, parameter.ParameterValues);
                    }
                }

                results = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return results;
        }

        private async Task<bool> OpenConnectionAsync()
        {
            _connection ??= new SqlConnection(_configuration.GetConnectionString("SQLDb"));

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
