using System.Data.Common;
using Dapper;

public interface IDbMapper
{
    Task<IEnumerable<T>> ExecuteListAsync<T>(string query, object? args = null) where T : class;
    Task<int> ExecuteNonQueryAsync(string query, object args = null);
    Task<T?> ExecuteObjectOrNullAsync<T>(string query, object args = null) where T : class;
}

public class DbMapper : IDbMapper
{
    private async Task<T> Execute<T>(Func<DbConnection, Task<T>> func, string sql, object args)
    {
        using var exec = new SqlExecutor(this);

        exec
            .SetConnection()
            .SetQuery(sql, args);

        return await exec.GetResultOrThrowException(func);
    }

    public Task<IEnumerable<T>> ExecuteListAsync<T>(string query, object? args = null) where T : class
        => Execute(c => c.QueryAsync<T>(query, args), query, args);

    public Task<int> ExecuteNonQueryAsync(string query, object? args = null)
        => Execute(c => c.ExecuteAsync(query, args), query, args);

    public Task<T?> ExecuteObjectOrNullAsync<T>(string query, object args = null) where T : class
            => Execute(c => c.QueryFirstOrDefaultAsync<T>(query, args), query, args);
}