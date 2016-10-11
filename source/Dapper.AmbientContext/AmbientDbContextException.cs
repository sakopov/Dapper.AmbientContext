// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientDbContextException.cs">
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
//   Represents the type that is responsible for creating ambient database context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Dapper.AmbientContext
{
    using System;

    /// <summary>
    /// Represents exceptions thrown during the initialization or lifetime of the ambient database context.
    /// </summary>
    public sealed class AmbientDbContextException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientDbContextException"/> class with 
        /// the specified message.
        /// </summary>
        /// <param name="message">
        /// A helpful message describing the exception.
        /// </param>
        public AmbientDbContextException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientDbContextException"/> class with 
        /// the specified message and exception.
        /// </summary>
        /// <param name="message">
        /// A helpful message describing the exception.
        /// </param>
        /// <param name="innerException">
        /// The underlying exception.
        /// </param>
        public AmbientDbContextException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}