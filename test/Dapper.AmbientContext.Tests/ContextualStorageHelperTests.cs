using System.Collections.Immutable;
using System.Data;
using Dapper.AmbientContext.Storage;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dapper.AmbientContext.Tests
{
    internal class ContextualStorageHelperTests
    {
        [Subject("Contextual Storage Helper")]
        class When_initializing_contextual_storage_helper
        {
            Establish context = () =>
            {
#if NET452
                _storage = new LogicalCallContextStorage();
#else
                _storage = new AsyncLocalContextStorage();
#endif

                AmbientDbContextStorageProvider.SetStorage(_storage);
            };

            Because of = () =>
            {
                _contextualStorageHelper = new ContextualStorageHelper(AmbientDbContextStorageProvider.Storage);

                _expectedValue = _storage.Exists(AmbientDbContextStorageKey.Key);
            };

            It should_create_a_cross_reference_in_the_storage_to_the_ambient_database_context_stack = () =>
            {
                _expectedValue.ShouldBeTrue();
            };

            Cleanup test = () =>
            {
                AmbientDbContextStorageProvider.SetStorage(null);
            };

            private static bool _expectedValue;
            private static IContextualStorage _storage;
            private static ContextualStorageHelper _contextualStorageHelper;
        }

        [Subject("Contextual Storage Helper")]
        class When_getting_an_empty_ambient_database_context_stack_from_the_contextual_storage_helper
        {
            Establish context = () =>
            {
#if NET452
                AmbientDbContextStorageProvider.SetStorage(new LogicalCallContextStorage());
#else
                AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());
#endif

                _contextualStorageHelper = new ContextualStorageHelper(AmbientDbContextStorageProvider.Storage);
            };

            Because of = () =>
            {
                _expectedValue = _contextualStorageHelper.GetStack();
            };

            It should_return_the_expected_ambient_database_context_stack = () =>
            {
                _expectedValue.ShouldNotBeNull();
                _expectedValue.IsEmpty.ShouldBeTrue();
            };

            Cleanup test = () =>
            {
                AmbientDbContextStorageProvider.SetStorage(null);
            };

            private static IImmutableStack<IAmbientDbContext> _expectedValue;
            private static ContextualStorageHelper _contextualStorageHelper;
        }

        [Subject("Contextual Storage Helper")]
        class When_getting_non_empty_ambient_database_context_stack_from_the_contextual_storage_helper
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

                _expectedAmbientDbContext = new AmbientDbContext(_dbConnectionMock.Object, true, true, IsolationLevel.ReadCommitted);

                _contextualStorageHelper = new ContextualStorageHelper(AmbientDbContextStorageProvider.Storage);

                var stack = ImmutableStack.Create<IAmbientDbContext>();
                stack = stack.Push(_expectedAmbientDbContext);

                _contextualStorageHelper.SaveStack(stack);
            };

            Because of = () =>
            {
                _returnedAmbientDbContextStack = _contextualStorageHelper.GetStack();
            };

            It should_return_the_expected_ambient_database_context_stack = () =>
            {
                _returnedAmbientDbContextStack.ShouldNotBeNull();
                _returnedAmbientDbContextStack.IsEmpty.ShouldBeFalse();
                _returnedAmbientDbContextStack.Peek().ShouldEqual(_expectedAmbientDbContext);
            };

            Cleanup test = () =>
            {
                _expectedAmbientDbContext.Dispose();

                AmbientDbContextStorageProvider.SetStorage(null);
            };

            private static Mock<IDbConnection> _dbConnectionMock;
            private static IAmbientDbContext _expectedAmbientDbContext;
            private static IImmutableStack<IAmbientDbContext> _returnedAmbientDbContextStack;
            private static ContextualStorageHelper _contextualStorageHelper;
        }
    }
}