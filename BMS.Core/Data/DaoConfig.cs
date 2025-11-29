using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace BMS.Core.Data;

public static class DaoConfig
{
    private static DbProviderFactory _providerFactory;
    private static string _connectionString;

    // Configure DbExecutor similar to EF Core style
    public static void Configure(Action<DbOptionsBuilder> configure)
    {
        var builder = new DbOptionsBuilder();
        configure(builder);

        DbProviderFactories.RegisterFactory("System.Data.SqlClient",
        "Microsoft.Data.SqlClient.SqlClientFactory, Microsoft.Data.SqlClient");

        _providerFactory = builder.ProviderFactory ?? throw new InvalidOperationException("DbProviderFactory must be set.");
        _connectionString = builder.ConnectionString ?? throw new InvalidOperationException("ConnectionString must be set.");
    }

    public static class ConnectionFactory
    {
        public static DbConnection CreateConnection()
        {
            if (_providerFactory == null || string.IsNullOrWhiteSpace(_connectionString))
                throw new InvalidOperationException("DaoConfig is not configured. Call DaoConfig.Configure(...) first.");

            var conn = _providerFactory.CreateConnection();
            conn.ConnectionString = _connectionString;
            return conn;
        }
    }

    public class DbOptionsBuilder
    {
        public string ConnectionString { get; private set; }
        public DbProviderFactory ProviderFactory { get; private set; }

        public DbOptionsBuilder UseSqlServer(string connectionString)
        {
            ConnectionString = connectionString;
            ProviderFactory = SqlClientFactory.Instance;
            return this;
        }
    }
}

