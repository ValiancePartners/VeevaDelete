// MP, 07/01/2019, Mantis 0001782, Vault Binder to Binder Links, merge into MigrationUtils repository
//-----------------------------------------------------------------------
// <copyright file="ItemSourceException.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RestUtility.Api
{
    /// <summary>
    /// Exception class for incorrect data sent to Vault Delete API
    /// </summary>
    [Serializable]
    public class ItemSourceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemSourceException" /> class.
        /// default constructor
        /// </summary>
        public ItemSourceException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemSourceException" /> class.
        /// standard constructor with error message
        /// </summary>
        /// <param name="message">the error message</param>
        public ItemSourceException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemSourceException" /> class.
        /// standard constructor with error message and original exception.
        /// </summary>
        /// <param name="message">the error message</param>
        /// <param name="inner">the original exception</param>
        public ItemSourceException(string message, Exception inner)
            : base(message, inner)
        {
        }

        // Without this constructor, deserialization will fail
        protected ItemSourceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

    }
}
// end changes by MP on 07/01/2019