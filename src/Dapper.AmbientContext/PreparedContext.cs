// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreparedContext.cs">
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
//   Represents a prepared database context with an open connection and optional transaction.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Dapper.AmbientContext
{
    using System.Data;

    /// <summary>
    /// Represents a prepared database context with an open connection and optional transaction.
    /// </summary>
    public readonly struct PreparedContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreparedContext"/> struct.
        /// </summary>
        /// <param name="connection">The database connection in an open state.</param>
        /// <param name="transaction">The database transaction, or null if transaction is suppressed.</param>
        internal PreparedContext(IDbConnection connection, IDbTransaction transaction)
        {
            Connection = connection;
            Transaction = transaction;
        }

        /// <summary>
        /// Gets the database connection in an open state.
        /// </summary>
        public IDbConnection Connection { get; }

        /// <summary>
        /// Gets the database transaction, or null if transaction is suppressed.
        /// </summary>
        public IDbTransaction Transaction { get; }
    }
}
