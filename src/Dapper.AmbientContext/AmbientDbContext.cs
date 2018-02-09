// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientDbContext.cs">
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
    using System.Collections.Immutable;
    using System.Data;

    using Storage;

    /// <summary>
    /// Represents the type that holds details about the active database context and
    /// manages its lifetime.
    /// </summary>
    public sealed partial class AmbientDbContext : IAmbientDbContext, IAmbientDbContextQueryProxy
    {
        /// <summary>
        /// The storage helper.
        /// </summary>
        private ContextualStorageHelper _storageHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientDbContext"/> class.
        /// </summary>
        /// <param name="connection">
        /// The database connection to use.
        /// </param>
        /// <param name="join">
        /// Determines whether to join an existing ambient database context or create a new one.
        /// </param>
        /// <param name="suppress">
        /// Determines whether to suppress the database transaction.
        /// </param>
        /// <param name="isolationLevel">
        /// The database transaction isolation level to use.
        /// </param>
        internal AmbientDbContext(IDbConnection connection, bool join, bool suppress, IsolationLevel isolationLevel)
        {
            var immutableStack = PrepareStorageAndStack();

            IsolationLevel = isolationLevel;
            Suppress = suppress;
            Connection = connection;

            if (join)
            {
                if (!immutableStack.IsEmpty)
                {
                    var parent = immutableStack.Peek();

                    Parent = parent;
                    Connection = parent.Connection;
                    Transaction = parent.Transaction;
                    Suppress = parent.Suppress;
                    IsolationLevel = parent.IsolationLevel;
                }
            }

            immutableStack = immutableStack.Push(this);

            _storageHelper.SaveStack(immutableStack);
        }

        /// <summary>
        /// Gets the ambient database context connection. If inheriting from a parent
        /// ambient database context, this property will be assigned to parent's connection.
        /// </summary>
        public IDbConnection Connection { get; private set; }

        /// <summary>
        /// Gets or sets the ambient database context transaction. If inheriting from a parent
        /// ambient database context, this property will be assigned to parent's transaction.
        /// </summary>
        public IDbTransaction Transaction { get; set; }

        /// <summary>
        /// Gets a value indicating whether to suppress the database transaction.
        /// </summary>
        public bool Suppress { get; }

        /// <summary>
        /// Gets the transaction isolation level.
        /// </summary>
        public IsolationLevel IsolationLevel { get; }

        /// <summary>
        /// Gets the parent ambient database context. This property will only be set
        /// if an ambient database context joins an already existing context and inherits its 
        /// state.
        /// </summary>
        internal IAmbientDbContext Parent { get; }

        /// <summary>
        /// Disposes ambient database context.
        /// </summary>
        public void Dispose()
        {
            var immutableStack = _storageHelper.GetStack();

            // This can only happen if the consumer of the library is messing with the storage.
            if (immutableStack.IsEmpty)
            {
                throw new AmbientDbContextException("Could not dispose ambient database context because it does not exist in storage.");
            }

            var topItem = immutableStack.Peek();

            if (this != topItem)
            {
                throw new InvalidOperationException("Could not dispose ambient database context because it is not the active ambient database context. This could occur because ambient database context is being disposed out of order.");
            }

            immutableStack = immutableStack.Pop();

            _storageHelper.SaveStack(immutableStack);

            if (Parent == null)
            {
                if (Transaction != null)
                {
                    Commit();
                }

                if (Connection != null)
                {
                    if (Connection.State == ConnectionState.Open)
                    {
                        Connection.Close();
                    }

                    Connection.Dispose();
                    Connection = null;
                }
            }
        }

        /// <summary>
        /// Commits ambient database context.
        /// </summary>
        public void Commit()
        {
            if (Parent != null)
            {
                return;
            }

            try
            {
                if (Transaction != null)
                {
                    Transaction.Commit();
                    Transaction.Dispose();
                    Transaction = null;
                }
            }
            catch (Exception)
            {
                Rollback();

                throw;
            }
        }

        /// <summary>
        /// Rolls back ambient database context transaction from a pending state.
        /// </summary>
        public void Rollback()
        {
            if (Parent != null)
            {
                return;
            }

            try
            {
                if (Transaction != null)
                {
                    Transaction.Rollback();
                    Transaction.Dispose();
                    Transaction = null;
                }
            }
            catch (Exception)
            {
                if (Transaction != null && Transaction.Connection != null)
                {
                    Transaction.Dispose();
                    Transaction = null;
                }

                throw;
            }
        }

        /// <summary>
        /// Initializes the underlying contextual storage.
        /// </summary>
        /// <returns>
        /// The immutable stack containing active ambient database contexts.
        /// </returns>
        private IImmutableStack<IAmbientDbContext> PrepareStorageAndStack()
        {
            var storage = AmbientDbContextStorageProvider.Storage;

            _storageHelper = new ContextualStorageHelper(storage);

            return _storageHelper.GetStack();
        }

        /// <summary>
        /// Injects current database transaction into the specified command definition.
        /// </summary>
        /// <param name="commandDefinition">
        /// The command definition to inject the current database transaction into.
        /// </param>
        /// <returns>
        /// The command definition with the injected database transaction.
        /// </returns>
        private CommandDefinition InjectTransaction(CommandDefinition commandDefinition)
        {
            if (Transaction == null)
            {
                return commandDefinition;
            }

            var command = new CommandDefinition(
                commandDefinition.CommandText,
                commandDefinition.Parameters,
                Transaction,
                commandDefinition.CommandTimeout,
                commandDefinition.CommandType,
                commandDefinition.Flags,
                commandDefinition.CancellationToken);

            return command;
        }
    }
}