//
// DbContext.cs
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
using System.Threading.Tasks;

namespace Dapper.AmbientContext
{
    /// <summary>
    /// Implements the database context which is consumed by any class that needs to make database calls. 
    /// This class acts as the proxy for <see cref="T:Dapper.AmbientContext.IDbContextScope"/> which contains 
    /// the actual logic. The reason for the separation is so to prevent the consuming class from having
    /// direct access to transaction and connection objects available at the database context scope level.
    /// </summary>
    public sealed class DbContext : IDbContext
    {
        private readonly IDbContextScope _dbContextScope;

        /// <summary>
        /// Creates a new instance of the <see cref="T:Dapper.AmbientContext.DbContext"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor is internal because it can only be called by <see cref="T:Dapper.AmbientContext.IAmbientDbContextLocator"/>.
        /// </remarks>
        /// <param name="dbContextScope">
        /// The active database context scope.
        /// </param>
        internal DbContext(IDbContextScope dbContextScope)
        {
            _dbContextScope = dbContextScope;
        }

        /// <summary>
        /// Executes a query, returning the data typed as per <typeparam name="T"></typeparam>.
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
        public IEnumerable<T> Query<T>(string query, object param = null, CommandType? commandType = null)
        {
            return _dbContextScope.Query<T>(query, param, commandType);
        }

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
        public async Task<IEnumerable<T>> QueryAsync<T>(string query, object param = null, CommandType? commandType = null)
        {
            return await _dbContextScope.QueryAsync<T>(query, param, commandType);
        }

        /// <summary>
        /// Executes a query, returning a list of dynamic objects.
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
        public IEnumerable<dynamic> Query(string query, object param = null, CommandType? commandType = null)
        {
            return _dbContextScope.Query(query, param, commandType);
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
        public async Task<IEnumerable<dynamic>> QueryAsync(string query, object param = null, CommandType? commandType = null)
        {
            return await _dbContextScope.QueryAsync(query, param, commandType);
        }

        /// <summary>
        /// Executes parameterized command.
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
        public int Execute(string sql, object param = null, CommandType? commandType = null)
        {
            return _dbContextScope.Execute(sql, param, commandType);
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
        public async Task<int> ExecuteAsync(string sql, object param = null, CommandType? commandType = null)
        {
            return await _dbContextScope.ExecuteAsync(sql, param, commandType);
        }

        /// <summary>
        /// Executes parameterized command that selects a single value and returns it typed as per 
        /// <typeparam name="T"></typeparam>.
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
        public T ExecuteScalar<T>(string sql, object param = null, CommandType? commandType = null)
        {
            return _dbContextScope.ExecuteScalar<T>(sql, param, commandType);
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
        public async Task<T> ExecuteScalarAsync<T>(string sql, object param = null, CommandType? commandType = null)
        {
            return await _dbContextScope.ExecuteScalarAsync<T>(sql, param, commandType);
        }
    }
}