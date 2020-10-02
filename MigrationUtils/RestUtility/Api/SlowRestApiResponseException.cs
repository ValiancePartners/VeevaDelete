// MP, 10/23/2019, Mantis 0001835, Add optional timeout length setting
//-----------------------------------------------------------------------
// <copyright file="SlowRestApiReponseException.cs" company="Valiance Partners">
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
    /// High-Level API failure exception class for when the APi has not responded in time
    /// Indicates (correctly formed) API request which did not complete
    /// </summary>
    [Serializable]
    public class SlowRestApiResponseException : RestApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SlowRestApiResponseException" /> class.
        /// default constructor
        /// </summary>
        public SlowRestApiResponseException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SlowRestApiResponseException" /> class.
        /// standard constructor with error message
        /// </summary>
        /// <param name="message">the error message</param>
        public SlowRestApiResponseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SlowRestApiResponseException" /> class.
        /// standard constructor with error message and original exception.
        /// </summary>
        /// <param name="message">the error message</param>
        /// <param name="inner">the original exception</param>
        public SlowRestApiResponseException(string message, Exception inner)
            : base(message, inner)
        {
        }

        // Without this constructor, deserialization will fail
        protected SlowRestApiResponseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

    }
}
// end changes by MP on 10/23/2019