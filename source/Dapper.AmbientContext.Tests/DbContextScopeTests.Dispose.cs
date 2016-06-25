using System;
using System.Data;
using System.Linq;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dapper.AmbientContext.Tests
{
    internal partial class DbContextScopeTests
    {
        [Subject("Disposing DB Context Scope")]
        public class When_disposing_the_same_database_context_scope_more_than_once
        {
            Establish context = () =>
            {
                var dbConnectionState = ConnectionState.Closed;

                var dbConnectionMock = new Mock<IDbConnection>();
                dbConnectionMock.Setup(mock => mock.State).Returns(() => dbConnectionState);
                dbConnectionMock.Setup(mock => mock.Open()).Callback(() => dbConnectionState = ConnectionState.Open);
                dbConnectionMock.Setup(mock => mock.BeginTransaction(Moq.It.IsAny<IsolationLevel>())).Returns(() => new Mock<IDbTransaction>().Object);

                ParentDbContextScope = new DbContextScope(dbConnectionMock.Object, DbContextScopeOption.New);
                ParentDbContextScope.Open();
            };

            Because of = () =>
            {
                ParentDbContextScope.Dispose();
                Exception = Catch.Exception(() => ParentDbContextScope.Dispose());
            };

            It should_not_throw_exceptions = () =>
            {
                Exception.ShouldBeNull();
            };

            private static DbContextScope ParentDbContextScope;
            private static Exception Exception;
        }

        [Subject("Disposing DB Context Scope")]
        public class When_disposing_child_database_context_scope
        {
            Establish context = () =>
            {
                var dbConnectionState = ConnectionState.Closed;

                DbTransactionMock = new Mock<IDbTransaction>();
                DbTransactionMock.Setup(mock => mock.Dispose()).Verifiable();

                DbConnectionMock = new Mock<IDbConnection>();
                DbConnectionMock.Setup(mock => mock.State).Returns(() => dbConnectionState);
                DbConnectionMock.Setup(mock => mock.Open()).Callback(() => dbConnectionState = ConnectionState.Open);
                DbConnectionMock.Setup(mock => mock.Close()).Callback(() => dbConnectionState = ConnectionState.Closed).Verifiable();
                DbConnectionMock.Setup(mock => mock.Dispose()).Verifiable();
                DbConnectionMock.Setup(mock => mock.BeginTransaction(Moq.It.IsAny<IsolationLevel>())).Returns(() => DbTransactionMock.Object);

                ParentDbContextScope = new DbContextScope(DbConnectionMock.Object, DbContextScopeOption.New);
                ParentDbContextScope.Open();

                ChildDbContextScope = new DbContextScope(option: DbContextScopeOption.Join);
            };

            Because of = () =>
            {
                ChildDbContextScope.Dispose();
            };

            It should_not_dispose_its_parent_database_connection = () =>
            {
                DbConnectionMock.Verify(mock => mock.Close(), Times.Never);
                DbConnectionMock.Verify(mock => mock.Dispose(), Times.Never);
                ParentDbContextScope.Connection.State.ShouldEqual(ConnectionState.Open);
            };

            It should_not_dispose_its_parent_database_transaction = () =>
            {
                DbTransactionMock.Verify(mock => mock.Dispose(), Times.Never);
                ParentDbContextScope.Transaction.ShouldNotBeNull();
            };

            It should_be_removed_from_the_database_context_scope = () =>
            {
                DbContextScope.DbContextScopeStack.IsEmpty.ShouldBeFalse();
                DbContextScope.DbContextScopeStack.Count().ShouldEqual(1);
                DbContextScope.DbContextScopeStack.Peek().ShouldEqual(ParentDbContextScope);
            };

            Cleanup scopes = () =>
            {
                ParentDbContextScope.Dispose();
            };

            private static DbContextScope ParentDbContextScope;
            private static DbContextScope ChildDbContextScope;
            private static Mock<IDbTransaction> DbTransactionMock;
            private static Mock<IDbConnection> DbConnectionMock;
        }

        [Subject("Disposing DB Context Scope")]
        public class When_disposing_child_and_parent_database_context_scopes
        {
            Establish context = () =>
            {
                var dbConnectionState = ConnectionState.Closed;

                var dbConnectionMock = new Mock<IDbConnection>();
                dbConnectionMock.Setup(mock => mock.State).Returns(() => dbConnectionState);
                dbConnectionMock.Setup(mock => mock.Open()).Callback(() => dbConnectionState = ConnectionState.Open);
                dbConnectionMock.Setup(mock => mock.BeginTransaction(Moq.It.IsAny<IsolationLevel>())).Returns(() => new Mock<IDbTransaction>().Object);

                ParentDbContextScope = new DbContextScope(dbConnectionMock.Object, DbContextScopeOption.New);
                ParentDbContextScope.Open();

                ChildDbContextScope = new DbContextScope(option: DbContextScopeOption.Join);
            };

            Because of = () =>
            {
                ChildDbContextScope.Dispose();
                ParentDbContextScope.Dispose();
            };

            It should_dispose_parent_database_connection = () =>
            {
                ParentDbContextScope.Connection.ShouldBeNull();
            };

            It should_dispose_parent_database_transaction = () =>
            {
                ParentDbContextScope.Transaction.ShouldBeNull();
            };

            It should_be_removed_from_the_database_context_scope = () =>
            {
                DbContextScope.DbContextScopeStack.IsEmpty.ShouldBeTrue();
            };

            private static DbContextScope ParentDbContextScope;
            private static DbContextScope ChildDbContextScope;
        }
    }
}
