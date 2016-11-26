// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAmbientDbContextQueryProxy.cs">
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
//   Defines an interface that is implemented by any type that acts as the query proxy for Dapper.NET.
//   This would typically be the implementor of <see cref="IAmbientDbContext"/>.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Dapper.AmbientContext
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines an interface that is implemented by the type that acts as the query proxy for Dapper.NET.
    /// This would typically be the implementer of <see cref="IAmbientDbContext"/>.
    /// </summary>
    public interface IAmbientDbContextQueryProxy
    {
        /// <summary>
        /// Execute parameterized SQL that selects a single value asynchronously using 
        /// .NET 4.5 <see cref="System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// The first cell selected.
        /// </returns>
        Task<object> ExecuteScalarAsync(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Execute parameterized SQL that selects a single value asynchronously using 
        /// .NET 4.5 <see cref="System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// The first cell selected.
        /// </returns>
        Task<T> ExecuteScalarAsync<T>(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Execute parameterized SQL that selects a single value asynchronously using 
        /// .NET 4.5 <see cref="System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// The first cell selected.
        /// </returns>
        Task<object> ExecuteScalarAsync(CommandDefinition command);

        /// <summary>
        /// Execute parameterized SQL that selects a single value asynchronously using 
        /// .NET 4.5 <see cref="System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// The first cell selected.
        /// </returns>
        Task<T> ExecuteScalarAsync<T>(CommandDefinition command);

        /// <summary>
        /// Execute a query asynchronously using .NET 4.5 <see cref="System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <remarks>
        /// Note: each row can be accessed via "dynamic", or by casting to an
        /// <see cref="System.Collections.Generic.IDictionary{String,Object}" />.
        /// </remarks>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<IEnumerable<object>> QueryAsync(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Execute a query asynchronously using .NET 4.5 <see cref="System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <remarks>
        /// Note: each row can be accessed via "dynamic", or by casting to an
        /// <see cref="System.Collections.Generic.IDictionary{String,Object}" />.
        /// </remarks>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<IEnumerable<object>> QueryAsync(CommandDefinition command);

        /// <summary>
        /// Execute a single-row query asynchronously using .NET 4.5 
        /// <see cref="System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <remarks>
        /// Note: each row can be accessed via "dynamic", or by casting to an
        /// <see cref="System.Collections.Generic.IDictionary{String,Object}" />.
        /// </remarks>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<object> QueryFirstAsync(CommandDefinition command);

        /// <summary>
        /// Execute a single-row query asynchronously using .NET 4.5 
        /// <see cref="System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <remarks>
        /// Note: each row can be accessed via "dynamic", or by casting to an
        /// <see cref="System.Collections.Generic.IDictionary{String,Object}" />.
        /// </remarks>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<object> QueryFirstOrDefaultAsync(CommandDefinition command);

        /// <summary>
        /// Execute a single-row query asynchronously using .NET 4.5 
        /// <see cref="System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <remarks>
        /// Note: each row can be accessed via "dynamic", or by casting to an
        /// <see cref="System.Collections.Generic.IDictionary{String,Object}" />.
        /// </remarks>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<object> QuerySingleAsync(CommandDefinition command);

        /// <summary>
        /// Execute a single-row query asynchronously using .NET 4.5 
        /// <see cref="System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <remarks>
        /// Note: each row can be accessed via "dynamic", or by casting to an
        /// <see cref="System.Collections.Generic.IDictionary{String,Object}" />.
        /// </remarks>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<object> QuerySingleOrDefaultAsync(CommandDefinition command);

        /// <summary>
        /// Execute a query asynchronously using .NET 4.5 
        /// <see cref="System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<IEnumerable<T>> QueryAsync<T>(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Execute a single-row query asynchronously using .NET 4.5 
        /// <see cref="System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<T> QueryFirstAsync<T>(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Execute a single-row query asynchronously using .NET 4.5 
        /// <see cref="System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<T> QueryFirstOrDefaultAsync<T>(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Execute a single-row query asynchronously using .NET 4.5 
        /// <see cref="System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<T> QuerySingleAsync<T>(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Execute a single-row query asynchronously using .NET 4.5
        /// <see cref="System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<T> QuerySingleOrDefaultAsync<T>(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Execute a query asynchronously using .NET 4.5
        /// <see cref="System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <param name="type">
        /// The expected return type.
        /// </param>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<IEnumerable<object>> QueryAsync(
            Type type,
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Execute a single-row query asynchronously using .NET 4.5
        /// <see cref="System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <param name="type">
        /// The expected return type.
        /// </param>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<object> QueryFirstAsync(
            Type type,
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Execute a single-row query asynchronously using .NET 4.5
        /// <see cref="System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <param name="type">
        /// The expected return type.
        /// </param>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<object> QueryFirstOrDefaultAsync(
            Type type,
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Execute a single-row query asynchronously using .NET 4.5
        /// <see cref="System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <param name="type">
        /// The expected return type.
        /// </param>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<object> QuerySingleAsync(
            Type type,
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Execute a single-row query asynchronously using .NET 4.5
        /// <see cref="System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <param name="type">
        /// The expected return type.
        /// </param>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<object> QuerySingleOrDefaultAsync(
            Type type,
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Execute a query asynchronously using .NET 4.5
        /// <see cref="System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<IEnumerable<T>> QueryAsync<T>(CommandDefinition command);

        /// <summary>
        /// Execute a query asynchronously using .NET 4.5
        /// <see cref="System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <param name="type">
        /// The expected return type.
        /// </param>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<IEnumerable<object>> QueryAsync(Type type, CommandDefinition command);

        /// <summary>
        /// Execute a single-row query asynchronously using .NET 4.5
        /// <see cref="System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <param name="type">
        /// The expected return type.
        /// </param>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<object> QueryFirstAsync(Type type, CommandDefinition command);

        /// <summary>
        /// Execute a single-row query asynchronously using .NET 4.5
        /// <see cref="System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <param name="type">
        /// The expected return type.
        /// </param>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<object> QueryFirstOrDefaultAsync(Type type, CommandDefinition command);

        /// <summary>
        /// Execute a single-row query asynchronously using .NET 4.5
        /// <see cref="System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <param name="type">
        /// The expected return type.
        /// </param>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<object> QuerySingleAsync(Type type, CommandDefinition command);

        /// <summary>
        /// Execute a single-row query asynchronously using .NET 4.5
        /// <see cref="System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <param name="type">
        /// The expected return type.
        /// </param>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<object> QuerySingleOrDefaultAsync(Type type, CommandDefinition command);

        /// <summary>
        /// Execute a command asynchronously using .NET 4.5
        /// <see cref="System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<int> ExecuteAsync(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Execute a command asynchronously using .NET 4.5
        /// <see cref="System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<int> ExecuteAsync(CommandDefinition command);

        /// <summary>
        /// Perform a multi mapping query with 2 input parameters.
        /// </summary>
        /// <typeparam name="TFirst">
        /// The first type in the record set.
        /// </typeparam>
        /// <typeparam name="TSecond">
        /// The second type in the record set.
        /// </typeparam>
        /// <typeparam name="TReturn">
        /// The return type.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="map">
        /// The delegate to map a single row to multiple objects.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="buffered">
        /// Determines whether to execute SQL and buffer the entire reader on return.
        /// </param>
        /// <param name="splitOn">
        /// The field we should split and read the second object from (default: id).
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(
            string sql,
            Func<TFirst, TSecond, TReturn> map,
            object param = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Perform a multi mapping query with 2 input parameters.
        /// </summary>
        /// <typeparam name="TFirst">
        /// The first type in the record set.
        /// </typeparam>
        /// <typeparam name="TSecond">
        /// The second type in the record set.
        /// </typeparam>
        /// <typeparam name="TReturn">
        /// The return type.
        /// </typeparam>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <param name="map">
        /// The delegate to map a single row to multiple objects.
        /// </param>
        /// <param name="splitOn">
        /// The field we should split and read the second object from (default: id).
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(
            CommandDefinition command,
            Func<TFirst, TSecond, TReturn> map,
            string splitOn = "Id");

        /// <summary>
        /// Perform a multi mapping query with 3 input parameters.
        /// </summary>
        /// <typeparam name="TFirst">
        /// The first type in the record set.
        /// </typeparam>
        /// <typeparam name="TSecond">
        /// The second type in the record set.
        /// </typeparam>
        /// <typeparam name="TThird">
        /// The third type in the record set.
        /// </typeparam>
        /// <typeparam name="TReturn">
        /// The return type.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="map">
        /// The delegate to map a single row to multiple objects.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="buffered">
        /// Determines whether to execute SQL and buffer the entire reader on return.
        /// </param>
        /// <param name="splitOn">
        /// The field we should split and read the second object from (default: id).
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(
            string sql,
            Func<TFirst, TSecond, TThird, TReturn> map,
            object param = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Perform a multi mapping query with 3 input parameters.
        /// </summary>
        /// <typeparam name="TFirst">
        /// The first type in the record set.
        /// </typeparam>
        /// <typeparam name="TSecond">
        /// The second type in the record set.
        /// </typeparam>
        /// <typeparam name="TThird">
        /// The third type in the record set.
        /// </typeparam>
        /// <typeparam name="TReturn">
        /// The return type.
        /// </typeparam>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <param name="map">
        /// The delegate to map a single row to multiple objects.
        /// </param>
        /// <param name="splitOn">
        /// The field we should split and read the second object from (default: id).
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(
            CommandDefinition command,
            Func<TFirst, TSecond, TThird, TReturn> map,
            string splitOn = "Id");

        /// <summary>
        /// Perform a multi mapping query with 4 input parameters.
        /// </summary>
        /// <typeparam name="TFirst">
        /// The first type in the record set.
        /// </typeparam>
        /// <typeparam name="TSecond">
        /// The second type in the record set.
        /// </typeparam>
        /// <typeparam name="TThird">
        /// The third type in the record set.
        /// </typeparam>
        /// <typeparam name="TFourth">
        /// The fourth type in the record set.
        /// </typeparam>
        /// <typeparam name="TReturn">
        /// The return type.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="map">
        /// The delegate to map a single row to multiple objects.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="buffered">
        /// Determines whether to execute SQL and buffer the entire reader on return.
        /// </param>
        /// <param name="splitOn">
        /// The field we should split and read the second object from (default: id).
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(
            string sql, 
            Func<TFirst, TSecond, TThird, TFourth, TReturn> map, 
            object param = null,
            bool buffered = true, 
            string splitOn = "Id", 
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Perform a multi mapping query with 4 input parameters.
        /// </summary>
        /// <typeparam name="TFirst">
        /// The first type in the record set.
        /// </typeparam>
        /// <typeparam name="TSecond">
        /// The second type in the record set.
        /// </typeparam>
        /// <typeparam name="TThird">
        /// The third type in the record set.
        /// </typeparam>
        /// <typeparam name="TFourth">
        /// The fourth type in the record set.
        /// </typeparam>
        /// <typeparam name="TReturn">
        /// The return type.
        /// </typeparam>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <param name="map">
        /// The delegate to map a single row to multiple objects.
        /// </param>
        /// <param name="splitOn">
        /// The field we should split and read the second object from (default: id).
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(
            CommandDefinition command, 
            Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
            string splitOn = "Id");

        /// <summary>
        /// Perform a multi mapping query with 5 input parameters
        /// </summary>
        /// <typeparam name="TFirst">
        /// The first type in the record set.
        /// </typeparam>
        /// <typeparam name="TSecond">
        /// The second type in the record set.
        /// </typeparam>
        /// <typeparam name="TThird">
        /// The third type in the record set.
        /// </typeparam>
        /// <typeparam name="TFourth">
        /// The fourth type in the record set.
        /// </typeparam>
        /// <typeparam name="TFifth">
        /// The fifth type in the record set.
        /// </typeparam>
        /// <typeparam name="TReturn">
        /// The return type.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="map">
        /// The delegate to map a single row to multiple objects.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="buffered">
        /// Determines whether to execute SQL and buffer the entire reader on return.
        /// </param>
        /// <param name="splitOn">
        /// The field we should split and read the second object from (default: id).
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
            string sql, 
            Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
            object param = null, 
            bool buffered = true, 
            string splitOn = "Id",
            int? commandTimeout = null, 
            CommandType? commandType = null);

        /// <summary>
        /// Perform a multi mapping query with 5 input parameters
        /// </summary>
        /// <typeparam name="TFirst">
        /// The first type in the record set.
        /// </typeparam>
        /// <typeparam name="TSecond">
        /// The second type in the record set.
        /// </typeparam>
        /// <typeparam name="TThird">
        /// The third type in the record set.
        /// </typeparam>
        /// <typeparam name="TFourth">
        /// The fourth type in the record set.
        /// </typeparam>
        /// <typeparam name="TFifth">
        /// The fifth type in the record set.
        /// </typeparam>
        /// <typeparam name="TReturn">
        /// The return type.
        /// </typeparam>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <param name="map">
        /// The delegate to map a single row to multiple objects.
        /// </param>
        /// <param name="splitOn">
        /// The field we should split and read the second object from (default: id).
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
            CommandDefinition command,
            Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, 
            string splitOn = "Id");

        /// <summary>
        /// Perform a multi mapping query with 6 input parameters
        /// </summary>
        /// <typeparam name="TFirst">
        /// The first type in the record set.
        /// </typeparam>
        /// <typeparam name="TSecond">
        /// The second type in the record set.
        /// </typeparam>
        /// <typeparam name="TThird">
        /// The third type in the record set.
        /// </typeparam>
        /// <typeparam name="TFourth">
        /// The fourth type in the record set.
        /// </typeparam>
        /// <typeparam name="TFifth">
        /// The fifth type in the record set.
        /// </typeparam>
        /// <typeparam name="TSixth">
        /// The sixth type in the record set.
        /// </typeparam>
        /// <typeparam name="TReturn">
        /// The return type.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="map">
        /// The delegate to map a single row to multiple objects.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="buffered">
        /// Determines whether to execute SQL and buffer the entire reader on return.
        /// </param>
        /// <param name="splitOn">
        /// The field we should split and read the second object from (default: id).
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(
            string sql, 
            Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map,
            object param = null, 
            bool buffered = true, 
            string splitOn = "Id",
            int? commandTimeout = null, 
            CommandType? commandType = null);

        /// <summary>
        /// Perform a multi mapping query with 6 input parameters
        /// </summary>
        /// <typeparam name="TFirst">
        /// The first type in the record set.
        /// </typeparam>
        /// <typeparam name="TSecond">
        /// The second type in the record set.
        /// </typeparam>
        /// <typeparam name="TThird">
        /// The third type in the record set.
        /// </typeparam>
        /// <typeparam name="TFourth">
        /// The fourth type in the record set.
        /// </typeparam>
        /// <typeparam name="TFifth">
        /// The fifth type in the record set.
        /// </typeparam>
        /// <typeparam name="TSixth">
        /// The sixth type in the record set.
        /// </typeparam>
        /// <typeparam name="TReturn">
        /// The return type.
        /// </typeparam>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <param name="map">
        /// The delegate to map a single row to multiple objects.
        /// </param>
        /// <param name="splitOn">
        /// The field we should split and read the second object from (default: id).
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(
            CommandDefinition command,
            Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, 
            string splitOn = "Id");

        /// <summary>
        /// Perform a multi mapping query with 7 input parameters
        /// </summary>
        /// <typeparam name="TFirst">
        /// The first type in the record set.
        /// </typeparam>
        /// <typeparam name="TSecond">
        /// The second type in the record set.
        /// </typeparam>
        /// <typeparam name="TThird">
        /// The third type in the record set.
        /// </typeparam>
        /// <typeparam name="TFourth">
        /// The fourth type in the record set.
        /// </typeparam>
        /// <typeparam name="TFifth">
        /// The fifth type in the record set.
        /// </typeparam>
        /// <typeparam name="TSixth">
        /// The sixth type in the record set.
        /// </typeparam>
        /// <typeparam name="TSeventh">
        /// The seventh type in the record set.
        /// </typeparam>
        /// <typeparam name="TReturn">
        /// The return type.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="map">
        /// The delegate to map a single row to multiple objects.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="buffered">
        /// Determines whether to execute SQL and buffer the entire reader on return.
        /// </param>
        /// <param name="splitOn">
        /// The field we should split and read the second object from (default: id).
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
            string sql,
            Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
            object param = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Perform a multi mapping query with 7 input parameters
        /// </summary>
        /// <typeparam name="TFirst">
        /// The first type in the record set.
        /// </typeparam>
        /// <typeparam name="TSecond">
        /// The second type in the record set.
        /// </typeparam>
        /// <typeparam name="TThird">
        /// The third type in the record set.
        /// </typeparam>
        /// <typeparam name="TFourth">
        /// The fourth type in the record set.
        /// </typeparam>
        /// <typeparam name="TFifth">
        /// The fifth type in the record set.
        /// </typeparam>
        /// <typeparam name="TSixth">
        /// The sixth type in the record set.
        /// </typeparam>
        /// <typeparam name="TSeventh">
        /// The seventh type in the record set.
        /// </typeparam>
        /// <typeparam name="TReturn">
        /// The return type.
        /// </typeparam>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <param name="map">
        /// The delegate to map a single row to multiple objects.
        /// </param>
        /// <param name="splitOn">
        /// The field we should split and read the second object from (default: id).
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
            CommandDefinition command,
            Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
            string splitOn = "Id");

        /// <summary>
        /// Perform a multi mapping query with arbitrary input parameters.
        /// </summary>
        /// <typeparam name="TReturn">
        /// The return type.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="types">
        /// The array of types in the record set.
        /// </param>
        /// <param name="map">
        /// The delegate to map a single row to multiple objects.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="buffered">
        /// Determines whether to execute SQL and buffer the entire reader on return.
        /// </param>
        /// <param name="splitOn">
        /// The field we should split and read the second object from (default: id).
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<IEnumerable<TReturn>> QueryAsync<TReturn>(
            string sql, 
            Type[] types,
            Func<object[], TReturn> map, 
            object param = null,
            bool buffered = true, 
            string splitOn = "Id", 
            int? commandTimeout = null, 
            CommandType? commandType = null);

        /// <summary>
        /// Execute a command that returns multiple result sets, and access each in turn.
        /// </summary>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<SqlMapper.GridReader> QueryMultipleAsync(
            string sql,
            object param = null, 
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Execute a command that returns multiple result sets, and access each in turn.
        /// </summary>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<SqlMapper.GridReader> QueryMultipleAsync(CommandDefinition command);

        /// <summary>
        /// Execute parameterized SQL and return an <see cref="T:System.Data.IDataReader"/>
        /// </summary> 
        /// <returns>
        /// An <see cref="T:System.Data.IDataReader"/> that can be used to iterate over the results of the SQL query.
        /// </returns> 
        /// <remarks>
        /// This is typically used when the results of a query are not processed by Dapper, for example, used to fill a 
        /// <see cref="T:System.Data.DataTable"/> or <see cref="T:DataSet"/>. 
        /// </remarks> 
        /// <example> 
        /// <code>
        /// <![CDATA[
        ///             DataTable table = new DataTable("MyTable");
        ///             using (var reader = ExecuteReader(cnn, sql, param))
        ///             {
        ///                 table.Load(reader);
        ///             }
        /// ]]>
        /// </code> 
        /// </example>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<IDataReader> ExecuteReaderAsync(
            string sql, 
            object param = null, 
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Execute parameterized SQL and return an <see cref="T:System.Data.IDataReader"/>
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Data.IDataReader"/> that can be used to iterate over the results of the SQL query.
        /// </returns> 
        /// <remarks>
        /// This is typically used when the results of a query are not processed by Dapper, for example, used to fill a 
        /// <see cref="T:System.Data.DataTable"/> or <see cref="T:DataSet"/>. 
        /// </remarks>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        Task<IDataReader> ExecuteReaderAsync(CommandDefinition command);

        /// <summary>
        /// Execute parameterized SQL.
        /// </summary>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// Number of rows affected.
        /// </returns>
        int Execute(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Execute parameterized SQL.
        /// </summary>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// Number of rows affected.
        /// </returns>
        int Execute(CommandDefinition command);

        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// The first cell selected.
        /// </returns>
        object ExecuteScalar(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// The first cell selected.
        /// </returns>
        T ExecuteScalar<T>(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// The first cell selected.
        /// </returns>
        object ExecuteScalar(CommandDefinition command);

        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// The first cell selected.
        /// </returns>
        T ExecuteScalar<T>(CommandDefinition command);

        /// <summary>
        /// Execute parameterized SQL and return an <see cref="T:System.Data.IDataReader" />.
        /// </summary>
        /// <remarks>
        /// This is typically used when the results of a query are not processed by Dapper, for example, used to fill a
        /// <see cref="T:System.Data.DataTable" />
        /// or <see cref="T:DataSet" />.
        /// </remarks>
        /// <example>
        /// <code>
        /// <![CDATA[
        ///             DataTable table = new DataTable("MyTable");
        ///             using (var reader = ExecuteReader(cnn, sql, param))
        ///             {
        ///                 table.Load(reader);
        ///             }
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// An <see cref="T:System.Data.IDataReader" /> that can be used to iterate over the results of the SQL query.
        /// </returns>
        IDataReader ExecuteReader(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Execute parameterized SQL and return an <see cref="T:System.Data.IDataReader" />.
        /// </summary>
        /// <remarks>
        /// This is typically used when the results of a query are not processed by Dapper, for example, used to fill a
        /// <see cref="T:System.Data.DataTable" /> or <see cref="T:DataSet" />.
        /// </remarks>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// An <see cref="T:System.Data.IDataReader" /> that can be used to iterate over the results of the SQL query.
        /// </returns>
        IDataReader ExecuteReader(CommandDefinition command);

        /// <summary>
        /// Execute parameterized SQL and return an <see cref="T:System.Data.IDataReader" />
        /// </summary>
        /// <remarks>
        /// This is typically used when the results of a query are not processed by Dapper, for example, used to fill a
        /// <see cref="T:System.Data.DataTable" /> or <see cref="T:DataSet" />.
        /// </remarks>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <param name="commandBehavior">
        /// The SQL command behavior.
        /// </param>
        /// <returns>
        /// An <see cref="T:System.Data.IDataReader" /> that can be used to iterate over the results of the SQL query.
        /// </returns>
        IDataReader ExecuteReader(CommandDefinition command, CommandBehavior commandBehavior);

        /// <summary>
        /// Return a sequence of dynamic objects with properties matching the columns.
        /// </summary>
        /// <remarks>
        /// Note: each row can be accessed via "dynamic", or by casting to an 
        /// <see cref="System.Collections.Generic.IDictionary{String,Object}" />.
        /// </remarks>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="buffered">
        /// Determines whether to execute SQL and buffer the entire reader on return.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A sequence of dynamic objects with properties matching the columns.
        /// </returns>
        IEnumerable<object> Query(
            string sql,
            object param = null,
            bool buffered = true,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Return a dynamic object with properties matching the columns.
        /// </summary>
        /// <remarks>
        /// Note: the row can be accessed via "dynamic", or by casting to an 
        /// <see cref="System.Collections.Generic.IDictionary{String,Object}" />.
        /// </remarks>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A dynamic object with properties matching the columns.
        /// </returns>
        object QueryFirst(
            string sql, 
            object param = null, 
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Return a dynamic object with properties matching the columns
        /// </summary>
        /// <remarks>
        /// Note: the row can be accessed via "dynamic", or by casting to an 
        /// <see cref="System.Collections.Generic.IDictionary{String,Object}" />.
        /// </remarks>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A dynamic object with properties matching the columns.
        /// </returns>
        object QueryFirstOrDefault(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Return a dynamic object with properties matching the columns.
        /// </summary>
        /// <remarks>
        /// Note: the row can be accessed via "dynamic", or by casting to an 
        /// <see cref="System.Collections.Generic.IDictionary{String,Object}" />.
        /// </remarks>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A dynamic object with properties matching the columns.
        /// </returns>
        object QuerySingle(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Return a dynamic object with properties matching the columns
        /// </summary>
        /// <remarks>
        /// Note: the row can be accessed via "dynamic", or by casting to an 
        /// <see cref="System.Collections.Generic.IDictionary{String,Object}" />.
        /// </remarks>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A dynamic object with properties matching the columns.
        /// </returns>
        object QuerySingleOrDefault(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Executes a query, returning the data typed as per <typeparam name="T"></typeparam>.
        /// </summary>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="buffered">
        /// Determines whether to execute SQL and buffer the entire reader on return.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A sequence of data of the supplied type.
        /// </returns>
        IEnumerable<T> Query<T>(
            string sql,
            object param = null,
            bool buffered = true,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Executes a single-row query, returning the data typed as per <typeparam name="T"></typeparam>.
        /// </summary>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A sequence of data of the supplied type; if a basic type is queried then the data from the first
        /// column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        T QueryFirst<T>(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Executes a single-row query, returning the data typed as per <typeparam name="T"></typeparam>.
        /// </summary>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A sequence of data of the supplied type; if a basic type is queried then the data from the first
        /// column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        T QueryFirstOrDefault<T>(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Executes a single-row query, returning the data typed as per <typeparam name="T"></typeparam>.
        /// </summary>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A sequence of data of the supplied type; if a basic type is queried then the data from the first
        /// column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        T QuerySingle<T>(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Executes a single-row query, returning the data typed as per <typeparam name="T"></typeparam>.
        /// </summary>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A sequence of data of the supplied type; if a basic type is queried then the data from the first
        /// column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        T QuerySingleOrDefault<T>(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Executes a single-row query, returning the data typed as per the specified type.
        /// </summary>
        /// <param name="type">
        /// The expected return type.
        /// </param>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="buffered">
        /// Determines whether to execute SQL and buffer the entire reader on return.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A sequence of data of the supplied type; if a basic type is queried then the data from the first
        /// column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        IEnumerable<object> Query(
            Type type,
            string sql,
            object param = null,
            bool buffered = true,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Executes a single-row query, returning the data typed as per the specified type.
        /// </summary>
        /// <param name="type">
        /// The expected return type.
        /// </param>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A sequence of data of the supplied type; if a basic type is queried then the data from the first
        /// column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        object QueryFirst(
            Type type,
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Executes a single-row query, returning the data typed as per the specified type.
        /// </summary>
        /// <param name="type">
        /// The expected return type.
        /// </param>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A sequence of data of the supplied type; if a basic type is queried then the data from the first
        /// column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        object QueryFirstOrDefault(
            Type type,
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Executes a single-row query, returning the data typed as per the specified type.
        /// </summary>
        /// <param name="type">
        /// The expected return type.
        /// </param>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A sequence of data of the supplied type; if a basic type is queried then the data from the first
        /// column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        object QuerySingle(
            Type type,
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Executes a single-row query, returning the data typed as per the specified type.
        /// </summary>
        /// <param name="type">
        /// The expected return type.
        /// </param>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A sequence of data of the supplied type; if a basic type is queried then the data from the first
        /// column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        object QuerySingleOrDefault(
            Type type,
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Executes a query, returning the data typed as per <typeparam name="T"></typeparam>.
        /// </summary>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// A sequence of data of the supplied type; if a basic type is queried then the data from the first
        /// column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        IEnumerable<T> Query<T>(CommandDefinition command);

        /// <summary>
        /// Executes a query, returning the data typed as per <typeparam name="T"></typeparam>.
        /// </summary>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// A single instance or null of the supplied type; if a basic type is queried then the data from the
        /// first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        T QueryFirst<T>(CommandDefinition command);

        /// <summary>
        /// Executes a query, returning the data typed as per <typeparam name="T"></typeparam>
        /// </summary>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// A single or null instance of the supplied type; if a basic type is queried then the data from the
        /// first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        T QueryFirstOrDefault<T>(CommandDefinition command);

        /// <summary>
        /// Executes a query, returning the data typed as per <typeparam name="T"></typeparam>.
        /// </summary>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// A single instance of the supplied type; if a basic type is queried then the data from the first
        /// column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        T QuerySingle<T>(CommandDefinition command);

        /// <summary>
        /// Executes a query, returning the data typed as per <typeparam name="T"></typeparam>.
        /// </summary>
        /// <typeparam name="T">
        /// The return type.
        /// </typeparam>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// A single instance of the supplied type; if a basic type is queried then the data from the first
        /// column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        T QuerySingleOrDefault<T>(CommandDefinition command);

        /// <summary>
        /// Execute a command that returns multiple result sets, and access each in turn
        /// </summary>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A <see cref="Dapper.SqlMapper.GridReader"/> containing multiple result sets.
        /// </returns>
        SqlMapper.GridReader QueryMultiple(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Execute a command that returns multiple result sets, and access each in turn.
        /// </summary>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// A <see cref="Dapper.SqlMapper.GridReader"/> containing multiple result sets.
        /// </returns>
        SqlMapper.GridReader QueryMultiple(CommandDefinition command);

        /// <summary>
        /// Maps a query to objects.
        /// </summary>
        /// <typeparam name="TFirst">
        /// The first type in the record set.
        /// </typeparam>
        /// <typeparam name="TSecond">
        /// The second type in the record set.
        /// </typeparam>
        /// <typeparam name="TReturn">
        /// The return type.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="map">
        /// The delegate to map a single row to multiple objects.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="buffered">
        /// Determines whether to execute SQL and buffer the entire reader on return.
        /// </param>
        /// <param name="splitOn">
        /// The field we should split and read the second object from (default: id).
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A sequence of data of the supplied type;
        /// </returns>
        IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(
            string sql,
            Func<TFirst, TSecond, TReturn> map,
            object param = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Perform a multi mapping query with 3 input parameters.
        /// </summary>
        /// <typeparam name="TFirst">
        /// The first type in the record set.
        /// </typeparam>
        /// <typeparam name="TSecond">
        /// The second type in the record set.
        /// </typeparam>
        /// <typeparam name="TThird">
        /// The third type in the record set.
        /// </typeparam>
        /// <typeparam name="TReturn">
        /// The return type.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="map">
        /// The delegate to map a single row to multiple objects.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="buffered">
        /// Determines whether to execute SQL and buffer the entire reader on return.
        /// </param>
        /// <param name="splitOn">
        /// The field we should split and read the second object from (default: id).
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A sequence of data of the supplied type;
        /// </returns>
        IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(
            string sql,
            Func<TFirst, TSecond, TThird, TReturn> map,
            object param = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Perform a multi mapping query with 4 input parameters.
        /// </summary>
        /// <typeparam name="TFirst">
        /// The first type in the record set.
        /// </typeparam>
        /// <typeparam name="TSecond">
        /// The second type in the record set.
        /// </typeparam>
        /// <typeparam name="TThird">
        /// The third type in the record set.
        /// </typeparam>
        /// <typeparam name="TFourth">
        /// The fourth type in the record set.
        /// </typeparam>
        /// <typeparam name="TReturn">
        /// The return type.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="map">
        /// The delegate to map a single row to multiple objects.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="buffered">
        /// Determines whether to execute SQL and buffer the entire reader on return.
        /// </param>
        /// <param name="splitOn">
        /// The field we should split and read the second object from (default: id).
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A sequence of data of the supplied type;
        /// </returns>
        IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(
            string sql,
            Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
            object param = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Perform a multi mapping query with 5 input parameters.
        /// </summary>
        /// <typeparam name="TFirst">
        /// The first type in the record set.
        /// </typeparam>
        /// <typeparam name="TSecond">
        /// The second type in the record set.
        /// </typeparam>
        /// <typeparam name="TThird">
        /// The third type in the record set.
        /// </typeparam>
        /// <typeparam name="TFourth">
        /// The fourth type in the record set.
        /// </typeparam>
        /// <typeparam name="TFifth">
        /// The fifth type in the record set.
        /// </typeparam>
        /// <typeparam name="TReturn">
        /// The return type.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="map">
        /// The delegate to map a single row to multiple objects.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="buffered">
        /// Determines whether to execute SQL and buffer the entire reader on return.
        /// </param>
        /// <param name="splitOn">
        /// The field we should split and read the second object from (default: id).
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A sequence of data of the supplied type;
        /// </returns>
        IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
            string sql,
            Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
            object param = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Perform a multi mapping query with 6 input parameters.
        /// </summary>
        /// <typeparam name="TFirst">
        /// The first type in the record set.
        /// </typeparam>
        /// <typeparam name="TSecond">
        /// The second type in the record set.
        /// </typeparam>
        /// <typeparam name="TThird">
        /// The third type in the record set.
        /// </typeparam>
        /// <typeparam name="TFourth">
        /// The fourth type in the record set.
        /// </typeparam>
        /// <typeparam name="TFifth">
        /// The fifth type in the record set.
        /// </typeparam>
        /// <typeparam name="TSixth">
        /// The sixth type in the record set.
        /// </typeparam>
        /// <typeparam name="TReturn">
        /// The return type.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="map">
        /// The delegate to map a single row to multiple objects.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="buffered">
        /// Determines whether to execute SQL and buffer the entire reader on return.
        /// </param>
        /// <param name="splitOn">
        /// The field we should split and read the second object from (default: id).
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A sequence of data of the supplied type;
        /// </returns>
        IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(
            string sql,
            Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map,
            object param = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Perform a multi mapping query with 7 input parameters.
        /// </summary>
        /// <typeparam name="TFirst">
        /// The first type in the record set.
        /// </typeparam>
        /// <typeparam name="TSecond">
        /// The second type in the record set.
        /// </typeparam>
        /// <typeparam name="TThird">
        /// The third type in the record set.
        /// </typeparam>
        /// <typeparam name="TFourth">
        /// The fourth type in the record set.
        /// </typeparam>
        /// <typeparam name="TFifth">
        /// The fifth type in the record set.
        /// </typeparam>
        /// <typeparam name="TSixth">
        /// The sixth type in the record set.
        /// </typeparam>
        /// <typeparam name="TSeventh">
        /// The seventh type in the record set.
        /// </typeparam>
        /// <typeparam name="TReturn">
        /// The return type.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="map">
        /// The delegate to map a single row to multiple objects.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="buffered">
        /// Determines whether to execute SQL and buffer the entire reader on return.
        /// </param>
        /// <param name="splitOn">
        /// The field we should split and read the second object from (default: id).
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A sequence of data of the supplied type;
        /// </returns>
        IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
            string sql,
            Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
            object param = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null);

        /// <summary>
        /// Perform a multi mapping query with arbitrary input parameters.
        /// </summary>
        /// <typeparam name="TReturn">
        /// The return type.
        /// </typeparam>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="types">
        /// The array of types in the record set.
        /// </param>
        /// <param name="map">
        /// The delegate to map a single row to multiple objects.
        /// </param>
        /// <param name="param">
        /// The SQL query parameters.
        /// </param>
        /// <param name="buffered">
        /// Determines whether to execute SQL and buffer the entire reader on return.
        /// </param>
        /// <param name="splitOn">
        /// The field we should split and read the second object from (default: id).
        /// </param>
        /// <param name="commandTimeout">
        /// The number of seconds before command execution timeout.
        /// </param>
        /// <param name="commandType">
        /// Determines whether the command is a stored procedure or a batch.
        /// </param>
        /// <returns>
        /// A sequence of data of the supplied type;
        /// </returns>
        IEnumerable<TReturn> Query<TReturn>(
            string sql,
            Type[] types,
            Func<object[], TReturn> map,
            object param = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null);
    }
}