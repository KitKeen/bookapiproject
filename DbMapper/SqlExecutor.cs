// SqlExecutor.cs
using System.Data;
using System.Data.Common;
using Npgsql;

internal class SqlExecutor : IDisposable
{
    private string _sql;
    private object _args;
    private DbConnection _connection;
    private Exception _exception;
    private readonly IDbMapper _dbMapper;

    public SqlExecutor(IDbMapper dbMapper)
    {
        _dbMapper = dbMapper;
    }

    public DbConnection CreateConnection()
    {
        var sbConn = new NpgsqlConnectionStringBuilder("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=kitkeen");

        return new NpgsqlConnection(sbConn.ToString());
    }

    public SqlExecutor SetConnection()
    {
        try
        {
            _connection = CreateConnection();
        }
        catch (Exception e)
        {
            e.Data["provider"] = GetType().FullName;
            _exception = e;

            throw;
        }

        return this;
    }

    private async Task OpenConnection()
    {
        if (_connection.State is ConnectionState.Connecting or ConnectionState.Open)
        {
            return;
        }

        await _connection.OpenAsync();
    }

    public SqlExecutor SetQuery(string sql, object args)
    {
        _sql = sql;
        _args = args;

        return this;
    }

    public async Task<T> GetResultOrThrowException<T>(Func<DbConnection, Task<T>> func)
    {
        try
        {
            await OpenConnection();
            return await func(_connection);
        }
        catch (Exception e)
        {
            _exception = e;
            throw;
        }
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}