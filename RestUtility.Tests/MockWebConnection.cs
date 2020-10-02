//-----------------------------------------------------------------------
// <copyright file="MockWebConnection.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using RestUtility.Api;
using RestUtility.VeevaVaultXml;

namespace RestUtility.Tests
{
    /// <summary>
    /// A mock web connection implementation for testing
    /// </summary>
    public sealed class MockWebConnection : IRestConnection
    {
        /// <summary>
        /// placeholder parameters for testing
        /// </summary>
        public static readonly ConnectionParameters MockParameters =
         new ConnectionParameters
         {
             Url = "https://site",
             SubService = "v1",
             Username = "USER",
             Password = "PASSWORD",
             ClientId = "Test company-Test org-Test product-Client-Test tool",
         };

        /// <summary>
        /// uri corresponding to placeholder parameters
        /// </summary>
        public static readonly string MockLoginUri =
            $"auth?username={MockParameters.Username}&password={MockParameters.Password}";

        /// <summary>
        /// placeholder failure message
        /// </summary>
        public static readonly string MockFailMessage = "MESSAGE";

        /// <summary>
        /// placeholder failure type
        /// </summary>
        public static readonly string MockFailType = "TYPE";

        /// <summary>
        /// placeholder busy message
        /// </summary>
        public static readonly string MockBusyMessage = "API limit exceeded";

        /// <summary>
        /// placeholder busy type
        /// </summary>
        public static readonly string MockBusyType = VaultXmlHelper.VaultStatus.API_LIMIT_EXCEEDED;

        /// <summary>
        /// response corresponding to failure message/type
        /// </summary>
        public static readonly string MockFailResponse =
            "<x><responseStatus/><errors><error><message>" +
            MockFailMessage + "</message>" + "<type>" +
            MockFailType + "</type></error></errors></x>";

        /// <summary>
        /// response corresponding to failure message/type
        /// </summary>
        public static readonly string MockBusyResponse =
            "<x><responseStatus/><errors><error><message>" +
            MockBusyMessage + "</message>" + "<type>" +
            MockBusyType + "</type></error></errors></x>";

        /// <summary>
        /// basic success response
        /// </summary>
        public static readonly string MockSuccessReponse = "<x><responseStatus>SUCCESS</responseStatus></x>";

        /// <summary>
        /// placeholder/dummy query
        /// </summary>
        public static readonly string MockQuery = "select id from documents where binder__v = false";

        /// <summary>
        /// urn corresponding to placeholder query
        /// </summary>
        public static readonly string MockQueryUrn = "query?q=" + MockQuery;

        /// <summary>
        /// API request for placeholder data query
        /// </summary>
        public static readonly string MockQueryDataRequest = "GET " + MockQueryUrn;

        /// <summary>
        /// API request for placeholder count query
        /// </summary>
        public static readonly string MockQueryCountRequest = MockQueryDataRequest + " limit 1";

        /// <summary>
        /// the number of items returned by the default mock response
        /// </summary>
        public static readonly int MockCount = 3;

        /// <summary>
        /// the page size of the default mock response
        /// </summary>
        public static readonly int MockLimit = 1000;

        /// <summary>
        /// the default mock response
        /// </summary>
        public static readonly string MockQueryResponse = "<x><responseStatus>SUCCESS</responseStatus>" +
            "<responseDetails><total>" + MockCount.ToString() + "</total></responseDetails><data>" +
            string.Join(string.Empty, Enumerable.Range(1, MockCount).Select(rowno => "<row><id>" + rowno.ToString() + "</id></row>")) +
            "</data></x>";

        /// <summary>
        /// the urn for batch deletion
        /// </summary>
        public static readonly string MockDeleteBatch = "DELETE objects/documents/batch";

        /// <summary>
        /// a dialog tracing the deletion of items returned by the standard mock response
        /// </summary>
        public static readonly Transfer[] MockQueryDeleteTransfers =
            Enumerable.Concat(
                new Transfer[] { new Transfer { Request = MockQueryDataRequest } },
                Enumerable.Range(1, MockCount).Select(rowno =>
                    new Transfer
                    {
                        Request = "DELETE objects/documents/" + rowno.ToString()
                    })).ToArray();

        /// <summary>
        /// Gets or sets the sequence of expected request/response exchanges
        /// </summary>
        private readonly IEnumerator<Transfer> transfers;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockWebConnection" /> class.
        /// Default constructor for use in connection setup, prepares login and initial query dialog
        /// </summary>
        public MockWebConnection() : this(
            new Transfer[]
            {
                new Transfer
                {
                    Request = "POST " + MockWebConnection.MockLoginUri,
                    Response  = "<x><responseStatus>SUCCESS</responseStatus><sessionId>TESTID</sessionId></x>"
                },
                new Transfer
                {
                    Request = MockQueryCountRequest
                }
            })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MockWebConnection" /> class with one exchange.
        /// </summary>
        /// <param name="transfer">one dialog request/response exchange</param>
        public MockWebConnection(Transfer transfer) : this(new Transfer[] { transfer })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MockWebConnection" /> class with a set of exchanges
        /// </summary>
        /// <param name="transfers">enumerable of exchanges with expected query/responses</param>
        public MockWebConnection(IEnumerable<Transfer> transfers)
        {
            if (!(transfers != null))
            {
                throw new ArgumentNullException(nameof(transfers));
            }

            Contract.EndContractBlock();

            this.transfers = transfers.GetEnumerator();
        }

