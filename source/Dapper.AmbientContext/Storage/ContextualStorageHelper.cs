// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextualStorageHelper.cs">
//   Copyright (c) 2016 Sergey Akopov
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy
//   of this software and associated documentation files (the "Software"), to deal
//   in the Software without restriction, including without limitation the rights
//   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//   copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in
//   all copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//   THE SOFTWARE.
// </copyright>
// <summary>
//   Represents the type which is responsible for handling access to the underlying contextual storage across AppDomains.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Dapper.AmbientContext.Storage
{
    using System;
    using System.Collections.Immutable;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Represents the type which is responsible for handling access to the underlying contextual storage across AppDomains.
    /// </summary>
    /// <remarks>
    /// Note, it's not possible to store non-serializable items in the Logical CallContext. This prevents us from storing
    /// immutable stack of ambient database contexts. Instead, the Logical CallContext only stores a cross-reference key to the 
    /// immutable stack stored in <see cref="System.Runtime.CompilerServices.ConditionalWeakTable{String, IImmutableStack{AmbientDbContext}}"/>.
    /// </remarks>
    internal class ContextualStorageHelper
    {
        /// <summary>
        /// Using a <c>ConditionalWeakTable</c> in order to prevent leaking <c>AmbientDbContext</c> instances if they're not
        /// disposed of properly. Using something like <c>ConcurrentDictionary</c> would hold a reference to the 
        /// <c>AmbientDbContext</c> and prevent Garbage Collector from collecting it, subsequently causing a memory
        /// leak. <c>ConditionalWeakTable</c> holds a "weak" reference to the <c>AmbientDbContext</c> instances. In other
        /// words an instance is referenced for as long as it exists in memory. This behavior does not obstruct
        /// Garbage Collector from collecting and disposing <c>AmbientDbContext</c> instances.
        /// More on this at <see href="http://stackoverflow.com/a/18613811"/>
        /// </summary>
        private static readonly ConditionalWeakTable<string, IImmutableStack<IAmbientDbContext>> AmbientDbContextTable = new ConditionalWeakTable<string, IImmutableStack<IAmbientDbContext>>();

        /// <summary>
        /// The underlying contextual storage.
        /// </summary>
        private readonly IContextualStorage _storage;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextualStorageHelper"/> class.
        /// </summary>
        /// <param name="storage">
        /// The contextual storage to use.
        /// </param>
        public ContextualStorageHelper(IContextualStorage storage)
        {
            _storage = storage;

            Initialize();
        }

        /// <summary>
        /// Gets the stack of <see cref="IAmbientDbContext"/> from the storage.
        /// </summary>
        /// <returns>
        /// The stack of <see cref="IAmbientDbContext"/> in the storage.
        /// </returns>
        public IImmutableStack<IAmbientDbContext> GetStack()
        {
            var crossReferenceKey = _storage.GetValue<ContextualStorageItem>(AmbientDbContextStorageKey.Key);

            // This can only happen if something explicitly calls RemoveValue on the storage. Otherwise, there will 
            // always be a value in storage.
            if (crossReferenceKey == null)
            {
                throw new AmbientDbContextException("Could not find ambient database context stack in the storage.");
            }

            IImmutableStack<IAmbientDbContext> value;

            AmbientDbContextTable.TryGetValue(crossReferenceKey.Value, out value);

            return value;
        }

        /// <summary>
        /// Saves the stack of <see cref="IAmbientDbContext"/> in the storage.
        /// </summary>
        /// <param name="stack">
        /// The stack of <see cref="IAmbientDbContext"/> to save.
        /// </param>
        public void SaveStack(IImmutableStack<IAmbientDbContext> stack)
        {
            var crossReferenceKey = _storage.GetValue<ContextualStorageItem>(AmbientDbContextStorageKey.Key);

            // This can only happen if something explicitly calls RemoveValue on the storage. Otherwise, there will 
            // always be a value in storage.
            if (crossReferenceKey == null)
            {
                throw new AmbientDbContextException("Could not find ambient database context stack in the storage.");
            }

            IImmutableStack<IAmbientDbContext> value;

            // Drop the existing key and recreate because the value is immutable
            if (AmbientDbContextTable.TryGetValue(crossReferenceKey.Value, out value))
            {
                AmbientDbContextTable.Remove(crossReferenceKey.Value);
            }

            AmbientDbContextTable.Add(crossReferenceKey.Value, stack);
        }

        /// <summary>
        /// Creates a cross-reference in the storage, which points to the stack of <see cref="AmbientDbContext"/>
        /// in the ConditionalWeakTable.
        /// </summary>
        private void Initialize()
        {
            var crossReferenceKey = _storage.GetValue<ContextualStorageItem>(AmbientDbContextStorageKey.Key);

            if (crossReferenceKey == null)
            {
                crossReferenceKey = new ContextualStorageItem(Guid.NewGuid().ToString("N"));

                _storage.SetValue(AmbientDbContextStorageKey.Key, crossReferenceKey);

                AmbientDbContextTable.Add(crossReferenceKey.Value, ImmutableStack.Create<IAmbientDbContext>());
            }
        }

        /// <summary>
        /// Wraps values in storage to enable access across AppDomains. While all storage
        /// mechanisms will use the same approach, it is only required for the Logical 
        /// CallContext storage.
        /// </summary>
        internal class ContextualStorageItem : MarshalByRefObject
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ContextualStorageItem"/> class.
            /// </summary>
            /// <param name="value">
            /// The value to store in the storage.
            /// </param>
            public ContextualStorageItem(string value)
            {
                Value = value;
            }

            /// <summary>
            /// Gets the stored item value.
            /// </summary>
            public string Value { get; }
        }
    }
}