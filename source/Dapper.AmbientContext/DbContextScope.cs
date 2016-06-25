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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;

namespace Dapper.AmbientContext
{
    /// <summary>
    /// Implements database context scope.
    /// </summary>
    public sealed partial class DbContextScope : IDbContextScope
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private readonly DbContextScopeOption _option;
        private readonly IsolationLevel _isolationLevel;
        private readonly IDbContextScope _parent;

        private const string LogicalCallContextKey = "DbContextScopeStack";
        private static readonly ConditionalWeakTable<string, IImmutableStack<IDbContextScope>> DbContextScopeTable = new ConditionalWeakTable<string, IImmutableStack<IDbContextScope>>();

        /// <summary>
        /// Creates a new instance of the <see cref="T:DbContextScope"/> class. This 
        /// class can only be created via <see cref="T:DbContextScopeFactory"/>.
        /// </summary>
        /// <param name="connection">
        /// The database connection, only if creating new connection.
        /// </param>
        /// <param name="option">
        /// The database context scope option.
        /// </param>
        /// <param name="isolationLevel">
        /// The database transaction isolation level.
        /// </param>
        internal DbContextScope(IDbConnection connection = null, DbContextScopeOption option = DbContextScopeOption.Join, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            _option = option;
            _isolationLevel = isolationLevel;
            _connection = connection;

            if (_option.HasFlag(DbContextScopeOption.New) && _connection == null)
            {
                throw new InvalidOperationException("Must specify a database connection when creating new database context scope.");
            }

            if (_option.HasFlag(DbContextScopeOption.New) && _option.HasFlag(DbContextScopeOption.Join))
            {
                throw new InvalidOperationException("The database context scope option should be set to either new or join.");
            }

            if (_option.HasFlag(DbContextScopeOption.Join))
            {
                if (DbContextScopeStack.IsEmpty)
                {
                    throw new InvalidOperationException("Could not find available database context scope to join.");
                }

                _parent = DbContextScopeStack.Peek();
            }

            DbContextScopeStack = DbContextScopeStack.Push(this);
        }

        /// <summary>
        /// Gets transaction isolation level at parent or current level.
        /// </summary>
        internal IsolationLevel IsolationLevel => _parent == null ? _isolationLevel : ((DbContextScope) _parent).IsolationLevel;

        /// <summary>
        /// Gets database context scope option at parent or current level.
        /// </summary>
        internal DbContextScopeOption Option => _parent == null ? _option : ((DbContextScope) _parent).Option;

        /// <summary>
        /// Gets database connection at parent or current level.
        /// </summary>
        internal IDbConnection Connection => _parent == null ? _connection : ((DbContextScope)_parent).Connection;

        /// <summary>
        /// Gets the parent scope, if available.
        /// </summary>
        internal IDbContextScope Parent => _parent;

