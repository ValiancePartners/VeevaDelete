// MP, 07/01/2019, Mantis 0001782, Vault Binder to Binder Links, merge into MigrationUtils repository
//-----------------------------------------------------------------------
// <copyright file="VaultXmlHelper.cs" company="Valiance Partners">
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
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using RestUtility.Api;

using XElement = System.Xml.XPath.XPathNavigator;

namespace RestUtility.VeevaVaultXml
{
    /// <summary>
    /// Utility to provide a Vault deletion API on top of a basic connector that accepts request strings
    /// follows the vault API request protocols, returns and breaks down XML responses
    /// </summary>
    public static class VaultXmlHelper
    {
        /// <summary>
        /// we want to allow non-conforming xml names for some vault atttributes
        /// </summary>
        private static readonly XmlReaderSettings vaultXmlReaderSettings = new XmlReaderSettings { CheckCharacters = false };

        /// <summary>
        /// we want to send only spaces, and no other form of white space to vault?
        /// </summary>
        private static readonly Regex whitespaceDetector = new Regex(@"[\r\n]+");

        private static readonly Dictionary<string, Func<Stream, XElement>> vaultXmlResponseProcessor =
            new Dictionary<string, Func<Stream, XElement>> { { "application/xml", VaultXmlHelper.GetResponseXml } };

        ///// <summary>
        ///// return the value of an XML element if it is non-null or null otherwise
        ///// </summary>
        ///// <param name="node">the XML element to get a value from</param>
        ///// <returns>the value of the node as a string or null</returns>
        //public static string GetValueOfOptionalXml(XElement node)
        //{
        //    return node?.Value;
        //}

        public static VaultResponse ExecuteXmlApi(this IRestConnection connection, string method, string url, string content = null)
        {
            //var responseXml = connection.ExecuteApi(vaultXmlResponseProcessor, method, url, content);
            // MP, 10/31/2019, Mantis 0001843, Tidy up Vault API query exception handling, report Vault API Exceptions as VaultAPIException
            connection.TryExecuteXmlApi(out VaultResponse response, method, url, content);
            ThrowUnlessSuccess(response);
            //if (!connection.TryExecuteXmlApi(out VaultResponse response, method, url, content))
            //{
            //    throw new DeniedRestApiRequestException(response.ToString());
            //}
            // end changes by MP on 10/31/2019
            return response;
        }

        public static bool TryExecuteXmlApi(this IRestConnection connection, out VaultResponse response, string method, string url, string content = null)
        {
            //var responseXml = connection.ExecuteApi(vaultXmlResponseProcessor, method, url, content);
            for (; ; )
            {
                XElement responseXml = connection.ExecuteApi(vaultXmlResponseProcessor, method, url, content);
                response = new VaultResponse(responseXml);
                if (response.Success || !response.InvalidSession)
                {
                    break;
                }
                connection.NewSession();
            };
            return response.Success;
        }

        /// <summary>
        /// Collect the next page url from a vault query response
        /// </summary>
        /// <param name="responseXml">the vault query response containing the item ids</param>
        /// <returns>the url of the next page after this provided response page</returns>
        public static string GetNextPageFromResponse(XElement response, string responseTag)
        {
            if (!(response != null))
            {
                throw new ArgumentNullException(nameof(response));
            }

            Contract.EndContractBlock();

            return response.ApiResponseElement(responseTag).Element("next_page")?.Value;
        }

        /// <summary>
        /// Collect the items for a vault request from a vault connection
        /// </summary>
        /// <param name="connection">the connection to talk to</param>
        /// <param name="query">the vault query</param>
        public static IEnumerable<List<string[]>> GetQueryPages(this IRestConnection connection, string query)
        {
            if (!(connection != null))
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (!(query != null))
            {
                throw new ArgumentNullException(nameof(query));
            }
            Contract.EndContractBlock();
            string firstUrl = VaultXmlHelper.GetQueryUrn(query);
            VaultResponse response = connection.ExecuteXmlApi("GET", firstUrl);
            int remainingRows = GetItemCountFromResponse(response.responseXml);
            string[] fieldNames = null;
            VaultTableParser tableParser = new VaultTableParser(VaultSymbols.ResponseDataRow);
            List<string[]> items = tableParser.CollectItemsFromResponse(ref fieldNames, response);
            yield return new List<string[]> { fieldNames };
            while (remainingRows > 0)
            {
                remainingRows -= items.Count();
                if (remainingRows > 0)
                {
                    string nextPageUrl = GetNextPageFromResponse(response.responseXml, VaultSymbols.ResponseDetails);
                    response = connection.ExecuteXmlApi("GET", nextPageUrl);
                }
                yield return items;
                if (remainingRows > 0)
                {
                    items = tableParser.CollectItemsFromResponse(ref fieldNames, response);
                }
            }
            Contract.Assert(remainingRows == 0, "more rows received than expected");
            yield break;
        }

