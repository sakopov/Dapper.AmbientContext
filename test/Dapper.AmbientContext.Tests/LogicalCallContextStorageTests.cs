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
#if NET452
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
                _storage.RemoveValue("key");
            };

            private static string _expectedValue;
            private static IContextualStorage _storage;
        }

        [Subject("Logical CallContext Storage")]
        class When_getting_inexisting_item_from_logical_callcontext_storage
        {
            Establish context = () =>
            {
#if NET452
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

        [Subject("Logical CallContext Storage")]
        class When_checking_if_added_item_exists_in_the_logical_callcontext_storage
        {
            Establish context = () =>
            {
#if NET452
                _storage = new LogicalCallContextStorage();
#else
                _storage = new AsyncLocalContextStorage();
#endif
            };

            Because of = () =>
            {
                _storage.SetValue("key", "value");

                _expectedValue = _storage.Exists("key");
            };

            It should_return_true = () =>
            {
                _expectedValue.ShouldEqual(true);
            };

            Cleanup test = () =>
            {
                _storage.RemoveValue("key");
            };

            private static bool _expectedValue;
            private static IContextualStorage _storage;
        }

        [Subject("Logical CallContext Storage")]
        class When_removing_an_item_from_the_logical_callcontext_storage
        {
            Establish context = () =>
            {
#if NET452
                _storage = new LogicalCallContextStorage();
#else
                _storage = new AsyncLocalContextStorage();
#endif
            };

            Because of = () =>
            {
                _storage.SetValue("key", "value");
                _storage.RemoveValue("key");

                _expectedValue = _storage.Exists("key");
            };

            It should_be_removed = () =>
            {
                _expectedValue.ShouldEqual(false);
            };

            Cleanup test = () =>
            {
                _storage.RemoveValue("key");
            };

            private static bool _expectedValue;
            private static IContextualStorage _storage;
        }
    }
}