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
        [Subject("Create DB Context Scope")]
        public class When_creating_new_database_context_scope
        {
            Establish context = () =>
            {
                DbConnectionMock = new Mock<IDbConnection>();
            };

            Because of = () =>
            {
                DbContextScope = new DbContextScope(DbConnectionMock.Object, DbContextScopeOption.New, IsolationLevel.ReadCommitted);
            };

            It should_be_added_to_the_top_of_the_database_context_scope_stack = () =>
            {
                DbContextScope.DbContextScopeStack.IsEmpty.ShouldBeFalse();
                DbContextScope.DbContextScopeStack.Peek().ShouldEqual(DbContextScope);
            };

            It should_not_have_a_parent_scope_reference = () =>
            {
                DbContextScope.Parent.ShouldBeNull();
            };

            It should_have_the_new_option = () =>
            {
                DbContextScope.Option.ShouldEqual(DbContextScopeOption.New);
            };

            It should_have_read_committed_isolation_level = () =>
            {
                DbContextScope.IsolationLevel.ShouldEqual(IsolationLevel.ReadCommitted);
            };

            It should_have_a_closed_database_connection = () =>
            {
                DbContextScope.Connection.ShouldEqual(DbConnectionMock.Object);
                DbContextScope.Connection.State.ShouldEqual(ConnectionState.Closed);
            };

            It should_not_have_a_transaction = () =>
            {
                DbContextScope.Transaction.ShouldBeNull();  
            };

            Cleanup scopes = () =>
            {
                DbContextScope.Dispose();  
            };

            private static DbContextScope DbContextScope;
            private static Mock<IDbConnection> DbConnectionMock;
        }

        [Subject("Create DB Context Scope")]
        public class When_creating_and_opening_new_database_context_scope
        {
            Establish context = () =>
            {
                var dbConnectionState = ConnectionState.Closed;

                DbConnectionMock = new Mock<IDbConnection>();
                DbConnectionMock.Setup(mock => mock.State).Returns(() => dbConnectionState);
                DbConnectionMock.Setup(mock => mock.Open()).Callback(() => dbConnectionState = ConnectionState.Open);
                DbConnectionMock.Setup(mock => mock.BeginTransaction(Moq.It.IsAny<IsolationLevel>())).Returns(() => new Mock<IDbTransaction>().Object);

                DbContextScope = new DbContextScope(DbConnectionMock.Object, DbContextScopeOption.New, IsolationLevel.ReadCommitted);
            };

            Because of = () =>
            {
                DbContextScope.Open();
            };

            It should_be_added_to_the_top_of_the_database_context_scope_stack = () =>
            {
                DbContextScope.DbContextScopeStack.IsEmpty.ShouldBeFalse();
                DbContextScope.DbContextScopeStack.Peek().ShouldEqual(DbContextScope);
            };

            It should_not_have_a_parent_scope_reference = () =>
            {
                DbContextScope.Parent.ShouldBeNull();
            };

            It should_have_the_new_option = () =>
            {
                DbContextScope.Option.ShouldEqual(DbContextScopeOption.New);
            };

            It should_have_read_committed_isolation_level = () =>
            {
                DbContextScope.IsolationLevel.ShouldEqual(IsolationLevel.ReadCommitted);
            };

            It should_have_an_open_database_connection = () =>
            {
                DbContextScope.Connection.ShouldEqual(DbConnectionMock.Object);
                DbContextScope.Connection.State.ShouldEqual(ConnectionState.Open);
            };

            It should_have_a_transaction = () =>
            {
                DbContextScope.Transaction.ShouldNotBeNull();
            };

            Cleanup scopes = () =>
            {
                DbContextScope.Dispose();
            };

            private static DbContextScope DbContextScope;
            private static Mock<IDbConnection> DbConnectionMock;
        }

        [Subject("Create DB Context Scope")]
        public class When_creating_database_context_scope_with_the_join_option_but_no_parent_scope_exists
        {
            Because of = () =>
            {
                Exception = Catch.Exception(() => new DbContextScope(option: DbContextScopeOption.Join));
            };

            It should_throw_the_expected_exception = () =>
            {
                Exception.ShouldBeOfExactType<InvalidOperationException>();
                Exception.Message.ShouldEqual("Could not find available database context scope to join.");
            };

            It should_not_be_added_to_the_database_context_scope_stack = () =>
            {
                DbContextScope.DbContextScopeStack.IsEmpty.ShouldBeTrue();
            };

            private static Exception Exception;
        }

        [Subject("Create DB Context Scope")]
        public class When_creating_new_database_context_scope_without_a_database_connection
        {
            Because of = () =>
            {
                Exception = Catch.Exception(() => new DbContextScope(null, DbContextScopeOption.New));
            };

            It should_throw_the_expected_exception = () =>
            {
                Exception.ShouldBeOfExactType<InvalidOperationException>();
                Exception.Message.ShouldEqual("Must specify a database connection when creating new database context scope.");
            };

            It should_not_be_added_to_the_database_context_scope_stack = () =>
            {
                DbContextScope.DbContextScopeStack.IsEmpty.ShouldBeTrue();
            };

            private static Exception Exception;
        }

        [Subject("Create DB Context Scope")]
        public class When_creating_a_database_context_scope_with_both_new_and_join_options
        {
            Establish context = () =>
            {
                DbConnectionMock = new Mock<IDbConnection>();
            };

            Because of = () =>
            {
                Exception = Catch.Exception(() => new DbContextScope(DbConnectionMock.Object, DbContextScopeOption.New | DbContextScopeOption.Join));
            };

            It should_throw_the_expected_exception = () =>
            {
                Exception.ShouldBeOfExactType<InvalidOperationException>();
                Exception.Message.ShouldEqual("The database context scope option should be set to either new or join.");
            };

            It should_not_be_added_to_the_database_context_scope_stack = () =>
            {
                DbContextScope.DbContextScopeStack.IsEmpty.ShouldBeTrue();
            };

            private static Exception Exception;
            private static Mock<IDbConnection> DbConnectionMock;
        }

        [Subject("Create DB Context Scope")]
        public class When_joining_to_a_parent_database_context_scope
        {
            Establish context = () =>
            {
                DatabaseTransactionMock = new Mock<IDbTransaction>();

                var dbConnectionState = ConnectionState.Closed;

                var dbConnectionMock = new Mock<IDbConnection>();
                dbConnectionMock.Setup(mock => mock.State).Returns(() => dbConnectionState);
                dbConnectionMock.Setup(mock => mock.Open()).Callback(() => dbConnectionState = ConnectionState.Open);
                dbConnectionMock.Setup(mock => mock.BeginTransaction(Moq.It.IsAny<IsolationLevel>())).Returns(() => DatabaseTransactionMock.Object);

                ParentDbContextScope = new DbContextScope(dbConnectionMock.Object, DbContextScopeOption.New);
                ParentDbContextScope.Open();
            };

            Because of = () =>
            {
                ChildDbContextScope = new DbContextScope(option: DbContextScopeOption.Join);
            };

            It should_push_the_parent_database_context_scope_to_the_bottom_of_the_stack = () =>
            {
                DbContextScope.DbContextScopeStack.ElementAt(1).ShouldEqual(ParentDbContextScope);
            };

            It should_be_added_to_the_top_of_the_database_context_scopes_stack = () =>
            {
                DbContextScope.DbContextScopeStack.IsEmpty.ShouldBeFalse();
                DbContextScope.DbContextScopeStack.Count().ShouldEqual(2);
                DbContextScope.DbContextScopeStack.Peek().ShouldEqual(ChildDbContextScope);
            };

            It should_reference_the_existing_parent_scope = () =>
            {
                ChildDbContextScope.Parent.ShouldEqual(ParentDbContextScope);
            };

            It should_inherit_scope_option_from_parent_scope = () =>
            {
                ChildDbContextScope.Option.ShouldEqual(DbContextScopeOption.New);
            };

            It should_inherit_isolation_level_from_parent_scope = () =>
            {
                ChildDbContextScope.IsolationLevel.ShouldEqual(IsolationLevel.ReadCommitted);
            };

            It should_inherit_database_connection_from_parent_scope = () =>
            {
                ChildDbContextScope.Connection.ShouldEqual(ParentDbContextScope.Connection);
                ChildDbContextScope.Connection.State.ShouldEqual(ConnectionState.Open);
            };

            It should_inherit_database_transaction_from_parent_scope = () =>
            {
                ChildDbContextScope.Transaction.ShouldEqual(DatabaseTransactionMock.Object);
            };

            Cleanup scopes = () =>
            {
                ChildDbContextScope.Dispose();
                ParentDbContextScope.Dispose();
            };

            private static DbContextScope ParentDbContextScope;
            private static DbContextScope ChildDbContextScope;
            private static Mock<IDbTransaction> DatabaseTransactionMock;
        }

        [Subject("Create DB Context Scope")]
        public class When_opening_parent_database_context_scope_with_the_suppress_option
        {
            Establish context = () =>
            {
                DatabaseTransactionMock = new Mock<IDbTransaction>();
                var dbConnectionState = ConnectionState.Closed;

                var dbConnectionMock = new Mock<IDbConnection>();
                dbConnectionMock.Setup(mock => mock.State).Returns(() => dbConnectionState);
                dbConnectionMock.Setup(mock => mock.Open()).Callback(() => dbConnectionState = ConnectionState.Open);
                dbConnectionMock.Setup(mock => mock.BeginTransaction(Moq.It.IsAny<IsolationLevel>())).Returns(() => DatabaseTransactionMock.Object);

                ParentDbContextScope = new DbContextScope(dbConnectionMock.Object, DbContextScopeOption.New | DbContextScopeOption.Suppress);
            };

            Because of = () =>
            {
                ParentDbContextScope.Open();
            };

            It should_create_a_database_connection = () =>
            {
                ParentDbContextScope.Connection.ShouldNotBeNull();
                ParentDbContextScope.Connection.State.ShouldEqual(ConnectionState.Open);
            };

            It should_not_create_a_database_transaction = () =>
            {
                ParentDbContextScope.Transaction.ShouldBeNull();
            };

            Cleanup scopes = () =>
            {
                ParentDbContextScope.Dispose();
            };

            private static DbContextScope ParentDbContextScope;
            private static Mock<IDbTransaction> DatabaseTransactionMock;
        }
    }
}