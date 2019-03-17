using System;
using System.Data;
using Dapper.AmbientContext.Storage;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dapper.AmbientContext.Tests
{
    internal class AmbientDbContextLocatorTests
    {
        [Subject("Ambient DB Context Locator")]
        class When_ambient_database_context_stack_is_empty
        {
            Establish context = () =>
            {
#if NET452
                AmbientDbContextStorageProvider.SetStorage(new LogicalCallContextStorage());
#else
                AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());
#endif

                _ambientDbContextLocator = new AmbientDbContextLocator();
            };

            Because of = () =>
            {
                _exception = Catch.Exception(() => _ambientDbContextLocator.Get());
            };

            It should_throw_ambient_database_context_exception = () =>
            {
                _exception.ShouldBeOfExactType<InvalidOperationException>();
                _exception.Message.ShouldEqual("Could not find active ambient database context instance. Use AmbientDbContextFactory to create ambient database context before attempting to execute queries.");
            };

            Cleanup test = () =>
            {
                AmbientDbContextStorageProvider.SetStorage(null);
            };

            private static Exception _exception;
            private static IAmbientDbContextLocator _ambientDbContextLocator;
        }

        [Subject("Ambient DB Context Locator")]
        class When_ambient_database_context_stack_has_one_ambient_database_context
        {
            Establish context = () =>
            {
#if NET452
                AmbientDbContextStorageProvider.SetStorage(new LogicalCallContextStorage());
#else
                AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());
#endif

                var dbConnectionMock = new Mock<IDbConnection>();

                _expectedAmbientDbContext = new AmbientDbContext(dbConnectionMock.Object, true, true, IsolationLevel.ReadCommitted);

                _ambientDbContextLocator = new AmbientDbContextLocator();
            };

            Because of = () =>
            {
                _ambientDbContext = (AmbientDbContext)_ambientDbContextLocator.Get();
            };

            It should_return_the_expected_ambient_database_context = () =>
            {
                _ambientDbContext.ShouldNotBeNull();
                _ambientDbContext.ShouldEqual(_expectedAmbientDbContext);
            };

            Cleanup test = () =>
            {
                _expectedAmbientDbContext.Dispose();

                AmbientDbContextStorageProvider.SetStorage(null);
            };

            private static IAmbientDbContext _expectedAmbientDbContext;
            private static IAmbientDbContext _ambientDbContext;
            private static IAmbientDbContextLocator _ambientDbContextLocator;
        }

        [Subject("Ambient DB Context Locator")]
        class When_ambient_database_context_stack_has_more_than_one_ambient_database_context
        {
            Establish context = () =>
            {
#if NET452
                AmbientDbContextStorageProvider.SetStorage(new LogicalCallContextStorage());
#else
                AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());
#endif

                var dbConnectionMock = new Mock<IDbConnection>();

                _expectedAmbientDbContext1 = new AmbientDbContext(dbConnectionMock.Object, true, true, IsolationLevel.ReadCommitted);
                _expectedAmbientDbContext2 = new AmbientDbContext(dbConnectionMock.Object, true, true, IsolationLevel.ReadCommitted);

                _ambientDbContextLocator = new AmbientDbContextLocator();
            };

            Because of = () =>
            {
                _ambientDbContext = (AmbientDbContext)_ambientDbContextLocator.Get();
            };

            It should_return_the_top_ambient_database_context_in_the_stack = () =>
            {
                _ambientDbContext.ShouldNotBeNull();
                _ambientDbContext.ShouldEqual(_expectedAmbientDbContext2);
            };

            Cleanup test = () =>
            {
                _expectedAmbientDbContext2.Dispose();
                _expectedAmbientDbContext1.Dispose();

                AmbientDbContextStorageProvider.SetStorage(null);
            };

            private static IAmbientDbContext _expectedAmbientDbContext1;
            private static IAmbientDbContext _expectedAmbientDbContext2;
            private static IAmbientDbContext _ambientDbContext;
            private static IAmbientDbContextLocator _ambientDbContextLocator;
        }
    }
}