        /// <summary>
        /// Get the UI names (singular and plural) for the object
        /// </summary>
        /// <param name="connection">the connection to talk to</param>
        /// <param name="objectName">the name__v value identifying the object type</param>
        /// <returns>The UI names for the requested API object name</returns>
        public static Tuple<string, string> GetObjectUINames(this IRestConnection connection, string objectName)
        {
            if (!(connection != null))
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (!(objectName != null))
            {
                throw new ArgumentNullException(nameof(objectName));
            }

            Contract.EndContractBlock();

            string objectUrn = "metadata/vobjects/" + objectName;

            var collectUIResponse = new Dictionary<string, Func<Stream, Tuple<string, string>>> {
                 { "application/xml", (Stream responseStream) => {
               XElement responseXml = GetResponseXml(responseStream);
               XElement objectMetaData = responseXml.ApiResponseElement(VaultSymbols.ResponseObject);
               string label = objectMetaData.ApiResponseElement(VaultSymbols.ResponseLabel).Value;
               string pluralLabel = objectMetaData.ApiResponseElement(VaultSymbols.ResponseLabelPlural).Value;
               return new Tuple<string, string>(label, pluralLabel);
            }  } };
            var returnValue = connection.ExecuteApi(collectUIResponse, "GET", objectUrn);
            return returnValue;

        }

        /// <summary>
        /// Convert response string to XML if any
        /// </summary>
        /// <param name="responseStr">the API response</param>
        /// <returns>the response as an XML object</returns>
        public static XElement GetResponseXml(Stream responseStream)
        {
            Contract.Ensures(Contract.Result<XElement>() != null);
            if (!(responseStream != null))
            {
                throw new ArgumentNullException(nameof(responseStream));
            }

            XmlReader xmlResponseReader = XmlReader.Create(responseStream, vaultXmlReaderSettings);
            try
            {
                XPathDocument document = new XPathDocument(xmlResponseReader);
                XElement rootElement = document.RootElement();
                return rootElement;
            }
            catch (Exception ex)
            {
                throw new UnexpectedResponseRestApiException("Cannot parse vault xml response", ex);
            }
        }

        /*
        /// <summary>
        /// Get response details node for a request that was accepted or throw exception for a request that was denied.
        /// </summary>
        /// <param name="responseStr">The API response strng</param>
        /// <returns>Details of a succesful response as an XML node</returns>
        //public static XElement GetResponseDetails(string responseStr)
        //{
        //    Contract.Ensures(Contract.Result<XElement>() != null);
        //    XElement responseXml = GetPassResponseXml(responseStr);
        //    XElement returnValue = responseXml.Root.ApiResponseElement("responseDetails");
        //    return returnValue;
        //}
         **/

        /// <summary>
        /// get an element from an XML API response
        /// </summary>
        /// <param name="node">the API response XML node</param>
        /// <param name="elementName">the name of the required element</param>
        /// <returns>the requested element
        /// throws <seealso cref="UnexpectedResponseRestApiException"/>if the element is not found under in the XML node
        /// </returns>
        public static XElement ApiResponseElement(this XElement node, string elementName)
        {
            if (!(node != null))
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (!(elementName != null))
            {
                throw new ArgumentNullException(nameof(elementName));
            }

            Contract.EndContractBlock();
            XElement returnValue = node.Element(elementName);
            if (returnValue == null)
            {
                throw new UnexpectedResponseRestApiException("Expected response element not found: " + elementName);
            }
            return returnValue;
        }

