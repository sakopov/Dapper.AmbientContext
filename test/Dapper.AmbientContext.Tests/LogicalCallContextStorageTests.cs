using Dapper.AmbientContext.Storage;
using Machine.Specifications;

namespace Dapper.AmbientContext.Tests
{
    internal class LogicalCallContextStorageTests
    {
        [Subject("Logical CallContext Storage")]
        class When_getting_existing_item_from_logical_callcontext_storage
        {
            Establish context = () =>
            {
#if NETFRAMEWORK
                _storage = new LogicalCallContextStorage();
#else
                _storage = new AsyncLocalContextStorage();
#endif
            };

            Because of = () =>
            {
                _storage.SetValue("key", "value");

                _expectedValue = _storage.GetValue<string>("key");
            };

            It should_return_the_expected_value = () =>
            {
                _expectedValue.ShouldEqual("value");
            };

            Cleanup test = () =>
            {
                _storage.SetValue<string>("key", null);
            };

            private static string _expectedValue;
            private static IContextualStorage _storage;
        }

        [Subject("Logical CallContext Storage")]
        class When_getting_inexisting_item_from_logical_callcontext_storage
        {
            Establish context = () =>
            {
#if NETFRAMEWORK
                _storage = new LogicalCallContextStorage();
#else
                _storage = new AsyncLocalContextStorage();
#endif
            };

            Because of = () =>
            {
                _expectedValue = _storage.GetValue<string>("key");
            };

            It should_return_null = () =>
            {
                _expectedValue.ShouldBeNull();
            };

            private static string _expectedValue;
            private static IContextualStorage _storage;
        }
    }
}