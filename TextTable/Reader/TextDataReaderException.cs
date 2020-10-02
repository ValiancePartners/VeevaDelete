//-----------------------------------------------------------------------
// <copyright file="TextDataReaderException.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TextTable.Reader
{
    /// <summary>
    /// Exception class for incorrect data sent to Vault Delete API
    /// </summary>
    [Serializable]
    public class TextDataReaderException: Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextDataReaderException" /> class.
        /// default constructor
        /// </summary>
        public TextDataReaderException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextDataReaderException" /> class.
        /// standard constructor with error message
        /// </summary>
        /// <param name="message">the error message</param>
        public TextDataReaderException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextDataReaderException" /> class.
        /// standard constructor with error message and original exception.
        /// </summary>
        /// <param name="message">the error message</param>
        /// <param name="inner">the original exception</param>
        public TextDataReaderException(string message, Exception inner)
            : base(message, inner)
        {
        }

        // Without this constructor, deserialization will fail
        protected TextDataReaderException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

    }
}