        /// <summary>
        /// get a set of elements from an XML API response
        /// </summary>
        /// <param name="parent">the node to collect the child element from</param>
        /// <param name="elementName">the name of the element to collect</param>
        /// <returns>the requested element
        /// throws <seealso cref="UnexpectedResponseRestApiException"/>if the element is not found under in the XML node
        /// </returns>
        public static XPathNodeIterator ApiResponseElements(this XElement parent, string elementName)
        {
            if (!(parent != null))
            {
                throw new ArgumentNullException(nameof(parent));
            }

            if (!(elementName != null))
            {
                throw new ArgumentNullException(nameof(elementName));
            }

            Contract.EndContractBlock();

            var nodes = parent.Elements(elementName);
            if (nodes == null)
            {
                throw new UnexpectedResponseRestApiException("Expected response elements not found: " + elementName);
            }

            return nodes;
        }

        /// <summary>
        public static XPathNavigator RootElement(this XPathDocument document)
        {
            XPathNavigator navigator = document.CreateNavigator();
            navigator.MoveToFirstChild();
            return navigator;
        }

        /// <summary>
        /// get an element from an XML API response
        /// </summary>
        /// <param name="node">the API response XML node</param>
        /// <param name="elementName">the name of the required element</param>
        /// <returns>the requested element
        /// throws <seealso cref="UnexpectedResponseRestApiException"/>if the element is not found under in the XML node
        /// </returns>
        public static XPathNavigator Element(this XPathNavigator node, string elementName)
        {
            if (!(node != null))
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (!(elementName != null))
            {
                throw new ArgumentNullException(nameof(elementName));
            }

            Contract.EndContractBlock();


            XPathNodeIterator elements = node.SelectChildren(elementName, "");
            XPathNavigator returnValue;
            if (!elements.MoveNext())
            {
                returnValue = null;
            }
            else
            {
                returnValue = elements.Current;
            }
            return returnValue;
        }

        /// <summary>
        /// get an element from an XML API response
        /// </summary>
        /// <param name="node">the API response XML node</param>
        /// <returns>the requested element
        /// throws <seealso cref="UnexpectedResponseRestApiException"/>if the element is not found under in the XML node
        /// </returns>
        public static XPathNodeIterator Elements(this XPathNavigator node)
        {
            if (!(node != null))
            {
                throw new ArgumentNullException(nameof(node));
            }

            Contract.EndContractBlock();

            XPathNodeIterator returnValue = node.SelectChildren("", "");
            return returnValue;
        }

        /// <summary>
        /// get an element from an XML API response
        /// </summary>
        /// <param name="node">the API response XML node</param>
        /// <param name="elementName">the name of the required element</param>
        /// <returns>the requested element
        /// throws <seealso cref="UnexpectedResponseRestApiException"/>if the element is not found under in the XML node
        /// </returns>
        public static XPathNodeIterator Elements(this XPathNavigator node, string elementName)
        {
            if (!(node != null))
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (!(elementName != null))
            {
                throw new ArgumentNullException(nameof(elementName));
            }

            Contract.EndContractBlock();

            XPathNodeIterator returnValue = node.SelectChildren(elementName, "");
            return returnValue;
        }

        public static XPathNodeIterator XPathSelectElements(this XPathNavigator node, string path)
        {
            if (!(node != null))
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (!(path != null))
            {
                throw new ArgumentNullException(nameof(path));
            }

            Contract.EndContractBlock();

            XPathNodeIterator returnValue = node.Select(path);
            return returnValue;
        }



        /// Get the local name of this element
        /// </summary>
        /// <param name="node">the element to get the local name of</param>
        /// <returns>the local name of the element</returns>
        public static string GetLocalName(this XPathNavigator node)
        {
            return node.LocalName;
        }

