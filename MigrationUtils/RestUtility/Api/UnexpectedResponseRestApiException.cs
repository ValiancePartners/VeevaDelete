// MP, 07/01/2019, Mantis 0001782, Vault Binder to Binder Links, merge into MigrationUtils repository
//-----------------------------------------------------------------------
// <copyright file="UnexpectedResponseRestApiException.cs" company="Valiance Partners">
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
    /// Low-Level API exception class for when response from API is not as expected (null or missing expected elements)
    /// Indicates either error in API implementation or in response returned by Vault
    /// </summary>
    [Serializable]
    public class UnexpectedResponseRestApiException : RestApiContractException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedResponseRestApiException" /> class.
        /// default constructor
        /// </summary>
        public UnexpectedResponseRestApiException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedResponseRestApiException" /> class.
        /// standard constructor with error message
        /// </summary>
        /// <param name="message">the error message</param>
        public UnexpectedResponseRestApiException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedResponseRestApiException" /> class.
        /// standard constructor with error message and original exception.
        /// </summary>
        /// <param name="message">the error message</param>
        /// <param name="inner">the original exception</param>
        public UnexpectedResponseRestApiException(string message, Exception inner)
            : base(message, inner)
        {
        }

        // Without this constructor, deserialization will fail
        protected UnexpectedResponseRestApiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

    }

}
// end changes by MP on 07/01/2019