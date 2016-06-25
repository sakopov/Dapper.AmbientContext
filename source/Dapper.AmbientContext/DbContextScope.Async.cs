//
// DbContextScope.cs
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

using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Dapper.AmbientContext
{
    /// <summary>
    /// Implements database context scope.
    /// </summary>
    public sealed partial class DbContextScope
    {
        /// <summary>
        /// Opens database connection and begins transaction, if not suppressed.
        /// </summary>
        internal async Task OpenAsync()
        {
            await PrepareAsyncConnection();
        }

        /// <summary>
        /// Opens a database connection asynchronously and starts a new transaction.
        /// </summary>
        private async Task PrepareAsyncConnection()
        {
            if (Connection.State == ConnectionState.Closed)
            {
                await ((DbConnection)Connection).OpenAsync();
            }

            if (Transaction == null)
            {
                if (!Option.HasFlag(DbContextScopeOption.Suppress))
                {
                    Transaction = Connection.BeginTransaction(IsolationLevel);
                }
            }
        }

        #region IDbContextScope Members

        /// <summary>
        /// Executes an asynchronous query, returning the data typed as per <typeparam name="T"></typeparam>
        /// </summary>
        /// <typeparam name="T">
        /// The data type of the returned object.
        /// </typeparam>
        /// <param name="query">
        /// The query to execute.
        /// </param>
        /// <param name="param">
        /// The query parameters.
        /// </param>
        /// <param name="commandType">
        /// The type of command.
        /// </param>
        /// <returns>
        /// A sequence of data of the supplied type <typeparam name="T"></typeparam>.
        /// </returns>
        async Task<IEnumerable<T>> IDbContext.QueryAsync<T>(string query, object param, CommandType? commandType)
        {
            await PrepareAsyncConnection();

            return await Connection.QueryAsync<T>(query, param, Transaction, commandType: commandType);
        }

        /// <summary>
        /// Executes an asynchronous query, returning a list of dynamic objects.
        /// </summary>
        /// <param name="query">
        /// The query to execute.
        /// </param>
        /// <param name="param">
        /// The query parameters.
        /// </param>
        /// <param name="commandType">
        /// The type of command.
        /// </param>
        /// <returns>
        /// A sequence of dynamic objects.
        /// </returns>
        async Task<IEnumerable<dynamic>> IDbContext.QueryAsync(string query, object param, CommandType? commandType)
        {
            await PrepareAsyncConnection();

            return await Connection.QueryAsync(query, param, Transaction, commandType: commandType);
        }

        /// <summary>
        /// Executes asynchronous parameterized command.
        /// </summary>
        /// <param name="sql">
        /// The query to execute.
        /// </param>
        /// <param name="param">
        /// The query parameters.
        /// </param>
        /// <param name="commandType">
        /// The type of command.
        /// </param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        async Task<int> IDbContext.ExecuteAsync(string sql, object param, CommandType? commandType)
        {
            await PrepareAsyncConnection();

            return await Connection.ExecuteAsync(sql, param, Transaction, commandType: commandType);
        }

        /// <summary>
        /// Executes asynchronous parameterized command that selects a single value and returns it typed as 
        /// per <typeparam name="T"></typeparam>.
        /// </summary>
        /// <param name="sql">
        /// The query to execute.
        /// </param>
        /// <param name="param">
        /// The query parameters.
        /// </param>
        /// <param name="commandType">
        /// The type of command.
        /// </param>
        /// <returns>
        /// The first cell selected.
        /// </returns>
        async Task<T> IDbContext.ExecuteScalarAsync<T>(string sql, object param, CommandType? commandType)
        {
            await PrepareAsyncConnection();

            return await Connection.ExecuteScalarAsync<T>(sql, param, Transaction, commandType: commandType);
        }

        #endregion
    }
}
