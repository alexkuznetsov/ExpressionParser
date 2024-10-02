using Dapper;

using ExpressionParser.WebApiExample.Common;

namespace ExpressionParser.WebApiExample.Data;

internal sealed class DapperDataSessionImpl(SqlConnectionFactory sqlConnectionFactory) : IDataSession
{
    public Guid DalId { get; } = Guid.NewGuid();

    public async Task<int> ExecuteAsync(string sql, object? parameters = null)
    {
        await using var dbConnection = sqlConnectionFactory.CreateConnection();
        await dbConnection.OpenOfClosedAsync().ConfigureAwait(false);

        return await dbConnection.ExecuteAsync(sql, parameters).ConfigureAwait(false);
    }

    public async Task<ICollection<T>> QueryAsync<T>(string sql, object? parameters = null)
    {
        await using var dbConnection = sqlConnectionFactory.CreateConnection();
        await dbConnection.OpenOfClosedAsync().ConfigureAwait(false);

        var result = await dbConnection.QueryAsync<T>(sql, parameters).ConfigureAwait(false);

        return result.ToArray();
    }

    public async Task<ICollection<TResult>> QueryAsync<T1, T2, TResult>(string sql, Func<T1, T2, TResult> map, object? parameters = null)
    {
        await using var dbConnection = sqlConnectionFactory.CreateConnection();
        await dbConnection.OpenOfClosedAsync().ConfigureAwait(false);

        var result = await dbConnection.QueryAsync(sql, map, parameters).ConfigureAwait(false);

        return result.ToArray();
    }
    public async Task<ICollection<TResult>> QueryAsync<T1, T2, T3, TResult>(string sql, 
        Func<T1, T2, T3, TResult> map, 
        object? parameters = null)
    {
        await using var dbConnection = sqlConnectionFactory.CreateConnection();
        await dbConnection.OpenOfClosedAsync().ConfigureAwait(false);

        var result = await dbConnection.QueryAsync(sql, map, parameters).ConfigureAwait(false);

        return result.ToArray();
    }

    public async Task<ICollection<TResult>> QueryAsync<T1, T2, T3, T4, TResult>(string sql, 
        Func<T1, T2, T3, T4, TResult> map, 
        object? parameters = null)
    {
        await using var dbConnection = sqlConnectionFactory.CreateConnection();
        await dbConnection.OpenOfClosedAsync().ConfigureAwait(false);

        var result = await dbConnection.QueryAsync(sql, map, parameters).ConfigureAwait(false);

        return result.ToArray();
    }

    public async Task<ICollection<TResult>> QueryAsync<T1, T2, T3, T4, T5, TResult>(string sql, 
        Func<T1, T2, T3, T4, T5, TResult> map, 
        object? parameters = null)
    {
        await using var dbConnection = sqlConnectionFactory.CreateConnection();
        await dbConnection.OpenOfClosedAsync().ConfigureAwait(false);

        var result = await dbConnection.QueryAsync(sql, map, parameters).ConfigureAwait(false);

        return result.ToArray();
    }

    public async Task<ICollection<dynamic>> QueryAsync(string sql, object? parameters = null)
    {
        await using var dbConnection = sqlConnectionFactory.CreateConnection();
        await dbConnection.OpenOfClosedAsync().ConfigureAwait(false);

        var result = await dbConnection.QueryAsync(sql, parameters).ConfigureAwait(false);

        return result.ToArray();
    }

    public async Task<T?> ExecuteScalarAsync<T>(string sql, object? parameters = null)
    {
        await using var dbConnection = sqlConnectionFactory.CreateConnection();
        await dbConnection.OpenOfClosedAsync().ConfigureAwait(false);

        return await dbConnection.ExecuteScalarAsync<T>(sql, parameters).ConfigureAwait(false);
    }

    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null)
    {
        await using var dbConnection = sqlConnectionFactory.CreateConnection();
        await dbConnection.OpenOfClosedAsync().ConfigureAwait(false);

        return await dbConnection.QueryFirstOrDefaultAsync<T>(sql, parameters).ConfigureAwait(false);
    }

    public async Task<TResult?> QueryFirstOrDefaultAsync<T1, T2, TResult>(string sql, Func<T1, T2, TResult> map, object? parameters = null)
    {
        await using var dbConnection = sqlConnectionFactory.CreateConnection();
        await dbConnection.OpenOfClosedAsync().ConfigureAwait(false);

        var result = await dbConnection.QueryAsync(sql, map, parameters).ConfigureAwait(false);

        return result.SingleOrDefault();
    }

    public async Task<T> QuerySingleAsync<T>(string sql, object? parameters = null)
    {
        await using var dbConnection = sqlConnectionFactory.CreateConnection();
        await dbConnection.OpenOfClosedAsync().ConfigureAwait(false);

        return await dbConnection.QuerySingleAsync<T>(sql, parameters).ConfigureAwait(false);
    }
}
