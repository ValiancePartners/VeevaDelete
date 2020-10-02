// MP, 07/01/2019, Mantis 0001782, Vault Binder to Binder Links, merge into MigrationUtils repository
//-----------------------------------------------------------------------
// <copyright file="DeniedRestApiRequestException.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
// MP, 10/31/2019, Mantis 0001843, Tidy up Vault API query exception handling, report as VaultApiException
using RestUtility.Api;

namespace RestUtility.VeevaVaultXml
//namespace RestUtility.Api
// end changes by MP on 10/31/2019
{
    /// <summary>
    /// High-Level API failure exception class for when response from Vault API indicates failure
    /// Indicates (correctly formed) Vault API request which failed
    /// </summary>
    [Serializable]
    // MP, 10/31/2019, Mantis 0001843, Tidy up Vault API query exception handling, report as VaultApiException
    public class VaultApiException : RestApiException
    //public class DeniedRestApiRequestException : RestApiException
    // end changes by MP on 10/31/2019
    {
        /// <summary>
        /// Gets the vault API error codes and messages
        /// </summary>
        // MP, 10/31/2019, Mantis 0001843, Tidy up Vault API query exception handling, report as VaultApiException
        public readonly IList<VaultXmlHelper.VaultError> VaultErrors;
        // end changes by MP on 10/31/2019

        /// <summary>
        /// Initializes a new instance of the <see cref="VaultApiException" /> class.
        /// default constructor
        /// </summary>
        // MP, 10/31/2019, Mantis 0001843, Tidy up Vault API query exception handling, report as VaultApiException
        public VaultApiException()
        //public DeniedRestApiRequestException()
        // end changes by MP on 10/31/2019
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VaultApiException" /> class.
        /// standard constructor with error message
        /// </summary>
        /// <param name="message">the error message</param>
        // MP, 10/31/2019, Mantis 0001843, Tidy up Vault API query exception handling, report as VaultApiException
        public VaultApiException(string message)
        //public DeniedRestApiRequestException(string message)
        // end changes by MP on 10/31/2019
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VaultApiException" /> class.
        /// standard constructor with error message and original exception.
        /// </summary>
        /// <param name="message">the error message</param>
        /// <param name="inner">the original exception</param>
        // MP, 10/31/2019, Mantis 0001843, Tidy up Vault API query exception handling, report as VaultApiException
        public VaultApiException(string message, Exception inner)
        //public DeniedRestApiRequestException(string message, Exception inner)
        // end changes by MP on 10/31/2019
            : base(message, inner)
        {
        }

        // Without this constructor, deserialization will fail
        // MP, 10/31/2019, Mantis 0001843, Tidy up Vault API query exception handling, report as VaultApiException
        protected VaultApiException(SerializationInfo info, StreamingContext context)
        //protected DeniedRestApiRequestException(SerializationInfo info, StreamingContext context)
        // end changes by MP on 10/31/2019
            : base(info, context)
        {
            // MP, 10/31/2019, Mantis 0001843, Tidy up Vault API query exception handling, report as VaultApiException
            if (info != null)
            {
                SerializationHelper.GetValue(info, nameof(this.VaultErrors), out this.VaultErrors);
            }
            // end changes by MP on 10/31/2019
        }

        // MP, 10/31/2019, Mantis 0001843, Tidy up Vault API query exception handling, report as VaultApiException
        public VaultApiException(VaultXmlHelper.VaultStatus status)
            : base(status.ToString())
        {
            this.VaultErrors = status.Errors;
        }

        public override void GetObjectData(SerializationInfo info,
           StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info != null)
            {
               SerializationHelper.AddValue(info, nameof(this.VaultErrors), this.VaultErrors);
            }
        }
        // end changes by MP on 10/31/2019

    }
}
// end changes by MP on 07/01/2019