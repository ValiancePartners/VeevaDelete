//-----------------------------------------------------------------------
// <copyright file="VaultXmlHelperTest.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestUtility.Api;
using RestUtility.VeevaVaultXml;
using System.Text;

namespace RestUtility.Tests
{
    /// <summary>
    /// This is a test class for VaultXmlHelperTest and is intended
    /// to contain all VaultXmlHelperTest Unit Tests
    /// </summary>
    [TestClass]
    public class VaultXmlHelperTest
    {
        /// <summary>
        /// provides information and functionality to the current test run
        /// </summary>
        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get
            {
                return this.testContextInstance;
            }

            set
            {
                this.testContextInstance = value;
            }
        }

        private Stream MakeStream(string value)
        {
            return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(value));
        }

        private XPathNavigator MakeXml(string value)
        {
            using (Stream stream = MakeStream(value))
            {
                XPathDocument document = new XPathDocument(stream);
                var responseXml = document.RootElement();
                return responseXml;
            }
        }

        private void GetTestPassResponse(string value)
        {
            var responseXml = MakeXml(value);
            var status = new VaultXmlHelper.VaultStatus(responseXml);
            VaultXmlHelper.ThrowUnlessSuccess(status);
        }

        public static string GetXmlString(XPathNavigator xml)
        {
            string returnValue;
            if (xml.NodeType == XPathNodeType.Attribute)
            {
                returnValue = xml.Name + "=\"" + xml.Value + "\"";
            }
            else if (xml.NodeType == XPathNodeType.Namespace)
            {
                if (xml.LocalName.Length == 0)
                {
                    returnValue = "xmlns=\"" + xml.Value + "\"";
                }
                else
                {
                    returnValue = "xmlns:" + xml.LocalName + "=\"" + xml.Value + "\"";
                }
            }
            else
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = false,
                    OmitXmlDeclaration = true,
                    ConformanceLevel = ConformanceLevel.Auto,
                    CloseOutput = true
                };
                StringBuilder sb = new StringBuilder();
                StringWriter output = new StringWriter(sb, CultureInfo.InvariantCulture);
                using (XmlWriter writer = XmlWriter.Create(output, settings))
                {
                    writer.WriteNode(xml, false);
                }
                returnValue = sb.ToString();
            }
            return returnValue;
        }

        public static string GetXmlString(XNode xml)
        {
            return xml.ToString();
        }


        #region Additional test attributes
        ////
        ////You can use the following additional attributes as you write your tests:
        ////
        ////Use ClassInitialize to run code before running the first test in the class
        ////[ClassInitialize()]
        ////public static void MyClassInitialize(TestContext testContext)
        ////{
        ////}
        ////
        ////Use ClassCleanup to run code after all tests in a class have run
        ////[ClassCleanup()]
        ////public static void MyClassCleanup()
        ////{
        ////}
        ////
        ////Use TestInitialize to run code before running each test
        ////[TestInitialize()]
        ////public void MyTestInitialize()
        ////{
        ////}
        ////
        ////Use TestCleanup to run code after each test has run
        ////[TestCleanup()]
        ////public void MyTestCleanup()
        ////{
        ////}
        ////
        #endregion

        /// <summary>
        /// A test for GetPassResponseXml with null string
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetResponseXmlNullTest()
        {
            VaultXmlHelper.GetResponseXml(null);
        }

        /// <summary>
        /// A test for GetResponseXml with empty string
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(UnexpectedResponseRestApiException))]
        public void GetResponseXmlEmptyTest()
        {
            using (var stream = MakeStream(string.Empty)) VaultXmlHelper.GetResponseXml(stream);
        }

        /// <summary>
        /// A test for GetResponseXml success
        /// </summary>
        [TestMethod()]
        public void GetResponseXmlTest()
        {
            string expected = "<x></x>";
            using (var stream = MakeStream(expected))
            {
                var responseXml = VaultXmlHelper.GetResponseXml(stream);
                var actual = GetXmlString(responseXml);
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for GetPassResponseXml successful
        /// </summary>
        [TestMethod()]
        public void GetPassResponseXmlSuccessTest()
        {
            string expected = MockWebConnection.MockSuccessReponse;
            using (var stream = MakeStream(expected))
            {
                var responseXml = VaultXmlHelper.GetResponseXml(stream);
                var actual = GetXmlString(responseXml);
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// A test for GetPassResponseXml API response failure
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(XmlException))]
        public void GetPassResponseXmlNoneTest()
        {
            GetTestPassResponse(string.Empty);
        }

        /// <summary>
        /// A test for GetPassResponseXml API response incomplete
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(UnexpectedResponseRestApiException))]
        public void GetPassResponseXmlBadTest()
        {
            GetTestPassResponse("<x></x>");
        }

        /// <summary>
        /// A test for GetPassResponseXml denied failure
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(VaultApiException))]
        public void GetPassResponseXmlFailTest()
        {
            GetTestPassResponse(MockWebConnection.MockFailResponse);
        }

        /// <summary>
        /// A test for GetPassResponseXml non-success response status
        /// </summary>
        [ExpectedException(typeof(UnexpectedResponseRestApiException))]
        [TestMethod()]
        public void GetPassResponseXmlFaultTest()
        {
            GetTestPassResponse("<x><responseStatus/></x>");
        }

        ///// <summary>
        ///// A test for GetVaultStatus with empty XML Document
        ///// </summary>
        //[TestMethod()]
        //[ExpectedException(typeof(ArgumentOutOfRangeException))]
        //public void GetVaultStatusNoRootTest()
        //{
        //    VaultXmlHelper.GetVaultStatus(new XDocument());
        //}

        /// <summary>
        /// A test for GetVaultStatus for no error details
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(UnexpectedResponseRestApiException))]
        public void GetVaultStatusEmptyResponseTest()
        {
            var responseXml = MakeXml("<x></x>");
            string expected = string.Empty;
            VaultXmlHelper.VaultStatus actual = VaultXmlHelper.GetVaultStatus(responseXml);
            Assert.AreEqual(expected, actual.Success);
        }

        /// <summary>
        /// A test for GetVaultStatus with error details
        /// </summary>
        [TestMethod()]
        public void GetVaultStatusTest()
        {
            var responseXml = MakeXml(MockWebConnection.MockFailResponse);
            string expected = MockWebConnection.MockFailType + ": " + MockWebConnection.MockFailMessage;
            VaultXmlHelper.VaultStatus actual = VaultXmlHelper.GetVaultStatus(responseXml);
            Assert.AreEqual(expected, actual.Errors[0].ToString());
        }

        /// <summary>
        /// A test for GetVaultStatus for failure
        /// </summary>
        [TestMethod()]
        public void GetVaultStatusFailureTest()
        {
            string responseStr = @"<X><responseStatus>FAILURE</responseStatus><errors><error><type/></error></errors></X>";
            var responseXml = MakeXml(responseStr);
            bool expected = false;
            VaultXmlHelper.VaultStatus actual = VaultXmlHelper.GetVaultStatus(responseXml);
            Assert.AreEqual(expected, actual.Success);
        }

        [TestMethod()]
        public void InvalidSessionStatusTest()
        {
            string responseStr =
                "<ApiRestResult><responseStatus>FAILURE</responseStatus><errors><error>" +
                "<type>INVALID_SESSION_ID</type><message>Invalid or expired session ID.</message>" +
                "</error></errors></ApiRestResult>";
            var responseXml = MakeXml(responseStr);
            var status = new VaultXmlHelper.VaultStatus(responseXml);
            bool expected = true;
            bool actual = status.InvalidSession;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for GetVaultStatus for success
        /// </summary>
        [TestMethod()]
        public void GetVaultStatusSuccessTest()
        {
            var responseXml = MakeXml(MockWebConnection.MockSuccessReponse);
            bool expected = true;
            VaultXmlHelper.VaultStatus actual = VaultXmlHelper.GetVaultStatus(responseXml);
            Assert.AreEqual(expected, actual.Success);
        }

        /// <summary>
        /// A test for GetResponseDetailsFromRequest
        /// </summary>
        [TestMethod()]
        public void GetPassResponseFromRequestTest()
        {
            MockWebConnection connection = new MockWebConnection(
                 new MockWebConnection.Transfer { Request = MockWebConnection.MockQueryDataRequest });
            VaultXmlHelper.VaultResponse response = connection.ExecuteXmlApi("GET", MockWebConnection.MockQueryUrn);
            //actual = GetXmlString(response.responseXml);//.ToString(SaveOptions.DisableFormatting);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// Test for GetCount where API doesn't return a response
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(UnexpectedResponseRestApiException))]
        public void GetCountBadResponseTest()
        {
            IRestConnection target = new MockWebConnection(
                new MockWebConnection.Transfer { Request = MockWebConnection.MockQueryCountRequest, Response = string.Empty });
            target.GetCount(MockWebConnection.MockQuery);
        }

        /// <summary>
        /// A test for GetCount where API doesn't return a count
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(UnexpectedResponseRestApiException))]
        public void GetCountEmptyTest()
        {
            MockWebConnection connection = new MockWebConnection(
                new MockWebConnection.Transfer[]
                {
                    new MockWebConnection.Transfer { Response = MockWebConnection.MockSuccessReponse }
                });
            string vql = MockWebConnection.MockQuery;
            int expected = -1;
            int actual;
            actual = connection.GetCount(vql);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for OpenConnection
        /// </summary>
        [TestMethod()]
        public void OpenConnectionTest()
        {
            ConnectionParameters connectionParameters = new ConnectionParameters
            {
                Url = "http://localhost",
                SubService = "v2",
                Username = "3",
                Password = "4",
                ClientId = "ClientID"
            };
            string expectedSessionId = "6";
            string expectedDefaultUrl = connectionParameters.Url + "/api/" +
                connectionParameters.SubService + "/";
            string expectedLoginSlug = "auth?username=" +
                connectionParameters.Username + "&password=" + connectionParameters.Password;

            MockWebConnection connection = new MockWebConnection(
                new MockWebConnection.Transfer[]
                {
                    new MockWebConnection.Transfer
                    {
                        Request = "POST " + expectedLoginSlug,
                        Response = "<x><responseStatus>SUCCESS</responseStatus><sessionId>" +
                        expectedSessionId + "</sessionId></x>"
                    }
                });

            VaultXmlHelper.OpenConnection(connection, connectionParameters);

            Assert.AreEqual(connectionParameters.ClientId.ToLower(), connection.ClientId);
            Assert.AreEqual(expectedSessionId, connection.SessionId);
            Assert.AreEqual(expectedDefaultUrl, connection.DefaultUrl.OriginalString);
        }

        /// <summary>
        /// A test for Open Connection failure
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(VaultApiException))]
        public void StartConnectionSiteFailTest()
        {
            MockWebConnection target = new MockWebConnection(
                new MockWebConnection.Transfer[]
                {
                    new MockWebConnection.Transfer
                    {
                        Request = "POST " + MockWebConnection.MockLoginUri,
                        Response = MockWebConnection.MockFailResponse
                    }
                });
            target.OpenConnection(MockWebConnection.MockParameters);
        }

        /// <summary>
        /// A test for OpenConnection with invalid connection url
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(UnsupportedRequestRestApiContractException))]
        public void OpenConnectionBadUriTest()
        {
            IRestConnection target = new MockWebConnection(
                    new MockWebConnection.Transfer[]
                    {
                        new MockWebConnection.Transfer
                        {
                            Request = "POST /api/v1/auth?username=&password="
                        }
                    });
            ConnectionParameters connectionParameters = new ConnectionParameters
            {
                Url = string.Empty,
                SubService = "v1",
                Username = string.Empty,
                Password = string.Empty,
                ClientId = string.Empty
            };
            target.OpenConnection(connectionParameters);
        }


        /// <summary>
        /// A test for OpenConnection with invalid connection api version
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void OpenConnectionBadVersionTest()
        {
            IRestConnection target = new MockWebConnection(
                    new MockWebConnection.Transfer[]
                    {
                        new MockWebConnection.Transfer
                        {
                            Request = "POST /api//auth?username=&password="
                        }
                    });

            ConnectionParameters connectionParameters = new ConnectionParameters
            {
                Url = string.Empty,
                SubService = string.Empty,
                Username = string.Empty,
                Password = string.Empty
            };
            target.OpenConnection(connectionParameters);
        }

    }
}
