// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientDbContextLocator.cs">
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
//   Represents the type which retrieves active database context from the storage.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Dapper.AmbientContext
{
    using System;
    using Storage;

    /// <summary>
    /// Represents the type which retrieves active database context from the storage.
    /// </summary>
    public sealed class AmbientDbContextLocator : IAmbientDbContextLocator
    {
        /// <summary>
        /// The contextual storage helper.
        /// </summary>
        private readonly ContextualStorageHelper _storageHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientDbContextLocator"/> class.
        /// </summary>
        public AmbientDbContextLocator()
        {
            _storageHelper = new ContextualStorageHelper(AmbientDbContextStorageProvider.Storage);
        }

        /// <summary>
        /// Retrieves active ambient database context from the storage.
        /// </summary>
        /// <returns>
        /// The active <see cref="IAmbientDbContextQueryProxy"/> instance.
        /// </returns>
        /// <exception cref="AmbientDbContextException">
        /// when the contextual storage does not contain an active ambient database context.
        /// </exception>
        public IAmbientDbContextQueryProxy Get()
        {
            var immutableStack = _storageHelper.GetStack();

            if (immutableStack.IsEmpty)
            {
                throw new InvalidOperationException("Could not find active ambient database context instance. Use AmbientDbContextFactory to create ambient database context before attempting to execute queries.");
            }

            return (IAmbientDbContextQueryProxy)immutableStack.Peek();
        }
    }
}