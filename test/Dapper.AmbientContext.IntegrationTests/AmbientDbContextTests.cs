using System.Data;
using System.Threading.Tasks;
using Dapper.AmbientContext.IntegrationTests.Fixtures;
using Shouldly;
using Xunit;

namespace Dapper.AmbientContext.IntegrationTests;

[Collection(PostgresCollection.Name)]
public class AmbientDbContextTests
{
    private readonly PostgresFixture _fixture;

    public AmbientDbContextTests(PostgresFixture fixture)
    {
        _fixture = fixture;
        _fixture.ConfigureStorage();
    }

    [Fact]
    public async Task Context_with_commit_should_persist_changes()
    {
        // Arrange
        var connectionFactory = _fixture.CreateConnectionFactory();
        var factory = new AmbientDbContextFactory(connectionFactory);

        // Act
        using (var context = factory.Create(join: false))
        {
            var prepared = await context.PrepareAsync();

            await prepared.Connection.ExecuteAsync(
                "INSERT INTO test_table (name, value) VALUES (@Name, @Value)",
                new { Name = "test1", Value = 100 });

            context.Commit();
        }

        // Assert - verify data persisted
        await using (var verifyConnection = new Npgsql.NpgsqlConnection(_fixture.ConnectionString))
        {
            await verifyConnection.OpenAsync();
            
            var count = await verifyConnection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM test_table WHERE name = @Name",
                new { Name = "test1" });

            count.ShouldBe(1);
        }