        /// <summary>
        /// Gets or sets the default base Url to prepend to any query when the connection is open
        /// </summary>
        public Uri DefaultUrl { get; set; }

        /// <summary>
        /// Gets or sets the session id of an open connection
        /// </summary>
        public string SessionId { get; set; }

        public string ClientId { get; private set; }

        // private bool xmlError = false;
        // private string xmlApiError = string.Empty;

        private decimal apiVersion;

        private string username;

        private string password;

        /// <summary>
        /// Gets the maximum size for batch deletes
        /// </summary>
        public int MaxBatchSize { get; }  = 3; 

        /// <summary>
        /// Gets the maximum size for batch deletes
        /// </summary>
        public decimal ApiVersion => this.apiVersion;

        // MP, 10/23/2019, Mantis 0001835, Add timeout handling
        /// <summary>
        /// Gets or sets the response timeout length
        /// </summary>
        public int TimeoutLength { get; set; } = -1;

        /// <summary>
        /// Gets or sets the number of times to retry a request if the response times out
        /// </summary>
        public int TimeoutRetries { get; set; } = 2;
        // end changes by MP on 10/23/2019

        /// <summary>
        /// Register a new connection
        /// </summary>
        /// <param name=apiVersion">the API Version for the connection</param>
        /// <param name="baseUrl">the base Url for the connection</param>
        /// <param name="username">the user name for the connection</param>
        /// <param name="password">the user name for the connection</param>
        /// <param name="clientId">the client id for the connection</param>
        public void SetupConnection(decimal apiVersion, Uri baseUrl, string username, string password, string clientId)
        {
            this.apiVersion = apiVersion;
            this.DefaultUrl = baseUrl;
            this.username = username;
            this.password = password;
            this.ClientId = clientId;
        }

        public void GetCredentials(out string username, out string password)
        {
            username = this.username;
            password = this.password;
        }

        /// <summary>
        /// Accept instruction and simulate result
        /// </summary>
        /// <param name="method">HTTP Method</param>
        /// <param name="vaultUri">Url or Urn reference</param>
        /// <param name="content">Content to sent</param>
        /// <returns>Result of HTTP method</returns>
        public T ExecuteApi<T>(Dictionary<string, Func<Stream, T>> responseProcessors, string method, string vaultUri, string content = null)
        {
            this.transfers.MoveNext();
            string expectedCall = this.transfers.Current.Request;
            if (expectedCall != null)
            {
                int splitterIndex = expectedCall.IndexOf(" ");
                Contract.Assume(splitterIndex >= 0);
                string expectedUri = expectedCall.Substring(splitterIndex + 1);
                string expectedMethod = expectedCall.Substring(0, splitterIndex);
                if (expectedMethod != method)
                {
                    throw new Exception(string.Format(
                        "Testing Exception Incorrect API request.\nExpected: {0} Received: {1}",
                        expectedMethod,
                        method));
                }

                if (expectedUri != vaultUri)
                {
                    throw new Exception(string.Format(
                        "Testing Exception Incorrect URI.\nExpected: \"{0}\" Received: \"{1}\"",
                        expectedUri,
                        vaultUri));
                }
            }

            string expectedContent = this.transfers.Current.Content;
            if (expectedContent != null)
            {
                if (expectedContent != content)
                {
                    throw new Exception(string.Format(
                        "Testing Exception Incorrect Content.\nExpected: \"{0}\" Received: \"{1}\"",
                        expectedContent,
                        content));
                }
            }

            string responseStr = this.transfers.Current.Response;
            if (responseStr == null)
            {
                responseStr= MockQueryResponse;
            }

            using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(responseStr)))
            {
                Func<Stream, T> responseProcessor = responseProcessors["application/xml"];
                T returnValue = responseProcessor(stream);
                return returnValue;
            }
        }

        /// <summary>
        /// expected exchange sequence containing request and corresponding response and content
        /// </summary>
        public class Transfer
        {
            /// <summary>
            /// Gets or sets the expected request
            /// null to match any request
            /// </summary>
            public string Request { get; set; }

            /// <summary>
            /// Gets or sets the response to give to the request
            /// a default successful query response will be provided if this is null
            /// </summary>
            public string Response { get; set; }

            /// <summary>
            /// Gets or sets the corresponding response content, if any
            /// </summary>
            public string Content { get; set; }
        }
    }
}
