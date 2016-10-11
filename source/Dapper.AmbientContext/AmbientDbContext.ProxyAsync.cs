// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientDbContext.ProxyAsync.cs">
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
//  Represents the type that holds details about the active database context and
//  manages its lifetime.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Dapper.AmbientContext
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the type that holds details about the active database context and
    /// manages its lifetime.
    /// </summary>
    public sealed partial class AmbientDbContext
    {
        #region Dapper Proxy Members

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
        public async Task<IEnumerable<dynamic>> QueryAsync(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            await PrepareConnectionAndTransactionAsync(default(CancellationToken)).ConfigureAwait(false);

            return await Connection.QueryAsync(sql, param, Transaction, commandTimeout, commandType).ConfigureAwait(false);
        }

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
        public async Task<IEnumerable<dynamic>> QueryAsync(CommandDefinition command)
        {
            await PrepareConnectionAndTransactionAsync(command.CancellationToken).ConfigureAwait(false);

            return await Connection.QueryAsync(InjectTransaction(command)).ConfigureAwait(false);
        }

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
        public async Task<dynamic> QueryFirstAsync(CommandDefinition command)
        {
            await PrepareConnectionAndTransactionAsync(command.CancellationToken).ConfigureAwait(false);

            return await Connection.QueryFirstAsync(InjectTransaction(command)).ConfigureAwait(false);
        }

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
        public async Task<dynamic> QueryFirstOrDefaultAsync(CommandDefinition command)
        {
            await PrepareConnectionAndTransactionAsync(command.CancellationToken).ConfigureAwait(false);

            return await Connection.QueryFirstOrDefaultAsync(InjectTransaction(command)).ConfigureAwait(false);
        }

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
        public async Task<dynamic> QuerySingleAsync(CommandDefinition command)
        {
            await PrepareConnectionAndTransactionAsync(command.CancellationToken).ConfigureAwait(false);

            return await Connection.QuerySingleAsync(InjectTransaction(command)).ConfigureAwait(false);
        }

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
        public async Task<dynamic> QuerySingleOrDefaultAsync(CommandDefinition command)
        {
            await PrepareConnectionAndTransactionAsync(command.CancellationToken).ConfigureAwait(false);

            return await Connection.QuerySingleOrDefaultAsync(InjectTransaction(command)).ConfigureAwait(false);
        }

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
        public async Task<IEnumerable<T>> QueryAsync<T>(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            await PrepareConnectionAndTransactionAsync(default(CancellationToken)).ConfigureAwait(false);

            return await Connection.QueryAsync<T>(sql, param, Transaction, commandTimeout, commandType).ConfigureAwait(false);
        }

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
        public async Task<T> QueryFirstAsync<T>(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            await PrepareConnectionAndTransactionAsync(default(CancellationToken)).ConfigureAwait(false);

            return await Connection.QueryFirstAsync<T>(sql, param, Transaction, commandTimeout, commandType)
                .ConfigureAwait(false);
        }

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
        public async Task<T> QueryFirstOrDefaultAsync<T>(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            await PrepareConnectionAndTransactionAsync(default(CancellationToken)).ConfigureAwait(false);

            return await Connection.QueryFirstOrDefaultAsync<T>(sql, param, Transaction, commandTimeout, commandType)
                .ConfigureAwait(false);
        }

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
        public async Task<T> QuerySingleAsync<T>(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            await PrepareConnectionAndTransactionAsync(default(CancellationToken)).ConfigureAwait(false);

            return await Connection.QuerySingleAsync<T>(sql, param, Transaction, commandTimeout, commandType)
                .ConfigureAwait(false);
        }

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
        public async Task<T> QuerySingleOrDefaultAsync<T>(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            await PrepareConnectionAndTransactionAsync(default(CancellationToken)).ConfigureAwait(false);

            return await Connection.QuerySingleOrDefaultAsync<T>(sql, param, Transaction, commandTimeout, commandType)
                .ConfigureAwait(false);
        }

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
        public async Task<IEnumerable<object>> QueryAsync(
            Type type,
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            await PrepareConnectionAndTransactionAsync(default(CancellationToken)).ConfigureAwait(false);

            return await Connection.QueryAsync(type, sql, param, Transaction, commandTimeout, commandType)
                .ConfigureAwait(false);
        }

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
        public async Task<object> QueryFirstAsync(
            Type type,
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            await PrepareConnectionAndTransactionAsync(default(CancellationToken)).ConfigureAwait(false);

            return await Connection.QueryFirstAsync(type, sql, param, Transaction, commandTimeout, commandType)
                .ConfigureAwait(false);
        }

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
        public async Task<object> QueryFirstOrDefaultAsync(
            Type type,
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            await PrepareConnectionAndTransactionAsync(default(CancellationToken)).ConfigureAwait(false);

            return await Connection.QueryFirstOrDefaultAsync(type, sql, param, Transaction, commandTimeout, commandType)
                .ConfigureAwait(false);
        }

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
        public async Task<object> QuerySingleAsync(
            Type type,
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            await PrepareConnectionAndTransactionAsync(default(CancellationToken)).ConfigureAwait(false);

            return await Connection.QuerySingleAsync(type, sql, param, Transaction, commandTimeout, commandType)
                .ConfigureAwait(false);
        }

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
        public async Task<object> QuerySingleOrDefaultAsync(
            Type type,
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            await PrepareConnectionAndTransactionAsync(default(CancellationToken)).ConfigureAwait(false);

            return await Connection.QuerySingleOrDefaultAsync(type, sql, param, Transaction, commandTimeout, commandType)
                .ConfigureAwait(false);
        }

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
        public async Task<IEnumerable<T>> QueryAsync<T>(CommandDefinition command)
        {
            await PrepareConnectionAndTransactionAsync(command.CancellationToken).ConfigureAwait(false);

            return await Connection.QueryAsync<T>(InjectTransaction(command)).ConfigureAwait(false);
        }

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
        public async Task<IEnumerable<object>> QueryAsync(Type type, CommandDefinition command)
        {
            await PrepareConnectionAndTransactionAsync(command.CancellationToken).ConfigureAwait(false);

            return await Connection.QueryAsync(type, InjectTransaction(command)).ConfigureAwait(false);
        }

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
        public async Task<object> QueryFirstAsync(Type type, CommandDefinition command)
        {
            await PrepareConnectionAndTransactionAsync(command.CancellationToken).ConfigureAwait(false);

            return await Connection.QueryFirstAsync(type, InjectTransaction(command)).ConfigureAwait(false);
        }

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
        public async Task<object> QueryFirstOrDefaultAsync(Type type, CommandDefinition command)
        {
            await PrepareConnectionAndTransactionAsync(command.CancellationToken).ConfigureAwait(false);

            return await Connection.QueryFirstOrDefaultAsync(type, InjectTransaction(command)).ConfigureAwait(false);
        }

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
        public async Task<object> QuerySingleAsync(Type type, CommandDefinition command)
        {
            await PrepareConnectionAndTransactionAsync(command.CancellationToken).ConfigureAwait(false);

            return await Connection.QuerySingleAsync(type, InjectTransaction(command)).ConfigureAwait(false);
        }

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
        public async Task<object> QuerySingleOrDefaultAsync(Type type, CommandDefinition command)
        {
            await PrepareConnectionAndTransactionAsync(command.CancellationToken).ConfigureAwait(false);

            return await Connection.QuerySingleOrDefaultAsync(type, InjectTransaction(command)).ConfigureAwait(false);
        }

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
        public async Task<int> ExecuteAsync(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            await PrepareConnectionAndTransactionAsync(default(CancellationToken)).ConfigureAwait(false);

            return await Connection.ExecuteAsync(sql, param, Transaction, commandTimeout, commandType)
                .ConfigureAwait(false);
        }

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
        public async Task<int> ExecuteAsync(CommandDefinition command)
        {
            await PrepareConnectionAndTransactionAsync(command.CancellationToken).ConfigureAwait(false);

            return await Connection.ExecuteAsync(InjectTransaction(command)).ConfigureAwait(false);
        }

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
        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(
            string sql,
            Func<TFirst, TSecond, TReturn> map,
            object param = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            await PrepareConnectionAndTransactionAsync(default(CancellationToken)).ConfigureAwait(false);

            return await Connection.QueryAsync(sql, map, param, Transaction, buffered, splitOn, commandTimeout, commandType)
                .ConfigureAwait(false);
        }

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
        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(
            CommandDefinition command,
            Func<TFirst, TSecond, TReturn> map,
            string splitOn = "Id")
        {
            await PrepareConnectionAndTransactionAsync(command.CancellationToken).ConfigureAwait(false);

            return await Connection.QueryAsync(InjectTransaction(command), map, splitOn)
                .ConfigureAwait(false);
        }

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
        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(
            string sql,
            Func<TFirst, TSecond, TThird, TReturn> map,
            object param = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            await PrepareConnectionAndTransactionAsync(default(CancellationToken)).ConfigureAwait(false);

            return await Connection.QueryAsync(sql, map, param, Transaction, buffered, splitOn, commandTimeout, commandType)
                .ConfigureAwait(false);
        }

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
        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(
            CommandDefinition command,
            Func<TFirst, TSecond, TThird, TReturn> map,
            string splitOn = "Id")
        {
            await PrepareConnectionAndTransactionAsync(command.CancellationToken).ConfigureAwait(false);

            return await Connection.QueryAsync(InjectTransaction(command), map, splitOn)
                .ConfigureAwait(false);
        }

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
        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(
            string sql, 
            Func<TFirst, TSecond, TThird, TFourth, TReturn> map, 
            object param = null,
            bool buffered = true, 
            string splitOn = "Id", 
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            await PrepareConnectionAndTransactionAsync(default(CancellationToken)).ConfigureAwait(false);

            return await Connection.QueryAsync(sql, map, param, Transaction, buffered, splitOn, commandTimeout, commandType)
                .ConfigureAwait(false);
        }

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
        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(
            CommandDefinition command, 
            Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
            string splitOn = "Id")
        {
            await PrepareConnectionAndTransactionAsync(command.CancellationToken).ConfigureAwait(false);

            return await Connection.QueryAsync(InjectTransaction(command), map, splitOn).ConfigureAwait(false);
        }

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
        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
            string sql, 
            Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
            object param = null, 
            bool buffered = true, 
            string splitOn = "Id",
            int? commandTimeout = null, 
            CommandType? commandType = null)
        {
            await PrepareConnectionAndTransactionAsync(default(CancellationToken)).ConfigureAwait(false);

            return await Connection.QueryAsync(sql, map, param, Transaction, buffered, splitOn, commandTimeout, commandType)
                .ConfigureAwait(false);
        }

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
        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
            CommandDefinition command,
            Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, 
            string splitOn = "Id")
        {
            await PrepareConnectionAndTransactionAsync(command.CancellationToken).ConfigureAwait(false);

            return await Connection.QueryAsync(InjectTransaction(command), map, splitOn).ConfigureAwait(false);
        }

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
        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(
            string sql, 
            Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map,
            object param = null, 
            bool buffered = true, 
            string splitOn = "Id",
            int? commandTimeout = null, 
            CommandType? commandType = null)
        {
            await PrepareConnectionAndTransactionAsync(default(CancellationToken)).ConfigureAwait(false);

            return await Connection.QueryAsync(sql, map, param, Transaction, buffered, splitOn, commandTimeout, commandType)
                .ConfigureAwait(false);
        }

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
        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(
            CommandDefinition command,
            Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, 
            string splitOn = "Id")
        {
            await PrepareConnectionAndTransactionAsync(command.CancellationToken).ConfigureAwait(false);

            return await Connection.QueryAsync(InjectTransaction(command), map, splitOn).ConfigureAwait(false);
        }

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
        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
            string sql,
            Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
            object param = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            await PrepareConnectionAndTransactionAsync(default(CancellationToken)).ConfigureAwait(false);

            return await Connection.QueryAsync(sql, map, param, Transaction, buffered, splitOn, commandTimeout, commandType)
                .ConfigureAwait(false);
        }

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
        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
            CommandDefinition command,
            Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
            string splitOn = "Id")
        {
            await PrepareConnectionAndTransactionAsync(command.CancellationToken).ConfigureAwait(false);

            return await Connection.QueryAsync(InjectTransaction(command), map, splitOn).ConfigureAwait(false);
        }

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
        public async Task<IEnumerable<TReturn>> QueryAsync<TReturn>(
            string sql, 
            Type[] types,
            Func<object[], TReturn> map, 
            object param = null,
            bool buffered = true, 
            string splitOn = "Id", 
            int? commandTimeout = null, 
            CommandType? commandType = null)
        {
            await PrepareConnectionAndTransactionAsync(default(CancellationToken)).ConfigureAwait(false);

            return await Connection.QueryAsync(sql, types, map, param, Transaction, buffered, splitOn, commandTimeout, commandType)
                .ConfigureAwait(false);
        }

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
        public async Task<SqlMapper.GridReader> QueryMultipleAsync(
            string sql,
            object param = null, 
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            await PrepareConnectionAndTransactionAsync(default(CancellationToken)).ConfigureAwait(false);

            return await Connection.QueryMultipleAsync(sql, param, Transaction, commandTimeout, commandType)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Execute a command that returns multiple result sets, and access each in turn.
        /// </summary>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        public async Task<SqlMapper.GridReader> QueryMultipleAsync(CommandDefinition command)
        {
            await PrepareConnectionAndTransactionAsync(command.CancellationToken).ConfigureAwait(false);

            return await Connection.QueryMultipleAsync(InjectTransaction(command)).ConfigureAwait(false);
        }

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
        public async Task<IDataReader> ExecuteReaderAsync(
            string sql, 
            object param = null, 
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            await PrepareConnectionAndTransactionAsync(default(CancellationToken)).ConfigureAwait(false);

            return await Connection.ExecuteReaderAsync(sql, param, Transaction, commandTimeout, commandType)
                .ConfigureAwait(false);
        }

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
        public async Task<IDataReader> ExecuteReaderAsync(CommandDefinition command)
        {
            await PrepareConnectionAndTransactionAsync(command.CancellationToken).ConfigureAwait(false);

            return await Connection.ExecuteReaderAsync(InjectTransaction(command)).ConfigureAwait(false);
        }

        #endregion

        /// <summary>
        /// Asynchronously opens the database connection on a parentless ambient database context
        /// and begins a transaction (if required).
        /// </summary>
        /// <param name="cancellationToken">
        /// The cancellation Token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task PrepareConnectionAndTransactionAsync(CancellationToken cancellationToken)
        {
            if (Parent == null && Connection.State != ConnectionState.Open)
            {
                await((DbConnection)Connection).OpenAsync(cancellationToken).ConfigureAwait(false);

                if (!Suppress)
                {
                    Transaction = Connection.BeginTransaction(IsolationLevel);
                }
            }

            // Has a parent but their connection was never opened
            if (Parent != null && Parent.Connection.State == ConnectionState.Closed)
            {
                await((DbConnection)Parent.Connection).OpenAsync(cancellationToken).ConfigureAwait(false);

                if (!Parent.Suppress)
                {
                    Parent.Transaction = Parent.Connection.BeginTransaction(Parent.IsolationLevel);
                }
            }

            // Opened the parent transaction, now inherit their transaction
            if (Parent != null && Parent.Connection.State == ConnectionState.Open)
            {
                if (Parent.Transaction != null && Transaction == null)
                {
                    Transaction = Parent.Transaction;
                }
            }
        }
    }
}