        /// <summary>
        /// Open a vault connection
        /// </summary>
        /// <param name="connection">the connection to talk to</param>
        /// <param name="connectionParameters">the connection details</param>
        public static void OpenConnection(this IRestConnection connection, ConnectionParameters connectionParameters)
        {
            if (!(connection != null))
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (!(connectionParameters != null))
            {
                throw new ArgumentNullException(nameof(connectionParameters));
            }

            if (!(connectionParameters.Url != null))
            {
                throw new ArgumentNullException(nameof(connectionParameters.Url));
            }

            if (!(connectionParameters.SubService != null))
            {
                // MP, 31/10/2019, Mantis 0001843, Tidy up Vault API query exception handling, report API Version exceptions with the text "API Version"
                throw new ArgumentNullException("API Version");
                //throw new ArgumentNullException(nameof(connectionParameters.SubService));
                // end changes by MP on 31/10/2019
            }

            if (!(connectionParameters.Username != null))
            {
                throw new ArgumentNullException(nameof(connectionParameters.Username));
            }

            if (!(connectionParameters.Password != null))
            {
                throw new ArgumentNullException(nameof(connectionParameters.Password));
            }

            if (!(Regex.IsMatch(connectionParameters.SubService, @"^v\d+(.\d+)?$")))
            {
                // MP, 31/10/2019, Mantis 0001843, Tidy up Vault API query exception handling, report API Version exceptions with the text "API Version"
                throw new ArgumentOutOfRangeException("API Version");
                //throw new ArgumentOutOfRangeException(nameof(connectionParameters.SubService));
                // end changes by MP on 31/10/2019
            }

            // MP, 09/16/2019, Mantis 0001817, Support Vault API Client ID, add clientId connection parameter
            if (!(connectionParameters.ClientId != null))
            {
                throw new ArgumentNullException(nameof(connectionParameters.ClientId));
            }
            // end changes by MP on 09/16/2019

            Contract.EndContractBlock();

            decimal apiVersion = decimal.Parse(connectionParameters.SubService.Remove(0, 1));
            // Construct Default Url to use with Queries
            StringBuilder urlBuilder = new StringBuilder();
            urlBuilder.AppendFormat("{0}/api/{1}/", connectionParameters.Url, connectionParameters.SubService);

            string defaultUrl = urlBuilder.ToString();

            if (!Uri.TryCreate(defaultUrl, UriKind.Absolute, out Uri validatedUri))
            {
                throw new UnsupportedRequestRestApiContractException(
                    string.Format("Incorrect URL format for Vault Connection: {0}", defaultUrl));
            }

            // if the URI is valid, the scheme might not be http or https
            if (validatedUri.Scheme != Uri.UriSchemeHttp && validatedUri.Scheme != Uri.UriSchemeHttps)
            {
                // if the scheme is invalid it must be the open session request
                throw new UnsupportedRequestRestApiContractException(
                    string.Format("HTTP(S) URI Scheme not used in Vault URL: {0}", defaultUrl));
            }

            // MP, 09/16/2019, Mantis 0001817, Support Vault API Client ID, add clientId connection parameter (lower required)
            connection.SetupConnection(apiVersion, validatedUri, connectionParameters.Username, connectionParameters.Password, connectionParameters.ClientId.ToLower());
            //connection.SetupConnection(apiVersion, validatedUri, connectionParameters.Username, connectionParameters.Password);
            // end changes by MP on 09/16/2019


            connection.NewSession();
        }

        private static void NewSession(this IRestConnection connection)
        {
            connection.GetCredentials(out string username, out string password);
            string connectionUrl = string.Format(
                "auth?username={0}&password={1}",
                Uri.EscapeDataString(username),
                Uri.EscapeDataString(password));

            var responseXml = connection.ExecuteApi(vaultXmlResponseProcessor, "POST", connectionUrl);
            var status = new VaultStatus(responseXml);
            // MP, 10/31/2019, Mantis 0001843, Tidy up Vault API query exception handling, report Vault API Exceptions as VaultAPIException
            ThrowUnlessSuccess(status);
            //if (!status.Success)
            //{
            //    throw new DeniedRestApiSessionException(status.ToString());
            //}
            // end changes by MP on 10/31/2019

            // sessionId = responseXml.Descendants("VersionedAuthRestResult").Select(
            //   s => s.ApiResponseElement("sessionId").Value).FirstOrDefault()            
            connection.SessionId = responseXml.ApiResponseElement("sessionId").Value;
        }

