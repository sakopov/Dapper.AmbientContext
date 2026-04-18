using System.Data;
using System.Threading.Tasks;
using Dapper.AmbientContext.Storage;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace Dapper.AmbientContext.IntegrationTests.Fixtures;

public class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("testdb")
        .WithUsername("testuser")
        .WithPassword("testpass")
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        // Initialize the test schema
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        await connection.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS test_table (
                id SERIAL PRIMARY KEY,
                name VARCHAR(100) NOT NULL,
                value INTEGER NOT NULL
            );
        ");
    }

    public async Task DisposeAsync()
    {
        await CleanupAsync();
        await _container.DisposeAsync();
    }

    public IDbConnectionFactory CreateConnectionFactory()
    {
        return new PostgresConnectionFactory(ConnectionString);
    }

    public void ConfigureStorage()
    {
        AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());
    }

    public async Task CleanupAsync()
    {
        AmbientDbContextStorageProvider.SetStorage(null);

        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await connection.ExecuteAsync("DELETE FROM test_table");
    }
}

internal class PostgresConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public PostgresConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection Create()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
