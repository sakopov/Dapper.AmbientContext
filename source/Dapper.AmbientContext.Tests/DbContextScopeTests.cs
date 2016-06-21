using System;
using System.Data;
using System.Linq;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dapper.AmbientContext.Tests
{
    internal class DbContextScopeTests
    {
        [Subject("Db Context Scope With Closed Connection")]
        public class When_instantiating_first_database_context_scope_with_new_option
        {
            Establish context = () =>
            {
                DbConnectionMock = new Mock<IDbConnection>();
            };

            Because of = () =>
            {
                DbContextScope = new DbContextScope(DbConnectionMock.Object, DbContextScopeOption.New);
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

        [Subject("Db Context Scope With Closed Connection")]
        public class When_instantiating_first_database_context_scope_with_join_option
        {
            Because of = () =>
            {
                Exception = Catch.Exception(() => new DbContextScope(option: DbContextScopeOption.Join));
            };

            It should_not_be_added_to_the_database_context_scope_stack = () =>
            {
                DbContextScope.DbContextScopeStack.IsEmpty.ShouldBeTrue();
            };

            It should_throw_the_expected_exception = () =>
            {
                Exception.ShouldBeOfExactType<InvalidOperationException>();
                Exception.Message.ShouldEqual("Could not find available database context scope to join.");
            };

            private static Exception Exception;
        }

        [Subject("Db Context Scope With Closed Connection")]
        public class When_instantiating_database_context_scope_with_new_option_and_without_database_connection
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

        [Subject("Db Context Scope With Closed Connection")]
        public class When_instantiating_database_context_scope_with_new_and_join_options
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

        [Subject("Db Context Scope With Closed Connection")]
        public class When_joining_to_an_existing_database_context_scope
        {
            Establish context = () =>
            {
                var dbConnectionMock = new Mock<IDbConnection>();

                ExistingDbContextScope = new DbContextScope(dbConnectionMock.Object, DbContextScopeOption.New);
            };

            Because of = () =>
            {
                JoinedDbContextScope = new DbContextScope(option: DbContextScopeOption.Join);
            };

            It should_push_the_existing_database_context_scope_to_the_bottom_of_the_stack = () =>
            {
                DbContextScope.DbContextScopeStack.ElementAt(1).ShouldEqual(ExistingDbContextScope);
            };

            It should_be_added_to_the_database_context_scopes_stack = () =>
            {
                DbContextScope.DbContextScopeStack.IsEmpty.ShouldBeFalse();
                DbContextScope.DbContextScopeStack.Count().ShouldEqual(2);
            };

            It should_be_added_to_the_top_of_the_database_context_scopes_stack = () =>
            {
                DbContextScope.DbContextScopeStack.Peek().ShouldEqual(JoinedDbContextScope);
            };

            It should_reference_the_existing_parent_scope = () =>
            {
                JoinedDbContextScope.Parent.ShouldEqual(ExistingDbContextScope);
            };

            It should_inherit_scope_option_from_parent_scope = () =>
            {
                JoinedDbContextScope.Option.ShouldEqual(DbContextScopeOption.New);
            };

            It should_inherit_isolation_level_level_from_parent_scope = () =>
            {
                JoinedDbContextScope.IsolationLevel.ShouldEqual(IsolationLevel.ReadCommitted);
            };

            It should_inherit_database_connection_from_parent_scope = () =>
            {
                JoinedDbContextScope.Connection.ShouldEqual(ExistingDbContextScope.Connection);
                JoinedDbContextScope.Connection.State.ShouldEqual(ConnectionState.Closed);
            };

            It should_not_have_a_transaction = () =>
            {
                JoinedDbContextScope.Transaction.ShouldBeNull();
            };

            Cleanup scopes = () =>
            {
                JoinedDbContextScope.Dispose();
                ExistingDbContextScope.Dispose();
            };

            private static DbContextScope ExistingDbContextScope;
            private static DbContextScope JoinedDbContextScope;
        }
    }
}
