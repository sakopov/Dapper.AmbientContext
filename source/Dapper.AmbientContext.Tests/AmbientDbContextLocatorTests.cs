using System;
using System.Data;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dapper.AmbientContext.Tests
{
    internal class AmbientDbContextLocatorTests
    {
        [Subject("Ambient DB Context Locator")]
        public class When_database_context_scope_stack_has_one_database_context_scope
        {
            Establish context = () =>
            {
                var dbConnectionState = ConnectionState.Closed;

                var dbConnectionMock = new Mock<IDbConnection>();
                dbConnectionMock.Setup(mock => mock.State).Returns(() => dbConnectionState);
                dbConnectionMock.Setup(mock => mock.Open()).Callback(() => dbConnectionState = ConnectionState.Open);
                dbConnectionMock.Setup(mock => mock.BeginTransaction(Moq.It.IsAny<IsolationLevel>())).Returns(() => new Mock<IDbTransaction>().Object);

                ExpectedDbContextScope = new DbContextScope(dbConnectionMock.Object, DbContextScopeOption.New);
                ExpectedDbContextScope.Open();

                DbContextLocator = new AmbientDbContextLocator();
            };

            Because of = () =>
            {
                DbContext = (DbContext)DbContextLocator.Get();
            };

            It should_return_the_database_context = () =>
            {
                DbContext.ShouldNotBeNull();
            };

            Cleanup scopes = () =>
            {
                ExpectedDbContextScope.Dispose();
            };

            private static DbContextScope ExpectedDbContextScope;
            private static DbContext DbContext;
            private static AmbientDbContextLocator DbContextLocator;
        }

        [Subject("Ambient DB Context Locator")]
        public class When_database_context_scope_stack_does_not_have_database_context_scopes
        {
            Establish context = () =>
            {
                DbContextLocator = new AmbientDbContextLocator();
            };

            Because of = () =>
            {
                Exception = Catch.Exception(() => DbContextLocator.Get());
            };

            It should_throw_the_expected_exception = () =>
            {
                Exception.ShouldBeOfExactType<InvalidOperationException>();
                Exception.Message.ShouldEqual("Cannot find active database context scope. Make sure a context scope is created before attempting to run a query.");
            };

            private static Exception Exception;
            private static AmbientDbContextLocator DbContextLocator;
        }
    }
}
