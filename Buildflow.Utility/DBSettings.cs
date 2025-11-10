using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Utility
{
    public class DBSettings
    {
        public static string GetDBMasterConnection(IConfiguration configuration)
        {
            var connection = string.Empty;
            if (configuration.GetConnectionString("IsEnvironmentConnection") == "true")
            {
                var DBServer = Environment.GetEnvironmentVariable("DBServer");
                var Database = Environment.GetEnvironmentVariable("Database");
                var DBPort = Environment.GetEnvironmentVariable("DBPort");
                var DBUser = Environment.GetEnvironmentVariable("DBUser");
                var DBPassword = Environment.GetEnvironmentVariable("DBPassword");

                connection = string.Format("Server={0};Port={1};Database={2};User Id={3};Password={4};",
                   DBServer, DBPort, Database, DBUser, DBPassword);
            }
            else
            {
                connection = configuration.GetConnectionString("SSConnection");
            }
            return connection;
        }

        public static string GetCustomerDBConnection(IConfiguration configuration)
        {
            var connection = string.Empty;
            if (configuration.GetConnectionString("IsEnvironmentConnection") == "true")
            {
                var DBServer = Environment.GetEnvironmentVariable("DBServer");
                var Database = Environment.GetEnvironmentVariable("Database");
                var DBPort = Environment.GetEnvironmentVariable("DBPort");
                var DBUser = Environment.GetEnvironmentVariable("DBUser");
                var DBPassword = Environment.GetEnvironmentVariable("DBPassword");

                connection = string.Format("Server={0};Port={1};Database={2};User Id={3};Password={4};",
                   DBServer, DBPort, Database, DBUser, DBPassword);
            }
            else
            {
                connection = configuration.GetConnectionString("SSCustomerConnection");
            }
            return connection;
        }
        public static string GetUpdatesDBConnection(IConfiguration configuration)
        {
            var connection = string.Empty;
            if (configuration.GetConnectionString("IsEnvironmentConnection") == "true")
            {
                var DBServer = Environment.GetEnvironmentVariable("DBServer");
                var Database = Environment.GetEnvironmentVariable("Database");
                var DBPort = Environment.GetEnvironmentVariable("DBPort");
                var DBUser = Environment.GetEnvironmentVariable("DBUser");
                var DBPassword = Environment.GetEnvironmentVariable("DBPassword");

                connection = string.Format("Server={0};Port={1};Database={2};User Id={3};Password={4};",
                   DBServer, DBPort, Database, DBUser, DBPassword);
            }
            else
            {
                connection = configuration.GetConnectionString("SSUpdatesConnection");
            }
            return connection;
        }

    }
}
