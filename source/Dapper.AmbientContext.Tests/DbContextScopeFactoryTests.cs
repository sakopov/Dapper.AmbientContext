using System.Data;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dapper.AmbientContext.Tests
{
    internal class DbContextScopeFactoryTests
    {
        [Subject("Create New DB Context Scope From Factory")]
        public class When_creating_new_database_context_scope_with_default_arguments
        {
            Establish context = () =>
            {
                var dbConnectionState = ConnectionState.Closed;

                var dbConnectionMock = new Mock<IDbConnection>();
                dbConnectionMock.Setup(mock => mock.State).Returns(() => dbConnectionState);
                dbConnectionMock.Setup(mock => mock.Open()).Callback(() => dbConnectionState = ConnectionState.Open);
                dbConnectionMock.Setup(mock => mock.BeginTransaction(Moq.It.IsAny<IsolationLevel>())).Returns(() => new Mock<IDbTransaction>().Object);

                var dbConnectionFactoryMock = new Mock<IDbConnectionFactory>();
                dbConnectionFactoryMock.Setup(mock => mock.Create()).Returns(dbConnectionMock.Object);

                DbContextScopeFactory = new DbContextScopeFactory(dbConnectionFactoryMock.Object);
            };

            Because of = () =>
            {
                DbContextScope = (DbContextScope)DbContextScopeFactory.Create();
            };

            It should_create_database_context_scope_instance = () =>
            {
                DbContextScope.ShouldNotBeNull();
            };

            It should_open_database_connection_in_database_context_scope = () =>
            {
                DbContextScope.Connection.ShouldNotBeNull();
                DbContextScope.Connection.State.ShouldEqual(ConnectionState.Open);
            };

            It should_begin_database_transaction_in_database_context_scope = () =>
            {
                DbContextScope.Transaction.ShouldNotBeNull();  
            };

            It should_create_database_context_scope_with_expected_isolation_level = () =>
            {
                DbContextScope.IsolationLevel.ShouldEqual(IsolationLevel.ReadCommitted);
            };

            It should_create_database_context_scope_with_the_expected_options = () =>
            {
                DbContextScope.Option.ShouldEqual(DbContextScopeOption.New);
            };

            Cleanup scopes = () =>
            {
                DbContextScope.Dispose();
            };

            private static DbContextScopeFactory DbContextScopeFactory;
            private static DbContextScope DbContextScope;
        }

        [Subject("Create New DB Context Scope From Factory")]
        public class When_creating_new_database_context_scope_with_database_transaction_suppression
        {
            Establish context = () =>
            {
                var dbConnectionState = ConnectionState.Closed;

                var dbConnectionMock = new Mock<IDbConnection>();
                dbConnectionMock.Setup(mock => mock.State).Returns(() => dbConnectionState);
                dbConnectionMock.Setup(mock => mock.Open()).Callback(() => dbConnectionState = ConnectionState.Open);
                dbConnectionMock.Setup(mock => mock.BeginTransaction(Moq.It.IsAny<IsolationLevel>())).Returns(() => new Mock<IDbTransaction>().Object);

                var dbConnectionFactoryMock = new Mock<IDbConnectionFactory>();
                dbConnectionFactoryMock.Setup(mock => mock.Create()).Returns(dbConnectionMock.Object);

                DbContextScopeFactory = new DbContextScopeFactory(dbConnectionFactoryMock.Object);
            };

            Because of = () =>
            {
                DbContextScope = (DbContextScope)DbContextScopeFactory.Create(suppress: true);
            };

            It should_create_database_context_scope_instance = () =>
            {
                DbContextScope.ShouldNotBeNull();
            };

            It should_open_database_connection_in_database_context_scope = () =>
            {
                DbContextScope.Connection.ShouldNotBeNull();
                DbContextScope.Connection.State.ShouldEqual(ConnectionState.Open);
            };

            It should_begin_database_transaction_in_database_context_scope = () =>
            {
                DbContextScope.Transaction.ShouldBeNull();
            };

            It should_create_database_context_scope_with_expected_isolation_level = () =>
            {
                DbContextScope.IsolationLevel.ShouldEqual(IsolationLevel.ReadCommitted);
            };

            It should_create_database_context_scope_with_the_expected_options = () =>
            {
                DbContextScope.Option.ShouldEqual(DbContextScopeOption.New | DbContextScopeOption.Suppress);
            };

            Cleanup scopes = () =>
            {
                DbContextScope.Dispose();
            };

            private static DbContextScopeFactory DbContextScopeFactory;
            private static DbContextScope DbContextScope;
        }

        [Subject("Create New DB Context Scope From Factory")]
        public class When_creating_new_database_context_scope_with_custom_transaction_isolation_level
        {
            Establish context = () =>
            {
                var dbConnectionState = ConnectionState.Closed;

                var dbConnectionMock = new Mock<IDbConnection>();
                dbConnectionMock.Setup(mock => mock.State).Returns(() => dbConnectionState);
                dbConnectionMock.Setup(mock => mock.Open()).Callback(() => dbConnectionState = ConnectionState.Open);
                dbConnectionMock.Setup(mock => mock.BeginTransaction(Moq.It.IsAny<IsolationLevel>())).Returns(() => new Mock<IDbTransaction>().Object);

                var dbConnectionFactoryMock = new Mock<IDbConnectionFactory>();
                dbConnectionFactoryMock.Setup(mock => mock.Create()).Returns(dbConnectionMock.Object);

                DbContextScopeFactory = new DbContextScopeFactory(dbConnectionFactoryMock.Object);
            };

            Because of = () =>
            {
                DbContextScope = (DbContextScope)DbContextScopeFactory.Create(isolationLevel: IsolationLevel.ReadUncommitted);
            };

            It should_create_database_context_scope_instance = () =>
            {
                DbContextScope.ShouldNotBeNull();
            };

            It should_open_database_connection_in_database_context_scope = () =>
            {
                DbContextScope.Connection.ShouldNotBeNull();
                DbContextScope.Connection.State.ShouldEqual(ConnectionState.Open);
            };

            It should_begin_database_transaction_in_database_context_scope = () =>
            {
                DbContextScope.Transaction.ShouldNotBeNull();
            };

            It should_create_database_context_scope_with_expected_isolation_level = () =>
            {
                DbContextScope.IsolationLevel.ShouldEqual(IsolationLevel.ReadUncommitted);
            };

            It should_create_database_context_scope_with_the_expected_options = () =>
            {
                DbContextScope.Option.ShouldEqual(DbContextScopeOption.New);
            };

            Cleanup scopes = () =>
            {
                DbContextScope.Dispose();
            };

            private static DbContextScopeFactory DbContextScopeFactory;
            private static DbContextScope DbContextScope;
        }

        [Subject("Create New DB Context Scope From Factory")]
        public class When_creating_joined_database_context_scope_without_active_parent_scope
        {
            Establish context = () =>
            {
                var dbConnectionState = ConnectionState.Closed;

                var dbConnectionMock = new Mock<IDbConnection>();
                dbConnectionMock.Setup(mock => mock.State).Returns(() => dbConnectionState);
                dbConnectionMock.Setup(mock => mock.Open()).Callback(() => dbConnectionState = ConnectionState.Open);
                dbConnectionMock.Setup(mock => mock.BeginTransaction(Moq.It.IsAny<IsolationLevel>())).Returns(() => new Mock<IDbTransaction>().Object);

                var dbConnectionFactoryMock = new Mock<IDbConnectionFactory>();
                dbConnectionFactoryMock.Setup(mock => mock.Create()).Returns(dbConnectionMock.Object);

                DbContextScopeFactory = new DbContextScopeFactory(dbConnectionFactoryMock.Object);
            };

            Because of = () =>
            {
                DbContextScope = (DbContextScope)DbContextScopeFactory.CreateOrJoin();
            };

            It should_create_database_context_scope_instance = () =>
            {
                DbContextScope.ShouldNotBeNull();
            };

            It should_open_database_connection_in_database_context_scope = () =>
            {
                DbContextScope.Connection.ShouldNotBeNull();
                DbContextScope.Connection.State.ShouldEqual(ConnectionState.Open);
            };

            It should_begin_database_transaction_in_database_context_scope = () =>
            {
                DbContextScope.Transaction.ShouldNotBeNull();
            };

            It should_create_database_context_scope_with_expected_isolation_level = () =>
            {
                DbContextScope.IsolationLevel.ShouldEqual(IsolationLevel.ReadCommitted);
            };

            It should_create_database_context_scope_with_the_expected_options = () =>
            {
                DbContextScope.Option.ShouldEqual(DbContextScopeOption.New);
            };

            Cleanup scopes = () =>
            {
                DbContextScope.Dispose();
            };

            private static DbContextScopeFactory DbContextScopeFactory;
            private static DbContextScope DbContextScope;
        }

        [Subject("Create New DB Context Scope From Factory")]
        public class When_creating_joined_database_context_scope_with_active_parent_scope
        {
            Establish context = () =>
            {
                var dbConnectionState = ConnectionState.Closed;

                var dbConnectionMock = new Mock<IDbConnection>();
                dbConnectionMock.Setup(mock => mock.State).Returns(() => dbConnectionState);
                dbConnectionMock.Setup(mock => mock.Open()).Callback(() => dbConnectionState = ConnectionState.Open);
                dbConnectionMock.Setup(mock => mock.BeginTransaction(Moq.It.IsAny<IsolationLevel>())).Returns(() => new Mock<IDbTransaction>().Object);

                var dbConnectionFactoryMock = new Mock<IDbConnectionFactory>();
                dbConnectionFactoryMock.Setup(mock => mock.Create()).Returns(dbConnectionMock.Object);

                DbContextScopeFactory = new DbContextScopeFactory(dbConnectionFactoryMock.Object);

                ParentDbContextScope = (DbContextScope)DbContextScopeFactory.Create();
            };

            Because of = () =>
            {
                ChildDbContextScope = (DbContextScope)DbContextScopeFactory.CreateOrJoin();
            };

            It should_create_database_context_scope_instance = () =>
            {
                ChildDbContextScope.ShouldNotBeNull();
            };

            It should_return_database_context_scope_with_reference_to_parent_scope = () =>
            {
                ChildDbContextScope.Parent.ShouldEqual(ParentDbContextScope);
            };

            It should_return_database_context_scope_with_inherited_database_connection_from_parent_scope = () =>
            {
                ChildDbContextScope.Connection.ShouldNotBeNull();
                ChildDbContextScope.Connection.ShouldEqual(ParentDbContextScope.Connection);
                ChildDbContextScope.Connection.State.ShouldEqual(ConnectionState.Open);
            };

            It should_return_database_context_scope_with_inherited_database_transaction_from_parent_scope = () =>
            {
                ChildDbContextScope.Transaction.ShouldEqual(ParentDbContextScope.Transaction);
            };

            It should_return_database_context_scope_with_inherited_isolation_level_from_parent_scope = () =>
            {
                ChildDbContextScope.IsolationLevel.ShouldEqual(ParentDbContextScope.IsolationLevel);
            };

            It should_return_database_context_scope_with_inherited_options_from_parent_scope = () =>
            {
                ChildDbContextScope.Option.ShouldEqual(ParentDbContextScope.Option);
            };

            Cleanup scopes = () =>
            {
                ChildDbContextScope.Dispose();
                ParentDbContextScope.Dispose();
            };

            private static DbContextScopeFactory DbContextScopeFactory;
            private static DbContextScope ChildDbContextScope;
            private static DbContextScope ParentDbContextScope;
        }
    }
}