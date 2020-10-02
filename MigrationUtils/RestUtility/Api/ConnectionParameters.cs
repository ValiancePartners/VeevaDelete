// MP, 07/01/2019, Mantis 0001782, Vault to Vault binder links, merge into MigrationUtils repository
//-----------------------------------------------------------------------
// <copyright file="ConnectionParameters.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
// <summary>
// This file contains the ConnectionParameters class.
// </summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestUtility.Api
{
    /// <summary>
    /// The parameters needed to open a web session
    /// </summary>
    public class ConnectionParameters
    {
        /// <summary>
        /// Gets or sets the base service URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the user account for accessing the service
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password for the user account
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a sub-service specification
        /// </summary>
        public string SubService { get; set; }

        // MP, 09/16/2019, Mantis 0001817, Support Vault API Client ID, add clientId connection parameter
        /// <summary>
        /// Get or set the Client ID to be included on all requests
        /// </summary>
        public string ClientId { get; set; }
        // end changes by MP on 09/16/2019
    }
}
// end changes by MP on 07/01/2019