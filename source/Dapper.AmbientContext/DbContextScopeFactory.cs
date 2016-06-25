//
// DbContextScopeFactory.cs
// 
// Author:
//       Sergey Akopov <info@sergeyakopov.com>
//
// Copyright (c) 2016 Sergey Akopov
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Data;

namespace Dapper.AmbientContext
{
    /// <summary>
    /// Implements the database context scope factory.
    /// </summary>
    public sealed partial class DbContextScopeFactory : IDbContextScopeFactory
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        /// <summary>
        /// Creates a new instance of the <see cref="DbContextScopeFactory"/> class.
        /// </summary>
        /// <param name="dbConnectionFactory">
        /// The database connection factory.
        /// </param>
        public DbContextScopeFactory(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        /// <summary>
        /// Creates a new database context scope.
        /// </summary>
        /// <param name="suppress">
        /// Indicates whether a database transaction should be suppressed. The default value is <c>false</c>.
        /// </param>
        /// <param name="isolationLevel">
        /// The database transaction isolation level. The default value is <c>IsolationLevel.ReadCommitted</c>.
        /// </param>
        /// <returns>
        /// The new database context scope.
        /// </returns>
        public IDbContextScope Create(bool suppress = false, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            var dbContext = CreateInternal(suppress, isolationLevel);

            dbContext.Open();

            return dbContext;
        }

        /// <summary>
        /// Joins an existing database context scope or creates a new one if one doesn't exist.
        /// </summary>
        /// <returns>
        /// The database context scope.
        /// </returns>
        public IDbContextScope CreateOrJoin()
        {
            if (DbContextScope.DbContextScopeStack.IsEmpty)
            {
                return Create();
            }

            return new DbContextScope(option: DbContextScopeOption.Join);
        }

        /// <summary>
        /// Creates new database context scope.
        /// </summary>
        /// <param name="suppress">
        /// Indicates whether a database transaction should be suppressed. The default value is <c>false</c>.
        /// </param>
        /// <param name="isolationLevel">
        /// The database transaction isolation level. The default value is <c>IsolationLevel.ReadCommitted</c>.
        /// </param>
        /// <returns>
        /// The new database context scope.
        /// </returns>
        private DbContextScope CreateInternal(bool suppress = false, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            var connection = _dbConnectionFactory.Create();

            var option = DbContextScopeOption.New;

            if (suppress)
            {
                option |= DbContextScopeOption.Suppress;
            }

            return new DbContextScope(connection, option, isolationLevel);
        }
    }
}