        /// <summary>
        /// give the number of items returned by the vault query
        /// </summary>
        /// <param name="connection">the connection to talk to</param>
        /// <param name="vql">the query to try</param>
        /// <returns>the number of items returned</returns>
        public static int GetCount(this IRestConnection connection, string vql)
        {
            if (!(connection != null))
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (!(vql != null))
            {
                throw new ArgumentNullException(nameof(vql));
            }

            Contract.EndContractBlock();

            // Generate Url to return one item.
            bool limited = vql.ToLower().Contains(" limit ");
            string url = GetQueryUrn(vql) + (limited ? string.Empty : " limit 1");

            // Api call.
            VaultResponse response = connection.ExecuteXmlApi("GET", url);
            int returnValue = GetItemCountFromResponse(response.responseXml);
            return returnValue;
        }

        /// <summary>
        /// Translate a vault query response into a collection of Item References
        /// </summary>
        /// <param name="responseXml">the vault query response containing the item ids</param>
        /// <returns>the number of items</returns>
        public static int GetItemCountFromResponse(XElement responseXml)
        {
            if (!(responseXml != null))
            {
                throw new ArgumentNullException(nameof(responseXml));
            }

            Contract.EndContractBlock();

            string recordCountResponse = responseXml.
                ApiResponseElement(VaultSymbols.ResponseDetails).
                ApiResponseElement(VaultSymbols.ResponseTotal).Value;
            if (int.TryParse(recordCountResponse, out int recordCount))
            {
                return recordCount;
            }

            throw new UnexpectedResponseRestApiException(
                "Total response size is not an integer: " + recordCountResponse);
        }

        /// <summary>
        /// calculate the urn required to send a query to vault
        /// </summary>
        /// <param name="selectQuery">the query to send</param>
        /// <returns>the request to send to vault</returns>
        public static string GetQueryUrn(string selectQuery)
        {
            if (string.IsNullOrEmpty(selectQuery))
            {
                throw new UnsupportedRequestRestApiContractException("No Query Provided");
            }

            string returnValue = "query?q=" + whitespaceDetector.Replace(selectQuery, whitespace => " ");
            return returnValue;
        }

        /// <summary>
        /// Get status of XML response
        /// </summary>
        /// <param name="responseXml">the XML response from Vault</param>
        /// <returns>The Vault status details</returns>
        public static VaultStatus GetVaultStatus(XElement responseXml)
        {
            if (!(responseXml != null))
            {
                throw new ArgumentNullException(nameof(responseXml));
            }

            Contract.EndContractBlock();

            VaultStatus returnValue = new VaultStatus(responseXml);
            return returnValue;
        }


        /// <summary>
        /// Check if the response indicates success
        /// </summary>
        /// <param name="responseXml">The response to check</param>
        /// <returns>true if the response indicates success</returns>
        private static bool GetXmlStatus(XElement responseXml)
        {
            if (responseXml == null)
            {
                return false;
            }

            bool status = false;

            // var result = responseXml.Element("VersionedAuthRestResult").Descendants();
            // string responseStatus = responseXml.Descendants("responseStatus").FirstOrDefault().Value;
            string responseStatus = responseXml.ApiResponseElement("responseStatus").Value;
            ////try
            ////{
            if (responseStatus.ToUpper().Equals("SUCCESS"))
            {
                // xmlApiError = string.Empty;
                status = true;
            }
            else
            {
                // xmlApiError = GetXmlError(responseMessage);
                status = false;
            }
            ////}
            ////catch (Exception e)
            ////{
            ////throw e;
            ////}

            return status;
        }

        /// <summary>
        /// Throw an exception unless the response failed because of an invalid session id
        /// </summary>
        /// <param name="status">The vault failure status</param>
        public static void ThrowUnlessSuccess(VaultStatus status)
        {
            if (!(status != null))
            {
                throw new ArgumentNullException(nameof(status));
            }
            Contract.EndContractBlock();

            if (status.Success)
            {
                return;
            }
            // MP, 10/31/2019, Mantis 0001843, Tidy up Vault API query exception handling, report Vault API Exceptions as VaultAPIException
            var ex = new VaultApiException(status);
            throw ex;
            //throw new DeniedRestApiRequestException(status.ToString());
            // end changes by MP on 10/31/2019
        }


