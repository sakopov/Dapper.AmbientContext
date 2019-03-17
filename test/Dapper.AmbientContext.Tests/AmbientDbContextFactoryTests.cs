using System;
using System.Data;
using Dapper.AmbientContext.Storage;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dapper.AmbientContext.Tests
{
    internal class AmbientDbContextFactoryTests
    {
        [Subject("Ambient DB Context Factory")]
        class When_calling_create_with_none_closed_database_connection
        {
            Establish context = () =>
            {
#if NET452
                AmbientDbContextStorageProvider.SetStorage(new LogicalCallContextStorage());
#else
                AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());
#endif

                var dbConnectionState = ConnectionState.Open;

                var dbConnectionMock = new Mock<IDbConnection>();
                dbConnectionMock.Setup(mock => mock.State).Returns(() => dbConnectionState);

                var dbConnectionFactoryMock = new Mock<IDbConnectionFactory>();
                dbConnectionFactoryMock.Setup(mock => mock.Create()).Returns(dbConnectionMock.Object);

                _ambientDatabaseContextFactory = new AmbientDbContextFactory(dbConnectionFactoryMock.Object);
            };

            Because of = () =>
            {
                _exception = Catch.Exception(() => _ambientDatabaseContextFactory.Create());
            };

            It should_throw_ambient_database_context_exception = () =>
            {
                _exception.ShouldBeOfExactType<AmbientDbContextException>();
                _exception.Message.ShouldEqual("The database connection factory returned a database connection in a non-closed state. This behavior is not allowed as the ambient database context will maintain database connection state as required.");
            };

            Cleanup test = () =>
            {
                AmbientDbContextStorageProvider.SetStorage(null);
            };

            private static Exception _exception;
            private static IAmbientDbContextFactory _ambientDatabaseContextFactory;
        }

        [Subject("Ambient DB Context Factory")]
        class When_calling_create_with_closed_database_connection
        {
            Establish context = () =>
            {
#if NET452
                AmbientDbContextStorageProvider.SetStorage(new LogicalCallContextStorage());
#else
                AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());
#endif

                _dbConnectionMock = new Mock<IDbConnection>();
                _dbConnectionMock.Setup(mock => mock.State).Returns(() => ConnectionState.Closed);

                var dbConnectionFactoryMock = new Mock<IDbConnectionFactory>();
                dbConnectionFactoryMock.Setup(mock => mock.Create()).Returns(_dbConnectionMock.Object);

                _ambientDatabaseContextFactory = new AmbientDbContextFactory(dbConnectionFactoryMock.Object);
            };

            Because of = () =>
            {
                _ambientDbContext = _ambientDatabaseContextFactory.Create();
            };

            It should_return_the_expected_ambient_database_context = () =>
            {
                _ambientDbContext.ShouldNotBeNull();
                _ambientDbContext.Connection.ShouldEqual(_dbConnectionMock.Object);
                _ambientDbContext.Connection.State.ShouldEqual(ConnectionState.Closed);
            };

            Cleanup test = () =>
            {
                AmbientDbContextStorageProvider.SetStorage(null);

                _ambientDbContext.Dispose();
            };

            private static IAmbientDbContext _ambientDbContext;
            private static IAmbientDbContextFactory _ambientDatabaseContextFactory;
            private static Mock<IDbConnection> _dbConnectionMock;
        }
    }
}