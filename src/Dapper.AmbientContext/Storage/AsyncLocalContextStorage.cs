#if NETSTANDARD1_3
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogicalCallContextStorage.cs">
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
//   Represents the type that implements storage on top of AsyncLocal context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Dapper.AmbientContext.Storage
{
    using System.Threading;

    /// <summary>
    /// Represents the type that implements storage on top of AsyncLocal context.
    /// </summary>
    public class AsyncLocalContextStorage : IContextualStorage
    {
        private static readonly AsyncLocal<object> Storage = new AsyncLocal<object>();

        /// <summary>
        /// Returns an entry from the storage.
        /// </summary>
        /// <param name="key">
        /// A unique identifier for the storage entry to get.
        /// </param>
        /// <typeparam name="T">
        /// The type of the storage entry.
        /// </typeparam>
        /// <returns>
        /// A reference to the storage entry that is identified by key, if the entry exists; otherwise, <c>null</c>.
        /// </returns>
        public T GetValue<T>(string key)
        {
            return (T)Storage.Value;
        }

        /// <summary>
        /// Determines whether a storage entry exists in the storage.
        /// </summary>
        /// <param name="key">
        /// A unique identifier for the storage entry to search for.
        /// </param>
        /// <returns>
        /// <c>true</c> if the storage contains a storage entry whose key matches key; otherwise, <c>false</c>.
        /// </returns>
        public bool Exists(string key)
        {
            return Storage.Value != null;
        }

        /// <summary>
        /// Removes a storage entry from the storage.
        /// </summary>
        /// <param name="key">
        /// A unique identifier for the storage entry to remove.
        /// </param>
        public void RemoveValue(string key)
        {
            Storage.Value = null;
        }

        /// <summary>
        /// Inserts a storage entry into the storage by using a key and a value.
        /// </summary>
        /// <param name="key">
        /// A unique identifier for the storage entry to insert.
        /// </param>
        /// <param name="value">
        /// The data for the storage entry.
        /// </param>
        /// <typeparam name="T">
        /// The type of the storage entry.
        /// </typeparam>
        public void SetValue<T>(string key, T value)
        {
            Storage.Value = value;
        }
    }
}
#endif