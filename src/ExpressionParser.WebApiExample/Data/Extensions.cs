using System.Data.Common;

using ExpressionParser.WebApiExample.Common;

namespace ExpressionParser.WebApiExample.Data;

internal static class Extensions
{
    internal static IServiceCollection AddData(this IServiceCollection services)
    {
        DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", 
            Microsoft.Data.SqlClient.SqlClientFactory.Instance);
        services.AddScoped<SqlConnectionFactory>();

        services.AddScoped<IDataSession, DapperDataSessionImpl>();

        return services;
    }
}