        /// <summary>
        /// Extract the error from a response indicating failure
        /// </summary>
        /// <param name="responseXml">The response</param>
        /// <returns>The error indicated by the response</returns>
        private static string GetXmlError(XElement responseXml)
        {
            if (!(responseXml != null))
            {
                throw new ArgumentNullException(nameof(responseXml));
            }

            Contract.EndContractBlock();

            ////XElement errors = responseXml.Root.ApiResponseElements("errors").FirstOrDefault();
            XElement errors = responseXml.ApiResponseElement("errors");
            XElement error = errors.ApiResponseElement("error");
            // errorType and errors/error/message both optional depending on API version and error
            string returnValue = "";
            XElement errorTypes = error.ApiResponseElement("errorType");
            if (errorTypes != null)
            {
                returnValue += errorTypes.Value + " Error\r\n";
            }
            returnValue += error.ApiResponseElement("type").Value;
            XElement message = error.Element("message");
            if (message != null) returnValue += ": " + message.Value;
            return returnValue;
        }

        /// <summary>
        /// Vault response errors class
        /// </summary>
        public class VaultError
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="VaultError"/> class,
            /// </summary>
            /// <param name="error">XML node containing the error details</param>
            public VaultError(XElement error)
            {
                if (!(error != null))
                {
                    throw new ArgumentNullException(nameof(error));
                }

                Contract.EndContractBlock();

                this.ErrorCode = error.ApiResponseElement("type").Value;
                this.ErrorMessage = error.Element("message")?.Value;
            }

            /// <summary>
            /// Gets the vault error code for this error
            /// </summary>
            public string ErrorCode { get; private set; }

            /// <summary>
            /// Gets the vault error message for this error
            /// </summary>
            public string ErrorMessage { get; private set; }

            /// <summary>
            /// Provide representation for display
            /// </summary>
            /// <returns>Error Type and message separated by ": "</returns>
            public override string ToString()
            {
                string returnValue = this.ErrorCode + ": " + this.ErrorMessage;
                return returnValue;
            }
        }

        /// <summary>
        /// Vault response status class
        /// </summary>
        public class VaultStatus
        {
            public const string API_LIMIT_EXCEEDED = "API_LIMIT_EXEEDED";

            public const string INVALID_SESSION_ID = "INVALID_SESSION_ID";

            /// <summary>
            /// Initializes a new instance of the <see cref="VaultStatus"/> class,
            /// </summary>
            /// <param name="element">XML node containing the status responses</param>

            public VaultStatus(XElement node)
            {
                if (!(node != null))
                {
                    throw new ArgumentNullException(nameof(node));
                }

                Contract.EndContractBlock();

                // var result = responseXml.Element("VersionedAuthRestResult").Descendants();
                // string responseStatus = responseXml.Descendants("responseStatus").FirstOrDefault().Value;
                string responseStatus = node.ApiResponseElement("responseStatus").Value;
                ////try
                ////{
                bool status = responseStatus.ToUpper().Equals("SUCCESS");
                XElement idNode = node.Element("id");
                if (idNode != null)
                {
                    this.Id = idNode.Value.ToString();
                }

                this.Success = status;
                if (status)
                {
                    this.Errors = null;
                }
                else
                {
                    //this.Errors = node.ApiResponseElements(errorsErrorExpression).Select(error => new VaultError(error)).ToList();
                    this.Errors = new List<VaultError>();
                    foreach (XElement errorNode in node.ApiResponseElement("errors").ApiResponseElements("error"))
                    {
                        this.Errors.Add(new VaultError(errorNode));
                    }
                }

                ////}
                ////catch (Exception e)
                ////{
                ////throw e;
                ////}
            }

            /// <summary>
            /// Gets a value indicating whether the operation succeeded or failed
            /// </summary>
            public bool Success { get; private set; }

            /// <summary>
            /// Gets the id for which the operation succeeded or failed.
            /// </summary>
            public string Id { get; private set; }


            public bool Busy
            {
                get
                {
                    return this.Errors?.Any(
                        e => e.ErrorCode == API_LIMIT_EXCEEDED)
                        ?? false;
                }
            }

            public bool InvalidSession
            {
                get
                {
                    return this.Errors?.Any(
                        e => e.ErrorCode == INVALID_SESSION_ID)
                        ?? false;
                }
            }


