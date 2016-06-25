using System;
using System.Data;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dapper.AmbientContext.Tests
{
    internal partial class DbContextScopeTests
    {
        [Subject("Commit DB Context Scope")]
        public class When_committing_child_database_context_scope
        {
            Establish context = () =>
            {
                var dbConnectionState = ConnectionState.Closed;

                DbTransactionMock = new Mock<IDbTransaction>();
                DbTransactionMock.Setup(mock => mock.Commit()).Verifiable();
                DbTransactionMock.Setup(mock => mock.Dispose()).Verifiable();
                DbTransactionMock.Setup(mock => mock.Rollback()).Verifiable();

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
                ChildDbContextScope.Commit();
            };

            It should_not_commit_dispose_or_rollback_its_parent_database_transaction = () =>
            {
                DbTransactionMock.Verify(mock => mock.Commit(), Times.Never);
                DbTransactionMock.Verify(mock => mock.Dispose(), Times.Never);
                DbTransactionMock.Verify(mock => mock.Rollback(), Times.Never);
                ParentDbContextScope.Transaction.ShouldNotBeNull();
            };

            Cleanup scopes = () =>
            {
                ChildDbContextScope.Dispose();
                ParentDbContextScope.Dispose();
            };

            private static DbContextScope ParentDbContextScope;
            private static DbContextScope ChildDbContextScope;
            private static Mock<IDbTransaction> DbTransactionMock;
        }

        [Subject("Commit DB Context Scope")]
        public class When_committing_parent_database_context_scope
        {
            Establish context = () =>
            {
                var dbConnectionState = ConnectionState.Closed;

                DbTransactionMock = new Mock<IDbTransaction>();
                DbTransactionMock.Setup(mock => mock.Commit()).Verifiable();
                DbTransactionMock.Setup(mock => mock.Dispose()).Verifiable();

                var dbConnectionMock = new Mock<IDbConnection>();
                dbConnectionMock.Setup(mock => mock.State).Returns(() => dbConnectionState);
                dbConnectionMock.Setup(mock => mock.Open()).Callback(() => dbConnectionState = ConnectionState.Open);
                dbConnectionMock.Setup(mock => mock.BeginTransaction(Moq.It.IsAny<IsolationLevel>())).Returns(() => DbTransactionMock.Object);

                ParentDbContextScope = new DbContextScope(dbConnectionMock.Object, DbContextScopeOption.New);
                ParentDbContextScope.Open();

                ChildDbContextScope = new DbContextScope(option: DbContextScopeOption.Join);
            };

            Because of = () =>
            {
                ParentDbContextScope.Commit();
            };

            It should_dispose_parent_database_transaction = () =>
            {
                DbTransactionMock.Verify(mock => mock.Commit(), Times.Once);
                DbTransactionMock.Verify(mock => mock.Dispose(), Times.Once);
                ParentDbContextScope.Transaction.ShouldBeNull();
            };

            Cleanup scopes = () =>
            {
                ChildDbContextScope.Dispose();
                ParentDbContextScope.Dispose();
            };

            private static DbContextScope ParentDbContextScope;
            private static DbContextScope ChildDbContextScope;
            private static Mock<IDbTransaction> DbTransactionMock;
        }

        [Subject("Commit DB Context Scope")]
        public class When_committing_parent_database_context_scope_fails
        {
            Establish context = () =>
            {
                var dbConnectionState = ConnectionState.Closed;

                DbTransactionMock = new Mock<IDbTransaction>();
                DbTransactionMock.Setup(mock => mock.Commit()).Throws<Exception>();
                DbTransactionMock.Setup(mock => mock.Dispose()).Verifiable();
                DbTransactionMock.Setup(mock => mock.Rollback()).Verifiable();

                var dbConnectionMock = new Mock<IDbConnection>();
                dbConnectionMock.Setup(mock => mock.State).Returns(() => dbConnectionState);
                dbConnectionMock.Setup(mock => mock.Open()).Callback(() => dbConnectionState = ConnectionState.Open);
                dbConnectionMock.Setup(mock => mock.BeginTransaction(Moq.It.IsAny<IsolationLevel>())).Returns(() => DbTransactionMock.Object);
                DbTransactionMock.Setup(mock => mock.Connection).Returns(() => dbConnectionMock.Object);

                ParentDbContextScope = new DbContextScope(dbConnectionMock.Object, DbContextScopeOption.New);
                ParentDbContextScope.Open();

                ChildDbContextScope = new DbContextScope(option: DbContextScopeOption.Join);
            };

            Because of = () =>
            {
                Exception = Catch.Exception(() => ParentDbContextScope.Commit());
            };

            It should_throw_exception = () =>
            {
                Exception.ShouldNotBeNull();  
            };

            It should_rollback_and_dispose_parent_database_transaction = () =>
            {
                DbTransactionMock.Verify(mock => mock.Dispose(), Times.Once);
                DbTransactionMock.Verify(mock => mock.Rollback(), Times.Once);
                ParentDbContextScope.Transaction.ShouldBeNull();
            };

            Cleanup scopes = () =>
            {
                ChildDbContextScope.Dispose();
                ParentDbContextScope.Dispose();
            };

            private static DbContextScope ParentDbContextScope;
            private static DbContextScope ChildDbContextScope;
            private static Mock<IDbTransaction> DbTransactionMock;
            private static Exception Exception;
        }
    }
}