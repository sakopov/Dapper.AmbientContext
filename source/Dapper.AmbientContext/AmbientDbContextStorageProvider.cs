// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientDbContextStorageProvider.cs">
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
//   Represents the type which provides contextual storage strategy.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Dapper.AmbientContext
{
    using System;
    using Storage;

    /// <summary>
    /// Represents the type which provides contextual storage strategy.
    /// </summary>
    public static class AmbientDbContextStorageProvider
    {
        /// <summary>
        /// The storage instance.
        /// </summary>
        private static IContextualStorage _storage;

        /// <summary>
        /// Gets current contextual storage.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// when attempting to get current contextual storage before setting it via <see cref="SetStorage"/>.
        /// </exception>
        internal static IContextualStorage Storage
        {
            get
            {
                if (_storage == null)
                {
                    throw new InvalidOperationException("Ambient database context storage hasn't been configured. Use AmbientDbContextStorageProvider.SetStorage(IContextualStorage) to configure ambient database context storage.");
                }

                return _storage;
            }
        }

        /// <summary>
        /// Sets contextual storage strategy for ambient database context.
        /// </summary>
        /// <param name="storage">
        /// The storage to use.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// when <paramref name="storage"/> is <c>null</c>.
        /// </exception>
        public static void SetStorage(IContextualStorage storage)
        {
            _storage = storage;
        }
    }
}