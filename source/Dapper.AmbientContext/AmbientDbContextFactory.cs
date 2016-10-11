// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientDbContextFactory.cs">
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
//   Represents the type that is responsible for creating ambient database context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Dapper.AmbientContext
{
    using System.Data;

    /// <summary>
    /// Represents the type that is responsible for creating ambient database context.
    /// </summary>
    public sealed class AmbientDbContextFactory : IAmbientDbContextFactory
    {
        /// <summary>
        /// The current database connection factory.
        /// </summary>
        private readonly IDbConnectionFactory _connectionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientDbContextFactory"/> class with the
        /// specified <see cref="IDbConnectionFactory"/> instance.
        /// </summary>
        /// <param name="dbConnectionFactory">
        /// The database connection factory.
        /// </param>
        public AmbientDbContextFactory(IDbConnectionFactory dbConnectionFactory)
        {
            _connectionFactory = dbConnectionFactory;
        }

        /// <summary>
        /// Creates an ambient database context with the specified options or joins an existing one.
        /// </summary>
        /// <param name="join">
        /// Determines whether to join and inherit an existing ambient database context. If one isn't found, a
        /// new ambient database context will be created based on the specified configurations provides via 
        /// <see cref="Create"/> method. The default is <c>true</c>.
        /// </param>
        /// <param name="suppress">
        /// Determines whether the transaction support should be disabled. The default is <c>false</c>.
        /// </param>
        /// <param name="isolationLevel">
        /// Sets the transaction isolation level. The default is <see cref="IsolationLevel.ReadCommitted"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IAmbientDbContext"/> instance.
        /// </returns>
        /// <exception cref="AmbientDbContextException">
        /// when the database connection factory returns a connection in a non-closed state.
        /// </exception>
        public IAmbientDbContext Create(bool join = true, bool suppress = false, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            var connection = _connectionFactory.Create();

            if (connection.State != ConnectionState.Closed)
            {
                throw new AmbientDbContextException("The database connection factory returned a database connection in a non-closed state. This behavior is not allowed as the ambient database context will maintain database connection state as required.");
            }

            var ambientDbContext = new AmbientDbContext(connection, join, suppress, isolationLevel);

            return ambientDbContext;
        }
    }
}