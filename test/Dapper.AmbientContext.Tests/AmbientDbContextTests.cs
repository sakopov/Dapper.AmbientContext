using System;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using Dapper.AmbientContext.Storage;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dapper.AmbientContext.Tests
{
    internal class AmbientDbContextTests
    {
        [Subject("Ambient DB Context Initialization")]
        class When_creating_first_ambient_database_context_with_transaction_suppression_and_read_committed_isolation_level
        {
            Establish context = () =>
            {
#if NET45
                AmbientDbContextStorageProvider.SetStorage(new LogicalCallContextStorage());
#else
                AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());
#endif

                _storageHelper = new ContextualStorageHelper(AmbientDbContextStorageProvider.Storage);

                _dbConnectionMock = new Mock<IDbConnection>();
                _dbConnectionMock.Setup(mock => mock.State).Returns(() => ConnectionState.Closed);
            };

            Because of = () =>
            {
                _ambientDbContext = new AmbientDbContext(_dbConnectionMock.Object, true, true, IsolationLevel.ReadCommitted);

                _stack = _storageHelper.GetStack();
            };

            It should_create_the_expected_ambient_database_context = () =>
            {
                _ambientDbContext.ShouldNotBeNull();
                _ambientDbContext.Parent.ShouldBeNull();
                _ambientDbContext.Connection.ShouldEqual(_dbConnectionMock.Object);
                _ambientDbContext.Connection.State.ShouldEqual(ConnectionState.Closed);
                _ambientDbContext.Transaction.ShouldBeNull();
            };

            It should_add_ambient_database_context_to_the_stack = () =>
            {
                _stack.IsEmpty.ShouldBeFalse();
                _stack.Count().ShouldEqual(1);
                _stack.Peek().ShouldEqual(_ambientDbContext);
            };

            Cleanup test = () =>
            {
                AmbientDbContextStorageProvider.SetStorage(null);

                _ambientDbContext.Dispose();
            };

            private static Mock<IDbConnection> _dbConnectionMock;
            private static AmbientDbContext _ambientDbContext;
            private static ContextualStorageHelper _storageHelper;
            private static IImmutableStack<IAmbientDbContext> _stack;
        }

        [Subject("Ambient DB Context Initialization")]
        class When_creating_multiple_joined_ambient_database_contexts_with_transaction_suppression_and_read_committed_isolation_level
        {
            Establish context = () =>
            {
#if NET45
                AmbientDbContextStorageProvider.SetStorage(new LogicalCallContextStorage());
#else
                AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());
#endif

                _storageHelper = new ContextualStorageHelper(AmbientDbContextStorageProvider.Storage);

                _dbConnectionMock = new Mock<IDbConnection>();
                _dbConnectionMock.Setup(mock => mock.State).Returns(() => ConnectionState.Closed);
            };

            Because of = () =>
            {
                _ambientDbContext1 = new AmbientDbContext(_dbConnectionMock.Object, true, true, IsolationLevel.ReadCommitted);
                _ambientDbContext2 = new AmbientDbContext(_dbConnectionMock.Object, true, true, IsolationLevel.ReadCommitted);

                _stack = _storageHelper.GetStack();
            };

            It should_add_both_ambient_database_contexts_to_the_stack = () =>
            {
                _stack.IsEmpty.ShouldBeFalse();
                _stack.Count().ShouldEqual(2);
                _stack.Peek().ShouldEqual(_ambientDbContext2);
                _stack.Last().ShouldEqual(_ambientDbContext1);
            };

            It should_create_the_expected_first_ambient_database_context = () =>
            {
                _ambientDbContext1.Parent.ShouldEqual(null);
                _ambientDbContext1.Connection.ShouldEqual(_ambientDbContext1.Connection);
                _ambientDbContext1.Transaction.ShouldEqual(_ambientDbContext1.Transaction);
            };

            It should_create_the_expected_second_ambient_database_context = () =>
            {
                _ambientDbContext2.Parent.ShouldEqual(_ambientDbContext1);
                _ambientDbContext2.Connection.ShouldEqual(_ambientDbContext1.Connection);
                _ambientDbContext2.Transaction.ShouldEqual(_ambientDbContext1.Transaction);
            };

            Cleanup test = () =>
            {
                AmbientDbContextStorageProvider.SetStorage(null);

                _ambientDbContext2.Dispose();
                _ambientDbContext1.Dispose();
            };

            private static Mock<IDbConnection> _dbConnectionMock;
            private static AmbientDbContext _ambientDbContext1;
            private static AmbientDbContext _ambientDbContext2;
            private static ContextualStorageHelper _storageHelper;
            private static IImmutableStack<IAmbientDbContext> _stack;
        }

        [Subject("Ambient DB Context Initialization")]
        class When_creating_disjoined_ambient_database_contexts_with_transaction_suppression_and_read_committed_isolation_level
        {
            Establish context = () =>
            {
#if NET45
                AmbientDbContextStorageProvider.SetStorage(new LogicalCallContextStorage());
#else
                AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());
#endif

                _storageHelper = new ContextualStorageHelper(AmbientDbContextStorageProvider.Storage);

                _dbConnectionMock1 = new Mock<IDbConnection>();
                _dbConnectionMock1.Setup(mock => mock.State).Returns(() => ConnectionState.Closed);

                _dbConnectionMock2 = new Mock<IDbConnection>();
                _dbConnectionMock2.Setup(mock => mock.State).Returns(() => ConnectionState.Closed);
            };

            Because of = () =>
            {
                _ambientDbContext1 = new AmbientDbContext(_dbConnectionMock1.Object, false, true, IsolationLevel.ReadCommitted);
                _ambientDbContext2 = new AmbientDbContext(_dbConnectionMock2.Object, false, true, IsolationLevel.ReadCommitted);

                _stack = _storageHelper.GetStack();
            };

            It should_add_both_ambient_database_contexts_to_the_stack = () =>
            {
                _stack.IsEmpty.ShouldBeFalse();
                _stack.Count().ShouldEqual(2);
                _stack.Peek().ShouldEqual(_ambientDbContext2);
                _stack.Last().ShouldEqual(_ambientDbContext1);
            };

            It should_make_two_distinct_ambient_database_contexts = () =>
            {
                _ambientDbContext2.Parent.ShouldEqual(null);
                _ambientDbContext2.Connection.ShouldEqual(_dbConnectionMock2.Object);
                _ambientDbContext2.Transaction.ShouldEqual(null);

                _ambientDbContext1.Parent.ShouldEqual(null);
                _ambientDbContext1.Connection.ShouldEqual(_dbConnectionMock1.Object);
                _ambientDbContext1.Transaction.ShouldEqual(null);
            };

            Cleanup test = () =>
            {
                AmbientDbContextStorageProvider.SetStorage(null);

                _ambientDbContext2.Dispose();
                _ambientDbContext1.Dispose();
            };

            private static Mock<IDbConnection> _dbConnectionMock1;
            private static Mock<IDbConnection> _dbConnectionMock2;
            private static AmbientDbContext _ambientDbContext1;
            private static AmbientDbContext _ambientDbContext2;
            private static ContextualStorageHelper _storageHelper;
            private static IImmutableStack<IAmbientDbContext> _stack;
        }

        [Subject("Ambient DB Context Disposal")]
        class When_disposing_single_ambient_database_context
        {
            Establish context = () =>
            {
#if NET45
                AmbientDbContextStorageProvider.SetStorage(new LogicalCallContextStorage());
#else
                AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());
#endif

                _storageHelper = new ContextualStorageHelper(AmbientDbContextStorageProvider.Storage);

                var dbConnectionState = ConnectionState.Open;

                _dbConnectionMock = new Mock<IDbConnection>();
                _dbConnectionMock.Setup(mock => mock.State).Returns(() => dbConnectionState);

                _ambientDbContext = new AmbientDbContext(_dbConnectionMock.Object, true, true, IsolationLevel.ReadCommitted);
            };

            Because of = () =>
            {
                _ambientDbContext.Dispose();
            };

            It should_close_the_database_connection = () =>
            {
                _dbConnectionMock.Verify(mock => mock.Close(), Times.Once);
            };

            It should_dispose_the_database_connection = () =>
            {
                _dbConnectionMock.Verify(mock => mock.Dispose(), Times.Once);
            };

            It should_set_the_database_connection_to_null = () =>
            {
                _ambientDbContext.Connection.ShouldBeNull();
            };

            It should_pop_ambient_database_context_from_stack = () =>
            {
                var stack = _storageHelper.GetStack();

                stack.IsEmpty.ShouldEqual(true);
            };

            Cleanup test = () =>
            {
                AmbientDbContextStorageProvider.SetStorage(null);
            };

            private static Mock<IDbConnection> _dbConnectionMock;
            private static AmbientDbContext _ambientDbContext;
            private static ContextualStorageHelper _storageHelper;
        }

        [Subject("Ambient DB Context Disposal")]
        class When_disposing_multiple_joined_ambient_database_contexts
        {
            Establish context = () =>
            {
#if NET45
                AmbientDbContextStorageProvider.SetStorage(new LogicalCallContextStorage());
#else
                AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());
#endif

                _storageHelper = new ContextualStorageHelper(AmbientDbContextStorageProvider.Storage);

                var dbConnectionState = ConnectionState.Open;

                _parentDbConnection1Mock = new Mock<IDbConnection>();
                _parentDbConnection1Mock.Setup(mock => mock.State).Returns(() => dbConnectionState);

                _childDbConnection2Mock = new Mock<IDbConnection>();
                _childDbConnection2Mock.Setup(mock => mock.State).Returns(() => dbConnectionState);

                _parentAmbientDbContext1 = new AmbientDbContext(_parentDbConnection1Mock.Object, true, true, IsolationLevel.ReadCommitted);
                _childAmbientDbContext2 = new AmbientDbContext(_childDbConnection2Mock.Object, true, true, IsolationLevel.ReadCommitted);
            };

            Because of = () =>
            {
                _childAmbientDbContext2.Dispose();
                _parentAmbientDbContext1.Dispose();
            };

            It should_not_close_the_database_connection_on_the_first_disposed_ambient_database_context = () =>
            {
                _childDbConnection2Mock.Verify(mock => mock.Close(), Times.Never);
            };

            It should_not_dispose_the_database_connection_on_the_first_disposed_ambient_database_context = () =>
            {
                _childDbConnection2Mock.Verify(mock => mock.Dispose(), Times.Never);
            };

            It should_not_set_the_database_connection_to_null_on_the_first_disposed_ambient_database_context = () =>
            {
                _childAmbientDbContext2.Connection.ShouldNotBeNull();
            };

            It should_close_the_database_connection_on_the_second_disposed_ambient_database_context = () =>
            {
                _parentDbConnection1Mock.Verify(mock => mock.Close(), Times.Once);
            };

            It should_dispose_the_database_connection_on_the_second_disposed_ambient_database_context = () =>
            {
                _parentDbConnection1Mock.Verify(mock => mock.Dispose(), Times.Once);
            };

            It should_set_the_database_connection_to_null_on_the_second_disposed_ambient_database_context = () =>
            {
                _parentAmbientDbContext1.Connection.ShouldBeNull();
            };

            It should_pop_both_database_contexts_off_the_stack = () =>
            {
                var stack = _storageHelper.GetStack();

                stack.IsEmpty.ShouldEqual(true);
            };

            Cleanup test = () =>
            {
                AmbientDbContextStorageProvider.SetStorage(null);
            };

            private static Mock<IDbConnection> _parentDbConnection1Mock;
            private static Mock<IDbConnection> _childDbConnection2Mock;
            private static AmbientDbContext _parentAmbientDbContext1;
            private static AmbientDbContext _childAmbientDbContext2;
            private static ContextualStorageHelper _storageHelper;
        }

        [Subject("Ambient DB Context Disposal")]
        class When_disposing_multiple_disjoined_ambient_database_contexts
        {
            Establish context = () =>
            {
#if NET45
                AmbientDbContextStorageProvider.SetStorage(new LogicalCallContextStorage());
#else
                AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());
#endif

                _storageHelper = new ContextualStorageHelper(AmbientDbContextStorageProvider.Storage);

                var dbConnectionState = ConnectionState.Open;

                _dbConnection1Mock = new Mock<IDbConnection>();
                _dbConnection1Mock.Setup(mock => mock.State).Returns(() => dbConnectionState);

                _dbConnection2Mock = new Mock<IDbConnection>();
                _dbConnection2Mock.Setup(mock => mock.State).Returns(() => dbConnectionState);

                _ambientDbContext1 = new AmbientDbContext(_dbConnection1Mock.Object, false, true, IsolationLevel.ReadCommitted);
                _ambientDbContext2 = new AmbientDbContext(_dbConnection2Mock.Object, false, true, IsolationLevel.ReadCommitted);
            };

            Because of = () =>
            {
                _ambientDbContext2.Dispose();
                _ambientDbContext1.Dispose();
            };

            It should_close_the_database_connection_on_the_first_disposed_ambient_database_context = () =>
            {
                _dbConnection2Mock.Verify(mock => mock.Close(), Times.Once);
            };

            It should_dispose_the_database_connection_on_the_first_disposed_ambient_database_context = () =>
            {
                _dbConnection2Mock.Verify(mock => mock.Dispose(), Times.Once);
            };

            It should_set_the_database_connection_to_null_on_the_first_disposed_ambient_database_context = () =>
            {
                _ambientDbContext2.Connection.ShouldBeNull();
            };

            It should_close_the_database_connection_on_the_second_disposed_ambient_database_context = () =>
            {
                _dbConnection1Mock.Verify(mock => mock.Close(), Times.Once);
            };

            It should_dispose_the_database_connection_on_the_second_disposed_ambient_database_context = () =>
            {
                _dbConnection1Mock.Verify(mock => mock.Dispose(), Times.Once);
            };

            It should_set_the_database_connection_to_null_on_the_second_disposed_ambient_database_context = () =>
            {
                _ambientDbContext1.Connection.ShouldBeNull();
            };

            It should_pop_both_database_contexts_off_the_stack = () =>
            {
                var stack = _storageHelper.GetStack();

                stack.IsEmpty.ShouldEqual(true);
            };

            Cleanup test = () =>
            {
                AmbientDbContextStorageProvider.SetStorage(null);
            };

            private static Mock<IDbConnection> _dbConnection1Mock;
            private static Mock<IDbConnection> _dbConnection2Mock;
            private static AmbientDbContext _ambientDbContext1;
            private static AmbientDbContext _ambientDbContext2;
            private static ContextualStorageHelper _storageHelper;
        }

        [Subject("Ambient DB Context Disposal")]
        class When_disposing_multiple_joined_ambient_database_contexts_out_of_order
        {
            Establish context = () =>
            {
#if NET45
                AmbientDbContextStorageProvider.SetStorage(new LogicalCallContextStorage());
#else
                AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());
#endif

                _storageHelper = new ContextualStorageHelper(AmbientDbContextStorageProvider.Storage);

                var dbConnectionState = ConnectionState.Open;

                _parentDbConnection1Mock = new Mock<IDbConnection>();
                _parentDbConnection1Mock.Setup(mock => mock.State).Returns(() => dbConnectionState);

                _childDbConnection2Mock = new Mock<IDbConnection>();
                _childDbConnection2Mock.Setup(mock => mock.State).Returns(() => dbConnectionState);

                _parentAmbientDbContext1 = new AmbientDbContext(_parentDbConnection1Mock.Object, true, true, IsolationLevel.ReadCommitted);
                _parentAmbientDbContext2 = new AmbientDbContext(_childDbConnection2Mock.Object, true, true, IsolationLevel.ReadCommitted);
            };

            Because of = () =>
            {
                _exception = Catch.Exception(() =>
                {
                    _parentAmbientDbContext1.Dispose();
                    _parentAmbientDbContext2.Dispose();
                });
            };

            It should_throw_invalid_operation_exception = () =>
            {
                _exception.ShouldBeOfExactType<InvalidOperationException>();
                _exception.Message.ShouldEqual("Could not dispose ambient database context because it is not the active ambient database context. This could occur because ambient database context is being disposed out of order.");
            };

            It should_not_close_the_database_connection_on_the_first_disposed_ambient_database_context = () =>
            {
                _childDbConnection2Mock.Verify(mock => mock.Close(), Times.Never);
            };

            It should_not_dispose_the_database_connection_on_the_first_disposed_ambient_database_context = () =>
            {
                _childDbConnection2Mock.Verify(mock => mock.Dispose(), Times.Never);
            };

            It should_not_set_the_database_connection_to_null_on_the_first_disposed_ambient_database_context = () =>
            {
                _parentAmbientDbContext2.Connection.ShouldNotBeNull();
            };

            It should_not_close_the_database_connection_on_the_second_disposed_ambient_database_context = () =>
            {
                _parentDbConnection1Mock.Verify(mock => mock.Close(), Times.Never);
            };

            It should_not_dispose_the_database_connection_on_the_second_disposed_ambient_database_context = () =>
            {
                _parentDbConnection1Mock.Verify(mock => mock.Dispose(), Times.Never);
            };

            It should_set_the_database_connection_to_null_on_the_second_disposed_ambient_database_context = () =>
            {
                _parentAmbientDbContext1.Connection.ShouldNotBeNull();
            };

            It should_not_pop_either_database_context_off_the_stack = () =>
            {
                var stack = _storageHelper.GetStack();

                stack.IsEmpty.ShouldEqual(false);
                stack.Count().ShouldEqual(2);
            };

            Cleanup test = () =>
            {
                AmbientDbContextStorageProvider.SetStorage(null);

                _parentAmbientDbContext2.Dispose();
                _parentAmbientDbContext1.Dispose();
            };

            private static Exception _exception;
            private static Mock<IDbConnection> _parentDbConnection1Mock;
            private static Mock<IDbConnection> _childDbConnection2Mock;
            private static AmbientDbContext _parentAmbientDbContext1;
            private static AmbientDbContext _parentAmbientDbContext2;
            private static ContextualStorageHelper _storageHelper;
        }

        [Subject("Ambient DB Context Commit")]
        class When_committing_single_ambient_database_context_without_suppressed_transaction_fails
        {
            Establish context = () =>
            {
#if NET45
                AmbientDbContextStorageProvider.SetStorage(new LogicalCallContextStorage());
#else
                AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());
#endif

                _storageHelper = new ContextualStorageHelper(AmbientDbContextStorageProvider.Storage);

                var dbConnectionState = ConnectionState.Closed;

                _dbConnectionMock = new Mock<IDbConnection>();
                _dbConnectionMock.Setup(mock => mock.State).Returns(() => dbConnectionState);
                _dbConnectionMock.Setup(mock => mock.Open()).Callback(() => dbConnectionState = ConnectionState.Open);

                _dbTransactionMock = new Mock<IDbTransaction>();
                _dbTransactionMock.Setup(mock => mock.Commit()).Throws<Exception>();

                _dbConnectionMock.Setup(mock => mock.BeginTransaction(Moq.It.IsAny<IsolationLevel>())).Returns(() => _dbTransactionMock.Object);

                _ambientDbContext = new AmbientDbContext(_dbConnectionMock.Object, true, false, IsolationLevel.ReadCommitted);

                _ambientDbContext.PrepareConnectionAndTransaction();
            };

            Because of = () =>
            {
                _exception = Catch.Exception(() => _ambientDbContext.Commit());
            };

            It should_rethrow_the_exception = () =>
            {
                _exception.ShouldNotBeNull();  
            };

            It should_rollback_the_database_transaction = () =>
            {
                _dbTransactionMock.Verify(mock => mock.Rollback(), Times.Once);
            };

            It should_dispose_the_database_transaction = () =>
            {
                _dbTransactionMock.Verify(mock => mock.Dispose(), Times.Once);
            };

            It should_set_the_database_transaction_to_null = () =>
            {
                _ambientDbContext.Transaction.ShouldBeNull();
            };

            Cleanup test = () =>
            {
                AmbientDbContextStorageProvider.SetStorage(null);

                _ambientDbContext.Dispose();
            };

            private static Mock<IDbConnection> _dbConnectionMock;
            private static Mock<IDbTransaction> _dbTransactionMock;
            private static AmbientDbContext _ambientDbContext;
            private static ContextualStorageHelper _storageHelper;
            private static Exception _exception;
        }

        [Subject("Ambient DB Context Commit")]
        class When_committing_single_ambient_database_context_without_suppressed_transaction
        {
            Establish context = () =>
            {
#if NET45
                AmbientDbContextStorageProvider.SetStorage(new LogicalCallContextStorage());
#else
                AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());
#endif

                _storageHelper = new ContextualStorageHelper(AmbientDbContextStorageProvider.Storage);

                var dbConnectionState = ConnectionState.Closed;

                _dbConnectionMock = new Mock<IDbConnection>();
                _dbConnectionMock.Setup(mock => mock.State).Returns(() => dbConnectionState);
                _dbConnectionMock.Setup(mock => mock.Open()).Callback(() => dbConnectionState = ConnectionState.Open);

                _dbTransactionMock = new Mock<IDbTransaction>();

                _dbConnectionMock.Setup(mock => mock.BeginTransaction(Moq.It.IsAny<IsolationLevel>())).Returns(() => _dbTransactionMock.Object);

                _ambientDbContext = new AmbientDbContext(_dbConnectionMock.Object, true, false, IsolationLevel.ReadCommitted);

                _ambientDbContext.PrepareConnectionAndTransaction();
            };

            Because of = () =>
            {
                _ambientDbContext.Commit();
            };

            It should_commit_the_database_transaction = () =>
            {
                _dbTransactionMock.Verify(mock => mock.Commit(), Times.Once);
            };

            It should_dispose_the_database_transaction = () =>
            {
                _dbTransactionMock.Verify(mock => mock.Dispose(), Times.Once);
            };

            It should_set_the_database_transaction_to_null = () =>
            {
                _ambientDbContext.Transaction.ShouldBeNull();
            };

            Cleanup test = () =>
            {
                AmbientDbContextStorageProvider.SetStorage(null);

                _ambientDbContext.Dispose();
            };

            private static Mock<IDbConnection> _dbConnectionMock;
            private static Mock<IDbTransaction> _dbTransactionMock;
            private static AmbientDbContext _ambientDbContext;
            private static ContextualStorageHelper _storageHelper;
        }

        [Subject("Ambient DB Context Commit")]
        class When_committing_single_ambient_database_context_with_suppressed_transaction
        {
            Establish context = () =>
            {
#if NET45
                AmbientDbContextStorageProvider.SetStorage(new LogicalCallContextStorage());
#else
                AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());
#endif

                _storageHelper = new ContextualStorageHelper(AmbientDbContextStorageProvider.Storage);

                var dbConnectionState = ConnectionState.Closed;

                _dbConnectionMock = new Mock<IDbConnection>();
                _dbConnectionMock.Setup(mock => mock.State).Returns(() => dbConnectionState);
                _dbConnectionMock.Setup(mock => mock.Open()).Callback(() => dbConnectionState = ConnectionState.Open);

                _dbTransactionMock = new Mock<IDbTransaction>();

                _dbConnectionMock.Setup(mock => mock.BeginTransaction(Moq.It.IsAny<IsolationLevel>())).Returns(() => _dbTransactionMock.Object);

                _ambientDbContext = new AmbientDbContext(_dbConnectionMock.Object, true, true, IsolationLevel.ReadCommitted);

                _ambientDbContext.PrepareConnectionAndTransaction();
            };

            Because of = () =>
            {
                _ambientDbContext.Commit();
            };

            It should_not_commit_the_database_transaction = () =>
            {
                _dbTransactionMock.Verify(mock => mock.Commit(), Times.Never);
            };

            It should_not_dispose_the_database_transaction = () =>
            {
                _dbTransactionMock.Verify(mock => mock.Dispose(), Times.Never);
            };

            Cleanup test = () =>
            {
                AmbientDbContextStorageProvider.SetStorage(null);

                _ambientDbContext.Dispose();
            };

            private static Mock<IDbConnection> _dbConnectionMock;
            private static Mock<IDbTransaction> _dbTransactionMock;
            private static AmbientDbContext _ambientDbContext;
            private static ContextualStorageHelper _storageHelper;
        }

        [Subject("Ambient DB Context Commit")]
        class When_committing_joined_child_ambient_database_context
        {
            Establish context = () =>
            {
#if NET45
                AmbientDbContextStorageProvider.SetStorage(new LogicalCallContextStorage());
#else
                AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());
#endif

                _storageHelper = new ContextualStorageHelper(AmbientDbContextStorageProvider.Storage);

                var dbConnectionState = ConnectionState.Closed;

                _parentDbConnectionMock = new Mock<IDbConnection>();
                _parentDbConnectionMock.Setup(mock => mock.State).Returns(() => dbConnectionState);
                _parentDbConnectionMock.Setup(mock => mock.Open()).Callback(() => dbConnectionState = ConnectionState.Open);

                _parentDbTransactionMock = new Mock<IDbTransaction>();

                _parentDbConnectionMock.Setup(mock => mock.BeginTransaction(Moq.It.IsAny<IsolationLevel>())).Returns(() => _parentDbTransactionMock.Object);

                _parentAmbientDbContext = new AmbientDbContext(_parentDbConnectionMock.Object, true, false, IsolationLevel.ReadCommitted);

                _parentAmbientDbContext.PrepareConnectionAndTransaction();

                _childDbConnectionMock = new Mock<IDbConnection>();
                _childAmbientDbContext = new AmbientDbContext(_childDbConnectionMock.Object, true, false, IsolationLevel.ReadCommitted);
            };

            Because of = () =>
            {
                _childAmbientDbContext.Commit();
            };

            It should_not_commit_the_parent_database_transaction = () =>
            {
                _parentDbTransactionMock.Verify(mock => mock.Commit(), Times.Never);
            };

            It should_not_dispose_the_parent_database_transaction = () =>
            {
                _parentDbTransactionMock.Verify(mock => mock.Dispose(), Times.Never);
            };

            It should_not_set_the_database_transaction_to_null = () =>
            {
                _parentAmbientDbContext.Transaction.ShouldNotBeNull();
            };

            Cleanup test = () =>
            {
                AmbientDbContextStorageProvider.SetStorage(null);

                _childAmbientDbContext.Dispose();
                _parentAmbientDbContext.Dispose();
            };

            private static Mock<IDbConnection> _parentDbConnectionMock;
            private static Mock<IDbTransaction> _parentDbTransactionMock;
            private static AmbientDbContext _parentAmbientDbContext;
            private static Mock<IDbConnection> _childDbConnectionMock;
            private static AmbientDbContext _childAmbientDbContext;
            private static ContextualStorageHelper _storageHelper;
        }

        [Subject("Ambient DB Context Commit")]
        class When_committing_joined_parent_ambient_database_context
        {
            Establish context = () =>
            {
#if NET45
                AmbientDbContextStorageProvider.SetStorage(new LogicalCallContextStorage());
#else
                AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());
#endif

                _storageHelper = new ContextualStorageHelper(AmbientDbContextStorageProvider.Storage);

                var dbConnectionState = ConnectionState.Closed;

                _parentDbConnectionMock = new Mock<IDbConnection>();
                _parentDbConnectionMock.Setup(mock => mock.State).Returns(() => dbConnectionState);
                _parentDbConnectionMock.Setup(mock => mock.Open()).Callback(() => dbConnectionState = ConnectionState.Open);

                _parentDbTransactionMock = new Mock<IDbTransaction>();

                _parentDbConnectionMock.Setup(mock => mock.BeginTransaction(Moq.It.IsAny<IsolationLevel>())).Returns(() => _parentDbTransactionMock.Object);

                _parentAmbientDbContext = new AmbientDbContext(_parentDbConnectionMock.Object, true, false, IsolationLevel.ReadCommitted);

                _parentAmbientDbContext.PrepareConnectionAndTransaction();

                _childDbConnectionMock = new Mock<IDbConnection>();

                _childAmbientDbContext = new AmbientDbContext(_childDbConnectionMock.Object, true, false, IsolationLevel.ReadCommitted);
            };

            Because of = () =>
            {
                _parentAmbientDbContext.Commit();
            };

            It should_commit_the_parent_database_transaction = () =>
            {
                _parentDbTransactionMock.Verify(mock => mock.Commit(), Times.Once);
            };

            It should_dispose_the_parent_database_transaction = () =>
            {
                _parentDbTransactionMock.Verify(mock => mock.Dispose(), Times.Once);
            };

            It should_set_the_database_transaction_to_null = () =>
            {
                _parentAmbientDbContext.Transaction.ShouldBeNull();
            };

            Cleanup test = () =>
            {
                AmbientDbContextStorageProvider.SetStorage(null);

                _childAmbientDbContext.Dispose();
                _parentAmbientDbContext.Dispose();
            };

            private static Mock<IDbConnection> _parentDbConnectionMock;
            private static Mock<IDbTransaction> _parentDbTransactionMock;
            private static AmbientDbContext _parentAmbientDbContext;
            private static Mock<IDbConnection> _childDbConnectionMock;
            private static AmbientDbContext _childAmbientDbContext;
            private static ContextualStorageHelper _storageHelper;
        }

        [Subject("Ambient DB Context Connection Preperation")]
        class When_the_joined_parent_ambient_database_context_requests_connection_first
        {
            Establish context = () =>
            {
#if NET45
                AmbientDbContextStorageProvider.SetStorage(new LogicalCallContextStorage());
#else
                AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());
#endif

                _storageHelper = new ContextualStorageHelper(AmbientDbContextStorageProvider.Storage);

                var dbConnectionState = ConnectionState.Closed;

                _parentDbConnectionMock = new Mock<IDbConnection>();
                _parentDbConnectionMock.Setup(mock => mock.State).Returns(() => dbConnectionState);
                _parentDbConnectionMock.Setup(mock => mock.Open()).Callback(() => dbConnectionState = ConnectionState.Open);

                _parentDbTransactionMock = new Mock<IDbTransaction>();
                _parentDbTransactionMock.Setup(mock => mock.IsolationLevel).Returns(IsolationLevel.ReadCommitted);

                _parentDbConnectionMock.Setup(mock => mock.BeginTransaction(Moq.It.IsAny<IsolationLevel>())).Returns(() => _parentDbTransactionMock.Object);

                _parentAmbientDbContext = new AmbientDbContext(_parentDbConnectionMock.Object, true, false, IsolationLevel.ReadCommitted);

                _childDbConnectionMock = new Mock<IDbConnection>();
                _childAmbientDbContext = new AmbientDbContext(_childDbConnectionMock.Object, true, true, IsolationLevel.Chaos);
            };

            Because of = () =>
            {
                _parentAmbientDbContext.PrepareConnectionAndTransaction();
            };

            It should_open_the_parent_database_connection = () =>
            {
                _parentAmbientDbContext.Connection.State.ShouldEqual(ConnectionState.Open);
            };

            It should_begin_the_parent_database_transaction = () =>
            {
                _parentAmbientDbContext.Transaction.ShouldNotBeNull();
                _parentAmbientDbContext.Transaction.IsolationLevel.ShouldEqual(_parentAmbientDbContext.IsolationLevel);
            };

            Cleanup test = () =>
            {
                AmbientDbContextStorageProvider.SetStorage(null);

                _childAmbientDbContext.Dispose();
                _parentAmbientDbContext.Dispose();
            };

            private static Mock<IDbConnection> _parentDbConnectionMock;
            private static Mock<IDbTransaction> _parentDbTransactionMock;
            private static AmbientDbContext _parentAmbientDbContext;
            private static Mock<IDbConnection> _childDbConnectionMock;
            private static AmbientDbContext _childAmbientDbContext;
            private static ContextualStorageHelper _storageHelper;
        }

        [Subject("Ambient DB Context Connection Preperation")]
        class When_the_joined_child_ambient_database_context_requests_connection_first
        {
            Establish context = () =>
            {
#if NET45
                AmbientDbContextStorageProvider.SetStorage(new LogicalCallContextStorage());
#else
                AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());
#endif

                _storageHelper = new ContextualStorageHelper(AmbientDbContextStorageProvider.Storage);

                var dbConnectionState = ConnectionState.Closed;

                _parentDbConnectionMock = new Mock<IDbConnection>();
                _parentDbConnectionMock.Setup(mock => mock.State).Returns(() => dbConnectionState);
                _parentDbConnectionMock.Setup(mock => mock.Open()).Callback(() => dbConnectionState = ConnectionState.Open);

                _parentDbTransactionMock = new Mock<IDbTransaction>();
                _parentDbTransactionMock.Setup(mock => mock.IsolationLevel).Returns(IsolationLevel.ReadCommitted);

                _parentDbConnectionMock.Setup(mock => mock.BeginTransaction(Moq.It.IsAny<IsolationLevel>())).Returns(() => _parentDbTransactionMock.Object);

                _parentAmbientDbContext = new AmbientDbContext(_parentDbConnectionMock.Object, true, false, IsolationLevel.ReadCommitted);

                _childDbConnectionMock = new Mock<IDbConnection>();
                _childAmbientDbContext = new AmbientDbContext(_childDbConnectionMock.Object, true, true, IsolationLevel.Chaos);
            };

            Because of = () =>
            {
                _childAmbientDbContext.PrepareConnectionAndTransaction();
            };

            It should_open_the_parent_database_connection = () =>
            {
                _parentAmbientDbContext.Connection.State.ShouldEqual(ConnectionState.Open);
                _childAmbientDbContext.Connection.ShouldEqual(_parentAmbientDbContext.Connection);
            };

            It should_begin_the_parent_database_transaction = () =>
            {
                _parentAmbientDbContext.Transaction.ShouldNotBeNull();
                _parentAmbientDbContext.Transaction.IsolationLevel.ShouldEqual(_parentAmbientDbContext.IsolationLevel);

                _childAmbientDbContext.Transaction.ShouldEqual(_parentAmbientDbContext.Transaction);
                _childAmbientDbContext.Transaction.IsolationLevel.ShouldEqual(_parentAmbientDbContext.IsolationLevel);
            };

            Cleanup test = () =>
            {
                AmbientDbContextStorageProvider.SetStorage(null);

                _childAmbientDbContext.Dispose();
                _parentAmbientDbContext.Dispose();
            };

            private static Mock<IDbConnection> _parentDbConnectionMock;
            private static Mock<IDbTransaction> _parentDbTransactionMock;
            private static AmbientDbContext _parentAmbientDbContext;
            private static Mock<IDbConnection> _childDbConnectionMock;
            private static AmbientDbContext _childAmbientDbContext;
            private static ContextualStorageHelper _storageHelper;
        }

        [Subject("Ambient DB Context Rollback")]
        class When_rolling_back_single_ambient_database_context_without_suppressed_transaction
        {
            Establish context = () =>
            {
#if NET45
                AmbientDbContextStorageProvider.SetStorage(new LogicalCallContextStorage());
#else
                AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());
#endif

                _storageHelper = new ContextualStorageHelper(AmbientDbContextStorageProvider.Storage);

                var dbConnectionState = ConnectionState.Closed;

                _dbConnectionMock = new Mock<IDbConnection>();
                _dbConnectionMock.Setup(mock => mock.State).Returns(() => dbConnectionState);
                _dbConnectionMock.Setup(mock => mock.Open()).Callback(() => dbConnectionState = ConnectionState.Open);

                _dbTransactionMock = new Mock<IDbTransaction>();

                _dbConnectionMock.Setup(mock => mock.BeginTransaction(Moq.It.IsAny<IsolationLevel>())).Returns(() => _dbTransactionMock.Object);

                _ambientDbContext = new AmbientDbContext(_dbConnectionMock.Object, true, false, IsolationLevel.ReadCommitted);

                _ambientDbContext.PrepareConnectionAndTransaction();
            };

            Because of = () =>
            {
                _ambientDbContext.Rollback();
            };

            It should_rollback_the_database_transaction = () =>
            {
                _dbTransactionMock.Verify(mock => mock.Rollback(), Times.Once);
            };

            It should_dispose_the_database_transaction = () =>
            {
                _dbTransactionMock.Verify(mock => mock.Dispose(), Times.Once);
            };

            It should_set_the_database_transaction_to_null = () =>
            {
                _ambientDbContext.Transaction.ShouldBeNull();
            };

            Cleanup test = () =>
            {
                AmbientDbContextStorageProvider.SetStorage(null);

                _ambientDbContext.Dispose();
            };

            private static Mock<IDbConnection> _dbConnectionMock;
            private static Mock<IDbTransaction> _dbTransactionMock;
            private static AmbientDbContext _ambientDbContext;
            private static ContextualStorageHelper _storageHelper;
        }

        [Subject("Ambient DB Context Rollback")]
        class When_rolling_back_single_ambient_database_context_with_suppressed_transaction
        {
            Establish context = () =>
            {
#if NET45
                AmbientDbContextStorageProvider.SetStorage(new LogicalCallContextStorage());
#else
                AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());
#endif

                _storageHelper = new ContextualStorageHelper(AmbientDbContextStorageProvider.Storage);

                var dbConnectionState = ConnectionState.Closed;

                _dbConnectionMock = new Mock<IDbConnection>();
                _dbConnectionMock.Setup(mock => mock.State).Returns(() => dbConnectionState);
                _dbConnectionMock.Setup(mock => mock.Open()).Callback(() => dbConnectionState = ConnectionState.Open);

                _dbTransactionMock = new Mock<IDbTransaction>();

                _dbConnectionMock.Setup(mock => mock.BeginTransaction(Moq.It.IsAny<IsolationLevel>())).Returns(() => _dbTransactionMock.Object);

                _ambientDbContext = new AmbientDbContext(_dbConnectionMock.Object, true, true, IsolationLevel.ReadCommitted);

                _ambientDbContext.PrepareConnectionAndTransaction();
            };

            Because of = () =>
            {
                _ambientDbContext.Rollback();
            };

            It should_not_commit_the_database_transaction = () =>
            {
                _dbTransactionMock.Verify(mock => mock.Commit(), Times.Never);
            };

            It should_not_dispose_the_database_transaction = () =>
            {
                _dbTransactionMock.Verify(mock => mock.Dispose(), Times.Never);
            };

            Cleanup test = () =>
            {
                AmbientDbContextStorageProvider.SetStorage(null);

                _ambientDbContext.Dispose();
            };

            private static Mock<IDbConnection> _dbConnectionMock;
            private static Mock<IDbTransaction> _dbTransactionMock;
            private static AmbientDbContext _ambientDbContext;
            private static ContextualStorageHelper _storageHelper;
        }

        [Subject("Ambient DB Context Rollback")]
        class When_rolling_back_joined_child_ambient_database_context
        {
            Establish context = () =>
            {
#if NET45
                AmbientDbContextStorageProvider.SetStorage(new LogicalCallContextStorage());
#else
                AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());
#endif

                _storageHelper = new ContextualStorageHelper(AmbientDbContextStorageProvider.Storage);

                var dbConnectionState = ConnectionState.Closed;

                _parentDbConnectionMock = new Mock<IDbConnection>();
                _parentDbConnectionMock.Setup(mock => mock.State).Returns(() => dbConnectionState);
                _parentDbConnectionMock.Setup(mock => mock.Open()).Callback(() => dbConnectionState = ConnectionState.Open);

                _parentDbTransactionMock = new Mock<IDbTransaction>();

                _parentDbConnectionMock.Setup(mock => mock.BeginTransaction(Moq.It.IsAny<IsolationLevel>())).Returns(() => _parentDbTransactionMock.Object);

                _parentAmbientDbContext = new AmbientDbContext(_parentDbConnectionMock.Object, true, false, IsolationLevel.ReadCommitted);

                _parentAmbientDbContext.PrepareConnectionAndTransaction();

                _childDbConnectionMock = new Mock<IDbConnection>();
                _childAmbientDbContext = new AmbientDbContext(_childDbConnectionMock.Object, true, false, IsolationLevel.ReadCommitted);
            };

            Because of = () =>
            {
                _childAmbientDbContext.Rollback();
            };

            It should_not_rollback_the_parent_database_transaction = () =>
            {
                _parentDbTransactionMock.Verify(mock => mock.Rollback(), Times.Never);
            };

            It should_not_dispose_the_parent_database_transaction = () =>
            {
                _parentDbTransactionMock.Verify(mock => mock.Dispose(), Times.Never);
            };

            It should_not_set_the_database_transaction_to_null = () =>
            {
                _parentAmbientDbContext.Transaction.ShouldNotBeNull();
            };

            Cleanup test = () =>
            {
                AmbientDbContextStorageProvider.SetStorage(null);

                _childAmbientDbContext.Dispose();
                _parentAmbientDbContext.Dispose();
            };

            private static Mock<IDbConnection> _parentDbConnectionMock;
            private static Mock<IDbTransaction> _parentDbTransactionMock;
            private static AmbientDbContext _parentAmbientDbContext;
            private static Mock<IDbConnection> _childDbConnectionMock;
            private static AmbientDbContext _childAmbientDbContext;
            private static ContextualStorageHelper _storageHelper;
        }

        [Subject("Ambient DB Context Commit")]
        class When_rolling_back_joined_parent_ambient_database_context
        {
            Establish context = () =>
            {
#if NET45
                AmbientDbContextStorageProvider.SetStorage(new LogicalCallContextStorage());
#else
                AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());
#endif

                _storageHelper = new ContextualStorageHelper(AmbientDbContextStorageProvider.Storage);

                var dbConnectionState = ConnectionState.Closed;

                _parentDbConnectionMock = new Mock<IDbConnection>();
                _parentDbConnectionMock.Setup(mock => mock.State).Returns(() => dbConnectionState);
                _parentDbConnectionMock.Setup(mock => mock.Open()).Callback(() => dbConnectionState = ConnectionState.Open);

                _parentDbTransactionMock = new Mock<IDbTransaction>();

                _parentDbConnectionMock.Setup(mock => mock.BeginTransaction(Moq.It.IsAny<IsolationLevel>())).Returns(() => _parentDbTransactionMock.Object);

                _parentAmbientDbContext = new AmbientDbContext(_parentDbConnectionMock.Object, true, false, IsolationLevel.ReadCommitted);

                _parentAmbientDbContext.PrepareConnectionAndTransaction();

                _childDbConnectionMock = new Mock<IDbConnection>();

                _childAmbientDbContext = new AmbientDbContext(_childDbConnectionMock.Object, true, false, IsolationLevel.ReadCommitted);
            };

            Because of = () =>
            {
                _parentAmbientDbContext.Rollback();
            };

            It should_rollback_the_parent_database_transaction = () =>
            {
                _parentDbTransactionMock.Verify(mock => mock.Rollback(), Times.Once);
            };

            It should_dispose_the_parent_database_transaction = () =>
            {
                _parentDbTransactionMock.Verify(mock => mock.Dispose(), Times.Once);
            };

            It should_set_the_database_transaction_to_null = () =>
            {
                _parentAmbientDbContext.Transaction.ShouldBeNull();
            };

            Cleanup test = () =>
            {
                AmbientDbContextStorageProvider.SetStorage(null);

                _childAmbientDbContext.Dispose();
                _parentAmbientDbContext.Dispose();
            };

            private static Mock<IDbConnection> _parentDbConnectionMock;
            private static Mock<IDbTransaction> _parentDbTransactionMock;
            private static AmbientDbContext _parentAmbientDbContext;
            private static Mock<IDbConnection> _childDbConnectionMock;
            private static AmbientDbContext _childAmbientDbContext;
            private static ContextualStorageHelper _storageHelper;
        }

        [Subject("Ambient DB Context Rollback")]
        class When_rolling_back_single_ambient_database_context_without_suppressed_transaction_fails
        {
            Establish context = () =>
            {
#if NET45
                AmbientDbContextStorageProvider.SetStorage(new LogicalCallContextStorage());
#else
                AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());
#endif

                _storageHelper = new ContextualStorageHelper(AmbientDbContextStorageProvider.Storage);

                var dbConnectionState = ConnectionState.Closed;

                _dbConnectionMock = new Mock<IDbConnection>();
                _dbConnectionMock.Setup(mock => mock.State).Returns(() => dbConnectionState);
                _dbConnectionMock.Setup(mock => mock.Open()).Callback(() => dbConnectionState = ConnectionState.Open);

                _dbTransactionMock = new Mock<IDbTransaction>();
                _dbTransactionMock.Setup(mock => mock.Connection).Returns(_dbConnectionMock.Object);
                _dbTransactionMock.Setup(mock => mock.Rollback()).Throws<Exception>();

                _dbConnectionMock.Setup(mock => mock.BeginTransaction(Moq.It.IsAny<IsolationLevel>())).Returns(() => _dbTransactionMock.Object);

                _ambientDbContext = new AmbientDbContext(_dbConnectionMock.Object, true, false, IsolationLevel.ReadCommitted);

                _ambientDbContext.PrepareConnectionAndTransaction();
            };

            Because of = () =>
            {
                _exception = Catch.Exception(() => _ambientDbContext.Rollback());
            };

            It should_rethrow_the_exception = () =>
            {
                _exception.ShouldNotBeNull();
            };

            It should_dispose_the_database_transaction = () =>
            {
                _dbTransactionMock.Verify(mock => mock.Dispose(), Times.Once);
            };

            It should_set_the_database_transaction_to_null = () =>
            {
                _ambientDbContext.Transaction.ShouldBeNull();
            };

            Cleanup test = () =>
            {
                AmbientDbContextStorageProvider.SetStorage(null);

                _ambientDbContext.Dispose();
            };

            private static Exception _exception;
            private static Mock<IDbConnection> _dbConnectionMock;
            private static Mock<IDbTransaction> _dbTransactionMock;
            private static AmbientDbContext _ambientDbContext;
            private static ContextualStorageHelper _storageHelper;
        }
    }
}