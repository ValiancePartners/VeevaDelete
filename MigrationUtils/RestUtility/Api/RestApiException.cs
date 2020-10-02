// MP, 07/01/2019, Mantis 0001782, Vault Binder to Binder Links, merge into MigrationUtils repository
//-----------------------------------------------------------------------
// <copyright file="RestApiException.cs" company="Valiance Partners">
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
    /// Base exception class for Web API implementations
    /// </summary>
    [Serializable]
    public class RestApiException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestApiException" /> class.
        /// default constructor
        /// </summary>
        public RestApiException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestApiException" /> class.
        /// standard constructor with error message
        /// </summary>
        /// <param name="message">the error message</param>
        public RestApiException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestApiException" /> class.
        /// standard constructor with error message and original exception.
        /// </summary>
        /// <param name="message">the error message</param>
        /// <param name="inner">the original exception</param>
        public RestApiException(string message, Exception inner)
            : base(message, inner)
        {
        }

        // Without this constructor, deserialization will fail
        protected RestApiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
// end changes by MP on 07/01/2019