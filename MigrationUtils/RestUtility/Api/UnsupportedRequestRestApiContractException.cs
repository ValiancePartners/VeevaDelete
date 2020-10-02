// MP, 07/01/2019, Mantis 0001782, Vault Binder to Binder Links, merge into MigrationUtils repository
//-----------------------------------------------------------------------
// <copyright file="UnsupportedRequestRestApiContractException.cs" company="Valiance Partners">
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
    /// Mid-Level Web API failure exception class intended for when response from API indicates incorrectly formed API request
    /// </summary>
    [Serializable]
    public class UnsupportedRequestRestApiContractException : RestApiContractException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedRequestRestApiContractException" /> class.
        /// default constructor
        /// </summary>
        public UnsupportedRequestRestApiContractException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedRequestRestApiContractException" /> class.
        /// standard constructor with error message
        /// </summary>
        /// <param name="message">the error message</param>
        public UnsupportedRequestRestApiContractException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedRequestRestApiContractException" /> class.
        /// standard constructor with error message and original exception.
        /// </summary>
        /// <param name="message">the error message</param>
        /// <param name="inner">the original exception</param>
        public UnsupportedRequestRestApiContractException(string message, Exception inner)
            : base(message, inner)
        {
        }

        // Without this constructor, deserialization will fail
        protected UnsupportedRequestRestApiContractException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
// end changes by MP on 07/01/2019