        /// <summary>
        /// Gets or sets the database transaction at parent or current level.
        /// </summary>
        internal IDbTransaction Transaction
        {
            get
            {
                return _parent == null ? _transaction : ((DbContextScope) _parent).Transaction;
            }

            set
            {
                if (_parent == null)
                {
                    _transaction = value;
                }
                else
                {
                    ((DbContextScope)_parent).Transaction = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the immutable stack containing active database context scopes.
        /// </summary>
        internal static IImmutableStack<IDbContextScope> DbContextScopeStack
        {
            get
            {
                var dbContextScopeKey = CallContext.LogicalGetData(LogicalCallContextKey) as DbContextScopeStackWrapper;

                if (dbContextScopeKey == null)
                {
                    dbContextScopeKey = new DbContextScopeStackWrapper(Guid.NewGuid().ToString("N"));

                    CallContext.LogicalSetData(LogicalCallContextKey, dbContextScopeKey);
                }

                IImmutableStack<IDbContextScope> dbContextScopeStack;

                if (DbContextScopeTable.TryGetValue(dbContextScopeKey.Value, out dbContextScopeStack))
                {
                    return dbContextScopeStack;
                }

                dbContextScopeStack = ImmutableStack.Create<IDbContextScope>();

                DbContextScopeTable.Add(dbContextScopeKey.Value, dbContextScopeStack);

                return dbContextScopeStack;
            }

            set
            {
                var dbContextScopeKey = CallContext.LogicalGetData(LogicalCallContextKey) as DbContextScopeStackWrapper;

                if (dbContextScopeKey == null)
                {
                    dbContextScopeKey = new DbContextScopeStackWrapper(Guid.NewGuid().ToString("N"));

                    CallContext.LogicalSetData(LogicalCallContextKey, dbContextScopeKey);
                }

                IImmutableStack<IDbContextScope> dbContextScopeStack;

                if (DbContextScopeTable.TryGetValue(dbContextScopeKey.Value, out dbContextScopeStack))
                {
                    DbContextScopeTable.Remove(dbContextScopeKey.Value);
                }

                DbContextScopeTable.Add(dbContextScopeKey.Value, value);
            }
        }

        /// <summary>
        /// Opens database connection and begins transaction, if not suppressed.
        /// </summary>
        internal void Open()
        {
            PrepareConnection();
        }

        /// <summary>
        /// Clears logical call context and the corresponding database context scope stack in the weak table.
        /// </summary>
        private static void ClearDbContextScopeStack()
        {
            var dbContextScopeKey = CallContext.LogicalGetData(LogicalCallContextKey) as DbContextScopeStackWrapper;

            if (dbContextScopeKey != null)
            {
                CallContext.LogicalSetData(LogicalCallContextKey, null);

                DbContextScopeTable.Remove(dbContextScopeKey.Value);
            }
        }

        /// <summary>
        /// Opens a database connection and starts a new transaction.
        /// </summary>
        private void PrepareConnection()
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }

            if (Transaction == null)
            {
                if (!Option.HasFlag(DbContextScopeOption.Suppress))
                {
                    Transaction = Connection.BeginTransaction(IsolationLevel);
                }
            }
        }

        /// <summary>
        /// Wraps values in logical call context to enable access across AppDomains.
        /// </summary>
        private class DbContextScopeStackWrapper : MarshalByRefObject
        {
            public DbContextScopeStackWrapper(string value)
            {
                Value = value;
            }

            public string Value { get; private set; }
        }

        #region IDbContextScope Members

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
        IEnumerable<T> IDbContext.Query<T>(string query, object param, CommandType? commandType)
        {
            PrepareConnection();

            return Connection.Query<T>(query, param, Transaction, commandType: commandType);
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
        IEnumerable<dynamic> IDbContext.Query(string query, object param, CommandType? commandType)
        {
            PrepareConnection();

            return Connection.Query(query, param, Transaction, commandType: commandType);
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
        int IDbContext.Execute(string sql, object param, CommandType? commandType)
        {
            PrepareConnection();

            return Connection.Execute(sql, param, Transaction, commandType: commandType);
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
        T IDbContext.ExecuteScalar<T>(string sql, object param, CommandType? commandType)
        {
            PrepareConnection();

            return Connection.ExecuteScalar<T>(sql, param, Transaction, commandType: commandType);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (_parent != null)
            {
                DbContextScopeStack = DbContextScopeStack.Pop();
                return;
            }

            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }

            if (_connection != null && _connection.State != ConnectionState.Closed)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }

            if (DbContextScopeStack.IsEmpty)
            {
                return;
            }

            DbContextScopeStack = DbContextScopeStack.Pop();

            if (DbContextScopeStack.IsEmpty)
            {
                ClearDbContextScopeStack();
            }
        }

        /// <summary>
        /// Commits changes in scope.
        /// </summary>
        public void Commit()
        {
            if (_parent != null)
            {
                return;
            }

            try
            {
                if (_transaction != null)
                {
                    _transaction.Commit();
                    _transaction.Dispose();
                    _transaction = null;
                }
            }
            catch (Exception)
            {
                Rollback();

                throw;
            }
        }

        /// <summary>
        /// Rolls back all changes in scope.
        /// </summary>
        public void Rollback()
        {
            if (_parent != null)
            {
                return;
            }

            try
            {
                if (_transaction != null)
                {
                    _transaction.Rollback();
                    _transaction.Dispose();
                    _transaction = null;
                }
            }
            catch (Exception)
            {
                if (_transaction != null && _transaction.Connection != null)
                {
                    _transaction.Dispose();
                    _transaction = null;
                }

                throw;
            }
        }

        #endregion
    }
}
