using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestUtility.Api;
using RestUtility.VeevaVaultWeb;

namespace RestUtility.Tests
{
    [TestClass()]
    public class ConnectionUtilityTests
    {
        [TestClass()]
        [DeploymentItem("TextTable.dll")]
        public class ConnectionUtilityTest
        {
            /// <summary>
            /// use current API on sandbox for login (failure) test
            /// </summary>
            private readonly string defaultBaseUrl = "https://vaultpartners-valiance-etmf-sbx.veevavault.com/api/v18.1";


            /// <summary>
            /// A test for Connection Utility Constructor
            /// </summary>
            [TestMethod()]
            public void ConnectionUtilConstructorTest()
            {
                ConnectionUtility connectionTarget = new ConnectionUtility();
                Assert.IsInstanceOfType(connectionTarget, typeof(ConnectionUtility));
                Assert.IsNull(connectionTarget.DefaultUrl);
                Assert.AreEqual(string.Empty, connectionTarget.SessionId);
            }


            /// the below tests need a site to test against willing to service the requests, no longer the case with Veeva
            /// <summary>
            /// A test for Execute API
            /// </summary>
            [TestMethod()]
            //[ExpectedException(typeof(UnsupportedRequestRestApiContractException))]
            public void ExecuteApiTest()
            {
                IRestConnection target = new ConnectionUtility();
                string method = "POST";
                string vaultUri = this.defaultBaseUrl;
                bool expected = true;
                bool actual;
                var processor = new Dictionary<string, Func<Stream, bool>> { { "application/xml", (Stream stream) => true } };
                actual = target.ExecuteApi(processor, method, vaultUri);
                Assert.AreEqual(expected, actual);
            }
        }
    }
}