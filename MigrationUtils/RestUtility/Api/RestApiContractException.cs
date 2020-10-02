// MP, 07/01/2019, Mantis 0001782, Vault Binder to Binder Links, merge into MigrationUtils repository
//-----------------------------------------------------------------------
// <copyright file="RestApiContractException.cs" company="Valiance Partners">
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
    /// Base Web API exception class fault in API response or request
    /// indicates error in implementation of API caller, error in implementation of API or error in Vault response content
    /// </summary>
    [Serializable]
    public class RestApiContractException : RestApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestApiContractException" /> class.
        /// default constructor
        /// </summary>
        public RestApiContractException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestApiContractException" /> class.
        /// standard constructor with error message
        /// </summary>
        /// <param name="message">the error message</param>
        public RestApiContractException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestApiContractException" /> class.
        /// standard constructor with error message and original exception.
        /// </summary>
        /// <param name="message">the error message</param>
        /// <param name="inner">the original exception</param>
        public RestApiContractException(string message, Exception inner)
            : base(message, inner)
        {
        }

        // Without this constructor, deserialization will fail
        protected RestApiContractException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

    }
}
// end changes by MP on 07/01/2019