using Xunit;

namespace Dapper.AmbientContext.IntegrationTests.Fixtures;

[CollectionDefinition(Name)]
public class PostgresCollection : ICollectionFixture<PostgresFixture>
{
    public const string Name = "Postgres";
}
