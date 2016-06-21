//
// IDbContext.cs
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
    /// The interface for the database context.
    /// </summary>
    public interface IDbContext
    {
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
        IEnumerable<T> Query<T>(string query, object param = null, CommandType? commandType = null);

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
        Task<IEnumerable<T>> QueryAsync<T>(string query, object param = null, CommandType? commandType = null);

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
        IEnumerable<dynamic> Query(string query, object param = null, CommandType? commandType = null);

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
        Task<IEnumerable<dynamic>> QueryAsync(string query, object param = null, CommandType? commandType = null);

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
        int Execute(string sql, object param = null, CommandType? commandType = null);

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
        Task<int> ExecuteAsync(string sql, object param = null, CommandType? commandType = null);

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
        T ExecuteScalar<T>(string sql, object param = null, CommandType? commandType = null);

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
        Task<T> ExecuteScalarAsync<T>(string sql, object param = null, CommandType? commandType = null); 
    }
}