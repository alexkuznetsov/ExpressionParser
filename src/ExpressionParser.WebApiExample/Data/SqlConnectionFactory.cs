using System.Data.Common;

namespace ExpressionParser.WebApiExample.Data;

internal sealed class SqlConnectionFactory
{
    private readonly DbProviderFactory _factory;
    private readonly Conf _dataConfig;

    class Conf
    {
        public string ConnectionString { get; set; } = "Server=(localdb)\\mssqllocaldb;Database=CitiesApiDb;Persist Security Info=True;Trusted_Connection=True;MultipleActiveResultSets=True;Encrypt=False";
        public string Provider { get; set; } = "Microsoft.Data.SqlClient";
    }

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _dataConfig = new Conf();
        configuration.GetSection("Data:Default").Bind(_dataConfig);

        _factory = DbProviderFactories.GetFactory(_dataConfig.Provider);
    }

    public DbConnection CreateConnection()
    {
        var connection = _factory.CreateConnection();

        connection.ConnectionString = _dataConfig.ConnectionString;

        return connection;
    }
}
