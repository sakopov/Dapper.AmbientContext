// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAmbientDbContext.cs">
//   Copyright (c) 2016-2026 Sergey Akopov
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
//   Defines an interface that is implemented by types capable of retrieving ambient database context from the storage.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Dapper.AmbientContext
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines an interface that is implemented by the ambient database context.
    /// </summary>
    public interface IAmbientDbContext : IDisposable
    {
        /// <summary>
        /// Gets the ambient database context connection. If inheriting from a parent
        /// ambient database context, this property will be assigned to parent's connection.
        /// </summary>
        IDbConnection Connection { get; }

        /// <summary>
        /// Gets the ambient database context transaction. If inheriting from a parent
        /// ambient database context, this property will be assigned to parent's transaction.
        /// </summary>
        IDbTransaction Transaction { get; }

        /// <summary>
        /// Gets a value indicating whether to suppress the database transaction.
        /// </summary>
        bool Suppress { get; }

        /// <summary>
        /// Gets the transaction isolation level.
        /// </summary>
        IsolationLevel IsolationLevel { get; }

        /// <summary>
        /// Prepares the database context by ensuring the connection is open and transaction
        /// is started (if not suppressed).
        /// </summary>
        /// <returns>A prepared context containing the connection and transaction.</returns>
        PreparedContext Prepare();

        /// <summary>
        /// Asynchronously prepares the database context by ensuring the connection is open
        /// and transaction is started (if not suppressed).
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation, containing the prepared context.</returns>
        Task<PreparedContext> PrepareAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits ambient database context.
        /// </summary>
        void Commit();

        /// <summary>
        /// Rolls back ambient database context transaction from a pending state.
        /// </summary>
        void Rollback();
    }
}