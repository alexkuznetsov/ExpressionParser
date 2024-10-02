namespace ExpressionParser.WebApiExample.Common;

internal interface IDataSession
{
    Task<int> ExecuteAsync(string sql, object? parameters = null);

    Task<ICollection<T>> QueryAsync<T>(string sql, object? parameters = null);
    Task<ICollection<TResult>> QueryAsync<T1, T2, TResult>(string sql, Func<T1, T2, TResult> map, object? parameters = null);
    Task<ICollection<TResult>> QueryAsync<T1, T2, T3, TResult>(string sql, Func<T1, T2, T3, TResult> map, object? parameters = null);
    Task<ICollection<TResult>> QueryAsync<T1, T2, T3, T4, TResult>(string sql, Func<T1, T2, T3, T4, TResult> map, object? parameters = null);
    Task<ICollection<TResult>> QueryAsync<T1, T2, T3, T4, T5, TResult>(string sql, Func<T1, T2, T3, T4, T5, TResult> map, object? parameters = null);

    Task<ICollection<dynamic>> QueryAsync(string sql, object? parameters = null);

    Task<T?> ExecuteScalarAsync<T>(string sql, object? parameters = null);

    Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null);
    Task<TResult?> QueryFirstOrDefaultAsync<T1, T2, TResult>(string sql, Func<T1, T2, TResult> map, object? parameters = null);

    Task<T> QuerySingleAsync<T>(string sql, object? parameters = null);
}
