// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContextualStorage.cs">
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
//   Defines an interface that is implemented by contextual storage.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Dapper.AmbientContext.Storage
{
    /// <summary>
    /// Defines an interface that is implemented by contextual storage.
    /// </summary>
    public interface IContextualStorage
    {
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
        T GetValue<T>(string key);

        /// <summary>
        /// Determines whether a storage entry exists in the storage.
        /// </summary>
        /// <param name="key">
        /// A unique identifier for the storage entry to search for.
        /// </param>
        /// <returns>
        /// <c>true</c> if the storage contains a storage entry whose key matches key; otherwise, <c>false</c>.
        /// </returns>
        bool Exists(string key);

        /// <summary>
        /// Removes a storage entry from the storage.
        /// </summary>
        /// <param name="key">
        /// A unique identifier for the storage entry to remove.
        /// </param>
        void RemoveValue(string key);

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
        void SetValue<T>(string key, T value);
    }
}