        // Cleanup
        await _fixture.CleanupAsync();
    }

    [Fact]
    public async Task Context_with_rollback_should_not_persist_changes()
    {
        // Arrange
        var connectionFactory = _fixture.CreateConnectionFactory();
        var factory = new AmbientDbContextFactory(connectionFactory);

        // Act
        using (var context = factory.Create(join: false))
        {
            var prepared = await context.PrepareAsync();

            await prepared.Connection.ExecuteAsync(
                "INSERT INTO test_table (name, value) VALUES (@Name, @Value)",
                new { Name = "test2", Value = 200 });

            context.Rollback();
        }

        // Assert - verify data was rolled back
        await using (var verifyConnection = new Npgsql.NpgsqlConnection(_fixture.ConnectionString))
        {
            await verifyConnection.OpenAsync();
            var count = await verifyConnection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM test_table WHERE name = @Name",
                new { Name = "test2" });

            count.ShouldBe(0);
        }

        await _fixture.CleanupAsync();
    }

    [Fact]
    public async Task Context_without_explicit_commit_should_auto_commit_on_dispose()
    {
        // Arrange
        var connectionFactory = _fixture.CreateConnectionFactory();
        var factory = new AmbientDbContextFactory(connectionFactory);

        // Act
        using (var context = factory.Create(join: false))
        {
            var prepared = await context.PrepareAsync();

            await prepared.Connection.ExecuteAsync(
                "INSERT INTO test_table (name, value) VALUES (@Name, @Value)",
                new { Name = "test3", Value = 300 });

            // No explicit commit - should auto-commit on dispose
        }

        // Assert
        await using (var verifyConnection = new Npgsql.NpgsqlConnection(_fixture.ConnectionString))
        {
            await verifyConnection.OpenAsync();

            var count = await verifyConnection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM test_table WHERE name = @Name",
                new { Name = "test3" });

            count.ShouldBe(1);
        }

        await _fixture.CleanupAsync();
    }

    [Fact]
    public async Task Nested_contexts_should_share_same_transaction()
    {
        // Arrange
        var connectionFactory = _fixture.CreateConnectionFactory();
        var factory = new AmbientDbContextFactory(connectionFactory);

        // Act
        using (var parentContext = factory.Create(join: false))
        {
            var parentPrepared = await parentContext.PrepareAsync();

            await parentPrepared.Connection.ExecuteAsync(
                "INSERT INTO test_table (name, value) VALUES (@Name, @Value)",
                new { Name = "parent", Value = 100 });

            using (var childContext = factory.Create(join: true))
            {
                var childPrepared = await childContext.PrepareAsync();

                // Verify child uses same connection and transaction
                childPrepared.Connection.ShouldBeSameAs(parentPrepared.Connection);
                childPrepared.Transaction.ShouldBeSameAs(parentPrepared.Transaction);

                await childPrepared.Connection.ExecuteAsync(
                    "INSERT INTO test_table (name, value) VALUES (@Name, @Value)",
                    new { Name = "child", Value = 200 });
            }

            // Rollback parent - should rollback both inserts
            parentContext.Rollback();
        }

        // Assert - both operations should be rolled back
        await using (var verifyConnection = new Npgsql.NpgsqlConnection(_fixture.ConnectionString))
        {
            await verifyConnection.OpenAsync();

            var count = await verifyConnection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM test_table WHERE name IN ('parent', 'child')");

            count.ShouldBe(0);
        }

        await _fixture.CleanupAsync();
    }

    [Fact]
    public async Task Suppressed_context_should_not_create_transaction()
    {
        // Arrange
        var connectionFactory = _fixture.CreateConnectionFactory();
        var factory = new AmbientDbContextFactory(connectionFactory);

        // Act
        using (var context = factory.Create(join: false, suppress: true))
        {
            var prepared = await context.PrepareAsync();

            // Assert - no transaction should be created
            prepared.Transaction.ShouldBeNull();
            prepared.Connection.ShouldNotBeNull();
            prepared.Connection.State.ShouldBe(ConnectionState.Open);

            // Insert should auto-commit (no transaction)
            await prepared.Connection.ExecuteAsync(
                "INSERT INTO test_table (name, value) VALUES (@Name, @Value)",
                new { Name = "suppressed", Value = 400 });
        }

        // Verify data persisted without explicit commit
        await using (var verifyConnection = new Npgsql.NpgsqlConnection(_fixture.ConnectionString))
        {
            await verifyConnection.OpenAsync();

            var count = await verifyConnection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM test_table WHERE name = @Name",
                new { Name = "suppressed" });

            count.ShouldBe(1);
        }

        await _fixture.CleanupAsync();
    }

    [Fact]
    public async Task Multiple_operations_in_same_context_should_use_same_transaction()
    {
        // Arrange
        var connectionFactory = _fixture.CreateConnectionFactory();
        var factory = new AmbientDbContextFactory(connectionFactory);

        // Act
        using (var context = factory.Create(join: false))
        {
            var prepared = await context.PrepareAsync();

            // Multiple operations
            await prepared.Connection.ExecuteAsync(
                "INSERT INTO test_table (name, value) VALUES (@Name, @Value)",
                new { Name = "op1", Value = 100 });

            await prepared.Connection.ExecuteAsync(
                "INSERT INTO test_table (name, value) VALUES (@Name, @Value)",
                new { Name = "op2", Value = 200 });

            var sum = await prepared.Connection.ExecuteScalarAsync<int>(
                "SELECT SUM(value) FROM test_table WHERE name IN ('op1', 'op2')");

            sum.ShouldBe(300);

            // Rollback all operations
            context.Rollback();
        }

        // Assert - verify rollback
        await using (var verifyConnection = new Npgsql.NpgsqlConnection(_fixture.ConnectionString))
        {
            await verifyConnection.OpenAsync();
            
            var count = await verifyConnection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM test_table WHERE name IN ('op1', 'op2')");

            count.ShouldBe(0);
        }

        await _fixture.CleanupAsync();
    }

    [Fact]
    public async Task Concurrent_child_contexts_should_safely_prepare()
    {
        // Arrange
        var connectionFactory = _fixture.CreateConnectionFactory();
        var factory = new AmbientDbContextFactory(connectionFactory);

        // Act
        using (var parentContext = factory.Create(join: false))
        {
            var parentPrepared = await parentContext.PrepareAsync();

            AmbientDbContext child1 = null;
            AmbientDbContext child2 = null;
            AmbientDbContext child3 = null;

            try
            {
                child1 = (AmbientDbContext)factory.Create(join: true);
                child2 = (AmbientDbContext)factory.Create(join: true);
                child3 = (AmbientDbContext)factory.Create(join: true);

                // Prepare children concurrently
                var task1 = Task.Run(async () => await child1.PrepareAsync());
                var task2 = Task.Run(async () => await child2.PrepareAsync());
                var task3 = Task.Run(async () => await child3.PrepareAsync());

                var results = await Task.WhenAll(task1, task2, task3);

                // All children should share parent's transaction
                results.ShouldAllBe(r => r.Transaction == parentPrepared.Transaction);
                results.ShouldAllBe(r => r.Connection == parentPrepared.Connection);
            }
            finally
            {
                child3?.Dispose();
                child2?.Dispose();
                child1?.Dispose();
            }
        }

        await _fixture.CleanupAsync();
    }
}