            /// <summary>
            /// Gets the list of errors for failed operations.
            /// </summary>
            public IList<VaultError> Errors { get; private set; }

            public override string ToString()
            {
                string returnValue = string.Join(Environment.NewLine, Errors.Select(error => error.ToString()));
                return returnValue;
            }
        }

        public class VaultResponse : VaultStatus
        {
            internal XElement responseXml;
            public VaultResponse(XElement responseXml) : base(responseXml)
            {
                this.responseXml = responseXml;
            }

            /// <summary>fArgument
            /// Get list of status from Vault XML response to batch request
            /// </summary>
            /// <param name="responseXml">Vault XML response</param>
            /// <returns>List of statuses corresponding to each batch item</returns>
            public IEnumerable<VaultStatus> GetStatusData(string dataTag)
            {
                if (!(dataTag != null))
                {
                    throw new ArgumentNullException(nameof(dataTag));
                }

                Contract.EndContractBlock();

                var dataTable = responseXml.ApiResponseElement(dataTag);
                var data = dataTable.ApiResponseElements(dataTag);
                var returnValue = new List<VaultStatus>();
                foreach (XElement datum in data)
                {
                    returnValue.Add(new VaultStatus(datum));
                }

                return returnValue;
            }
        }

        public class VaultTableParser
        {

            private readonly XPathExpression rowPathExpression;

            public VaultTableParser(string rowFilter)
            {
                if (!(rowFilter != null))
                {
                    throw new ArgumentNullException(nameof(rowFilter));
                }
                this.rowPathExpression = XPathExpression.Compile("//" + rowFilter);
            }

            /// <summary>
            /// Collect the items from a vault query response
            /// </summary>
            /// <param name="fieldNames">the collected tag names</param>
            /// <param name="collector">the source XML</param>
            /// <returns>the data items collected</returns>
            public List<string[]> CollectItemsFromResponse(ref string[] fieldNames, VaultResponse response)
            {
                List<string[]> items = new List<string[]>();

                Contract.EndContractBlock();

                Dictionary<string, string> firstRow;
                if (fieldNames == null)
                {
                    firstRow = new Dictionary<string, string>();
                }
                else
                {
                    firstRow = null;
                }

                //bool hasVersionColumns = false;)
                foreach (XElement row in response.responseXml.Select(rowPathExpression))
                {
                    string[] fieldValues = null;
                    if (firstRow == null)
                    {
                        fieldValues = new string[fieldNames.Length];
                    }
                    var columns = row.Elements();
                    foreach (XElement column in columns)
                    {
                        string columnName = column.GetLocalName();
                        if (firstRow != null)
                        {
                            firstRow.Add(columnName, column.Value);
                        }
                        else
                        {
                            int fieldNo = Array.IndexOf(fieldNames, columnName);
                            if (fieldNo < 0)
                            {
                                throw new ItemSourceException(string.Format("Unexpected field {0}", columnName));
                            }
                            fieldValues[fieldNo] = column.Value.ToString();
                        }
                    }
                    if (firstRow != null)
                    {
                        fieldNames = firstRow.Keys.ToArray();
                        fieldValues = firstRow.Values.ToArray();
                        firstRow = null;
                    }
                    items.Add(fieldValues);

                    //XElement idNode = row.ApiResponseElement(IdFieldName);
                    //XElement majorVersionNode = row.Element(MajorVersionFieldName);
                    //XElement minorVersionNode = row.Element(MinorVersionFieldName);
                    //if (majorVersionNode != null && minorVersionNode != null)
                    //{
                    //    hasVersionColumns = true;
                    //}
                    //else if (majorVersionNode != null || minorVersionNode != null)
                    //{
                    //    throw new ItemSourceException("Major and Minor Version number must be used together");
                    //}

                    //items.Add(
                    //    new ItemReference
                    //    {
                    //        Id = idNode.Value,
                    //        WithVersion = hasVersionColumns,
                    //        MajorVersion = GetValueOfOptionalXml(majorVersionNode),
                    //        MinorVersion = GetValueOfOptionalXml(minorVersionNode)
                    //    });
                }

                return items;
            }

        }
    }
}
// end changes by MP on 07/01/2019