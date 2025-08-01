using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Microsoft.Data.SqlClient;

namespace AcuEnvManager
{
    public class DatabaseHelper
    {
        private readonly string _serverName;
        private readonly string _databaseName;

        public DatabaseHelper(string serverName, string databaseName)
        {
            _serverName = serverName ?? throw new ArgumentNullException(nameof(serverName));
            _databaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
        }

        private string BuildConnectionString()
        {
            // Uses Integrated Security; modify as needed for SQL authentication
            return $"Server={_serverName};Database={_databaseName};Integrated Security=True;TrustServerCertificate=True";
        }

        public List<Dictionary<string, object>> ExecuteQuery(string query, Dictionary<string, object>? parameters = null)
        {
            var results = new List<Dictionary<string, object>>();
            var connectionString = BuildConnectionString();

            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection);

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
            }

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null! : reader.GetValue(i);
                }
                results.Add(row);
            }

            return results;
        }

        public T? ExecuteScalar<T>(string query, Dictionary<string, object>? parameters = null)
        {
            var stopwatch = Stopwatch.StartNew();
            var connectionString = BuildConnectionString();

            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(query, connection);

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
            }

            try
            {
                connection.Open();
                var result = command.ExecuteScalar();
                stopwatch.Stop();
                Debug.WriteLine($"ExecuteScalar took {stopwatch.ElapsedMilliseconds} ms for query: {query}");
                return result == null || result is DBNull ? default : (T)result;
            }
            catch (Exception ex)
            {
                if (typeof(T) == typeof(string))
                {
                    return (T)(object)ex.Message;
                }
                throw; // Re-throw the exception for other types
            }
        }

        public string? GetDatabaseVersion()
        {
            const string query = "SELECT version FROM version";
            return ExecuteScalar<string>(query);
        }

        internal bool IsValidConnection()
        {
            var connectionString = BuildConnectionString();
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                return connection.State == System.Data.ConnectionState.Open;
            }
            catch
            {
                return false;
            }
        }
    }
}