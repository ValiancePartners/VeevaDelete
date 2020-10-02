// MP, 07/01/2019, Mantis 0001782, Vault Binder to Binder Links, merge into MigrationUtils repository
//-----------------------------------------------------------------------
// <copyright file="IRestConnection.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
// <summary>
// This file contains IRestConnection interface.
// </summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;

namespace RestUtility.Api
{
    /// <summary>
    /// The IRestConnection interface
    /// </summary>    
    public interface IRestConnection
    {
        /// <summary>
        /// Gets the maximum size for batch deletes. &lt; 0 means no limit, zero means not supported
        /// </summary>
        int MaxBatchSize { get; }

        // MP, 10/23/2019, Mantis 0001835, Add optional timeout length setting
        /// <summary>
        /// Gets or sets the an optional maximum number of milliseconds to wait for an API response before timeing out. To disable, use System.Threading.Infinite (-1)
        /// </summary>
        int TimeoutLength { get; set; }

        /// <summary>
        /// Gets or sets the an optional maximum number of retries to perform on an API request if it times out. To disable, use System.Threading.Infinite (-1)
        /// </summary>
        int TimeoutRetries { get; set; }
        // end changes by MP on 10/23/2019

        /// <summary>
        /// Gets the api version
        /// </summary>
        decimal ApiVersion { get; }

        /// <summary>
        /// The current session id
        /// </summary>
        string SessionId { get; set; }

        /// <summary>
        /// Execute the specified web request method and universal resource identifier giving the specified content
        /// </summary>
        /// <param name="responseProcessors">a collection of resposne processors per response type</param>
        /// <param name="method">the request method to use</param>
        /// <param name="url">the absolute or relative uniform resource locator to use</param>
        /// <param name="content">the content to include</param>
        /// <returns>the response to the request</returns>
        T ExecuteApi<T>(Dictionary<string, Func<Stream,T>> responseProcessors,string method, string url, string content=null);

        // MP, 09/16/2019, Mantis 0001817, Support Vault API Client ID, add clientId connection parameter
        /// <summary>
        /// Register a new connection
        /// </summary>
        /// <param name="apiVersion">the API veersion</param>
        /// <param name="baseUrl">the absolute URL to combine with future relative URL</param>
        /// <param name="username">the user name for the connection</param>
        /// <param name="password">the password for the connection</param>
        /// <param name="clientId">the client Id to be included in every REST request</param>
        void SetupConnection(decimal apiVersion, Uri baseUrl, string username, string password, string clientId);
        //void SetupConnection(decimal apiVersion, Uri baseUrl, string username, string password);
        // end changes by MP on 09/16/2019

        /// <summary>
        /// Get login credentials
        /// </summary>
        /// <param name="username">the login user name </param>
        /// <param name="password">the login password</param>
        void GetCredentials(out string username, out string password);

    }
}
// end changes by MP on 07/01/2019