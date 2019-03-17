using System;
using Dapper.AmbientContext.Storage;
using Machine.Specifications;

namespace Dapper.AmbientContext.Tests
{
    internal class AmbientDbContextStorageProviderTests
    {
        [Subject("Ambient DB Context Storage Provider")]
        class When_getting_storage_before_it_has_been_set
        {
            Because of = () =>
            {
                _exception = Catch.Exception(() => AmbientDbContextStorageProvider.Storage);
            };

            It should_throw_invalid_operation_exception = () =>
            {
                _exception.ShouldBeOfExactType<InvalidOperationException>();
                _exception.Message.ShouldEqual("Ambient database context storage hasn't been configured. Use AmbientDbContextStorageProvider.SetStorage(IContextualStorage) to configure ambient database context storage.");
            };

            private static Exception _exception;
        }

        [Subject("Ambient DB Context Storage Provider")]
        class When_getting_storage_after_it_has_been_set
        {
            Establish context = () =>
            {
#if NET452
                AmbientDbContextStorageProvider.SetStorage(new LogicalCallContextStorage());
#else
                AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());
#endif
            };

            Because of = () =>
            {
                _storage = AmbientDbContextStorageProvider.Storage;
            };

            It should_not_be_null = () =>
            {
                _storage.ShouldNotBeNull();
            };

            Cleanup test = () =>
            {
                AmbientDbContextStorageProvider.SetStorage(null);
            };

            private static IContextualStorage _storage;
        }
    }
}