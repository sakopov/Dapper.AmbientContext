// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientDbContext.Proxy.cs">
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

    /// <summary>
    /// Represents the type that holds details about the active database context and
    /// manages its lifetime.
    /// </summary>
    public sealed partial class AmbientDbContext
    {
        #region Dapper Proxy Members

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
        public int Execute(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            PrepareConnectionAndTransaction();

            return Connection.Execute(sql, param, Transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Execute parameterized SQL.
        /// </summary>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// Number of rows affected.
        /// </returns>
        public int Execute(CommandDefinition command)
        {
            PrepareConnectionAndTransaction();

            return Connection.Execute(InjectTransaction(command));
        }

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
        public object ExecuteScalar(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            PrepareConnectionAndTransaction();

            return Connection.ExecuteScalar(sql, param, Transaction, commandTimeout, commandType);
        }

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
        public T ExecuteScalar<T>(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            PrepareConnectionAndTransaction();

            return Connection.ExecuteScalar<T>(sql, param, Transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// The first cell selected.
        /// </returns>
        public object ExecuteScalar(CommandDefinition command)
        {
            PrepareConnectionAndTransaction();

            return Connection.ExecuteScalar(InjectTransaction(command));
        }

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
        public T ExecuteScalar<T>(CommandDefinition command)
        {
            PrepareConnectionAndTransaction();

            return Connection.ExecuteScalar<T>(InjectTransaction(command));
        }

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
        public IDataReader ExecuteReader(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            PrepareConnectionAndTransaction();

            return Connection.ExecuteReader(sql, param, Transaction, commandTimeout, commandType);
        }

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
        public IDataReader ExecuteReader(CommandDefinition command)
        {
            PrepareConnectionAndTransaction();

            return Connection.ExecuteReader(InjectTransaction(command));
        }

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
        public IDataReader ExecuteReader(CommandDefinition command, CommandBehavior commandBehavior)
        {
            PrepareConnectionAndTransaction();

            return Connection.ExecuteReader(InjectTransaction(command), commandBehavior);
        }

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
        public IEnumerable<object> Query(
            string sql,
            object param = null,
            bool buffered = true,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            PrepareConnectionAndTransaction();

            return Connection.Query(sql, param, Transaction, buffered, commandTimeout, commandType);
        }

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
        public object QueryFirst(
            string sql, 
            object param = null, 
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            PrepareConnectionAndTransaction();

            return Connection.QueryFirst(sql, param, Transaction, commandTimeout, commandType);
        }

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
        public object QueryFirstOrDefault(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            PrepareConnectionAndTransaction();

            return Connection.QueryFirstOrDefault(sql, param, Transaction, commandTimeout, commandType);
        }

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
        public object QuerySingle(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            PrepareConnectionAndTransaction();

            return Connection.QuerySingle(sql, param, Transaction, commandTimeout, commandType);
        }

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
        public object QuerySingleOrDefault(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            PrepareConnectionAndTransaction();

            return Connection.QuerySingleOrDefault(sql, param, Transaction, commandTimeout, commandType);
        }

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
        public IEnumerable<T> Query<T>(
            string sql,
            object param = null,
            bool buffered = true,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            PrepareConnectionAndTransaction();

            return Connection.Query<T>(sql, param, Transaction, buffered, commandTimeout, commandType);
        }

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
        public T QueryFirst<T>(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            PrepareConnectionAndTransaction();

            return Connection.QueryFirst<T>(sql, param, Transaction, commandTimeout, commandType);
        }

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
        public T QueryFirstOrDefault<T>(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            PrepareConnectionAndTransaction();

            return Connection.QueryFirstOrDefault<T>(sql, param, Transaction, commandTimeout, commandType);
        }

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
        public T QuerySingle<T>(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            PrepareConnectionAndTransaction();

            return Connection.QuerySingle<T>(sql, param, Transaction, commandTimeout, commandType);
        }

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
        public T QuerySingleOrDefault<T>(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            PrepareConnectionAndTransaction();

            return Connection.QuerySingleOrDefault<T>(sql, param, Transaction, commandTimeout, commandType);
        }

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
        public IEnumerable<object> Query(
            Type type,
            string sql,
            object param = null,
            bool buffered = true,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            PrepareConnectionAndTransaction();

            return Connection.Query(type, sql, param, Transaction, buffered, commandTimeout, commandType);
        }

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
        public object QueryFirst(
            Type type,
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            PrepareConnectionAndTransaction();

            return Connection.QueryFirst(type, sql, param, Transaction, commandTimeout, commandType);
        }

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
        public object QueryFirstOrDefault(
            Type type,
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            PrepareConnectionAndTransaction();

            return Connection.QueryFirstOrDefault(type, sql, param, Transaction, commandTimeout, commandType);
        }

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
        public object QuerySingle(
            Type type,
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            PrepareConnectionAndTransaction();

            return Connection.QuerySingle(type, sql, param, Transaction, commandTimeout, commandType);
        }

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
        public object QuerySingleOrDefault(
            Type type,
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            PrepareConnectionAndTransaction();

            return Connection.QuerySingleOrDefault(type, sql, param, Transaction, commandTimeout, commandType);
        }

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
        public IEnumerable<T> Query<T>(CommandDefinition command)
        {
            PrepareConnectionAndTransaction();

            return Connection.Query<T>(InjectTransaction(command));
        }

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
        public T QueryFirst<T>(CommandDefinition command)
        {
            PrepareConnectionAndTransaction();

            return Connection.QueryFirst<T>(InjectTransaction(command));
        }

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
        public T QueryFirstOrDefault<T>(CommandDefinition command)
        {
            PrepareConnectionAndTransaction();

            return Connection.QueryFirstOrDefault<T>(InjectTransaction(command));
        }

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
        public T QuerySingle<T>(CommandDefinition command)
        {
            PrepareConnectionAndTransaction();

            return Connection.QuerySingle<T>(InjectTransaction(command));
        }

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
        public T QuerySingleOrDefault<T>(CommandDefinition command)
        {
            PrepareConnectionAndTransaction();

            return Connection.QuerySingleOrDefault<T>(InjectTransaction(command));
        }

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
        public SqlMapper.GridReader QueryMultiple(
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            PrepareConnectionAndTransaction();

            return Connection.QueryMultiple(sql, param, Transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Execute a command that returns multiple result sets, and access each in turn.
        /// </summary>
        /// <param name="command">
        /// The SQL command definition.
        /// </param>
        /// <returns>
        /// A <see cref="Dapper.SqlMapper.GridReader"/> containing multiple result sets.
        /// </returns>
        public SqlMapper.GridReader QueryMultiple(CommandDefinition command)
        {
            PrepareConnectionAndTransaction();

            return Connection.QueryMultiple(InjectTransaction(command));
        }

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
        public IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(
            string sql,
            Func<TFirst, TSecond, TReturn> map,
            object param = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            PrepareConnectionAndTransaction();

            return Connection.Query(sql, map, param, Transaction, buffered, splitOn, commandTimeout, commandType);
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
        /// A sequence of data of the supplied type;
        /// </returns>
        public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(
            string sql,
            Func<TFirst, TSecond, TThird, TReturn> map,
            object param = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            PrepareConnectionAndTransaction();

            return Connection.Query(sql, map, param, Transaction, buffered, splitOn, commandTimeout, commandType);
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
        /// A sequence of data of the supplied type;
        /// </returns>
        public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(
            string sql,
            Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
            object param = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            PrepareConnectionAndTransaction();

            return Connection.Query(sql, map, param, Transaction, buffered, splitOn, commandTimeout, commandType);
        }

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
        public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
            string sql,
            Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
            object param = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            PrepareConnectionAndTransaction();

            return Connection.Query(sql, map, param, Transaction, buffered, splitOn, commandTimeout, commandType);
        }

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
        public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(
            string sql,
            Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map,
            object param = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            PrepareConnectionAndTransaction();

            return Connection.Query(sql, map, param, Transaction, buffered, splitOn, commandTimeout, commandType);
        }

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
        public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(
            string sql,
            Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map,
            object param = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            PrepareConnectionAndTransaction();

            return Connection.Query(sql, map, param, Transaction, buffered, splitOn, commandTimeout, commandType);
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
        /// A sequence of data of the supplied type;
        /// </returns>
        public IEnumerable<TReturn> Query<TReturn>(
            string sql,
            Type[] types,
            Func<object[], TReturn> map,
            object param = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            PrepareConnectionAndTransaction();

            return Connection.Query(sql, types, map, param, Transaction, buffered, splitOn, commandTimeout, commandType);
        }

        #endregion

        /// <summary>
        /// Opens the database connection and starts a database transaction on the
        /// terminating (parent == null) ambient database contexts.
        /// </summary>
        internal void PrepareConnectionAndTransaction()
        {
            // The parent itself with a closed connection
            if (Parent == null && Connection.State != ConnectionState.Open)
            {
                Connection.Open();

                if (!Suppress)
                {
                    Transaction = Connection.BeginTransaction(IsolationLevel);
                }
            }

            // Has a parent but their connection was never opened
            if (Parent != null && Parent.Connection.State == ConnectionState.Closed)
            {
                Parent.Connection.Open();

                if (!Parent.Suppress)
                {
                    Parent.Transaction = Parent.Connection.BeginTransaction(Parent.IsolationLevel);
                }
            }

            // Opened the parent connection, now inherit their transaction
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