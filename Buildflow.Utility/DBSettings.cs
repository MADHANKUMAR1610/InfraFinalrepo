using Microsoft.Extensions.Configuration;
using System;

namespace Buildflow.Utility
{
    public class DBSettings
    {
        private static string BuildEnvConnectionString()
        {
            var DBServer = Environment.GetEnvironmentVariable("DBServer");
            var Database = Environment.GetEnvironmentVariable("Database");
            var DBPort = Environment.GetEnvironmentVariable("DBPort");
            var DBUser = Environment.GetEnvironmentVariable("DBUser");
            var DBPassword = Environment.GetEnvironmentVariable("DBPassword");

            // Validate required env vars
            if (string.IsNullOrWhiteSpace(DBServer) ||
                string.IsNullOrWhiteSpace(Database) ||
                string.IsNullOrWhiteSpace(DBPort) ||
                string.IsNullOrWhiteSpace(DBUser) ||
                string.IsNullOrWhiteSpace(DBPassword))
            {
                throw new Exception("Environment DB variables are missing. Please set DBServer, Database, DBPort, DBUser, DBPassword.");
            }

            // Use Host for Npgsql/PostgreSQL
            return $"Host={DBServer};Port={DBPort};Database={Database};Username={DBUser};Password={DBPassword};";
        }

        private static string GetConnection(IConfiguration configuration)
        {
            // If IsEnvironmentConnection key missing -> treat as false
            var isEnv = configuration.GetConnectionString("IsEnvironmentConnection");

            if (!string.IsNullOrWhiteSpace(isEnv) &&
                isEnv.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                return BuildEnvConnectionString();
            }

            // ✅ Always use DefaultConnection from appsettings.json
            var cs = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(cs))
                throw new Exception("DefaultConnection is missing in appsettings.json");

            return cs;
        }

        // ✅ All methods will return same correct connection string
        public static string GetDBMasterConnection(IConfiguration configuration)
            => GetConnection(configuration);

        public static string GetCustomerDBConnection(IConfiguration configuration)
            => GetConnection(configuration);

        public static string GetUpdatesDBConnection(IConfiguration configuration)
            => GetConnection(configuration);
    }
}
