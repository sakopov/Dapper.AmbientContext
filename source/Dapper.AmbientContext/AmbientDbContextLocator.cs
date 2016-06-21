//
// AmbientDbContextLocator.cs
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

namespace Dapper.AmbientContext
{
    /// <summary>
    /// Implements ambient database context locator which retrieves active database
    /// context from a stack in logical call context.
    /// </summary>
    public sealed class AmbientDbContextLocator : IAmbientDbContextLocator
    {
        /// <summary>
        /// Retrieves current ambient database context.
        /// </summary>
        /// <returns>
        /// The current ambient database context.
        /// </returns>
        public IDbContext Get()
        {
            if (DbContextScope.DbContextScopeStack.IsEmpty)
            {
                throw new InvalidOperationException("Cannot find active database context scope. Make sure a context scope is created before attempting to run a query.");
            }

            var dbContextScope = DbContextScope.DbContextScopeStack.Peek();

            return new DbContext(dbContextScope);
        }
    }
}