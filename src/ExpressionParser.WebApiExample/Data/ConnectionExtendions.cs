using System.Data.Common;

namespace ExpressionParser.WebApiExample.Data;

internal static class ConnectionExtendions
{
    public static async Task OpenOfClosedAsync(this DbConnection dbConnection, CancellationToken cancellationToken = default)
    {
        if (dbConnection.State != System.Data.ConnectionState.Open)
            await dbConnection.OpenAsync(cancellationToken);
    }
}
