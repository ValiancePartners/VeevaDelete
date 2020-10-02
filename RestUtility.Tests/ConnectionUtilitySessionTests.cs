using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestUtility.Api;
using RestUtility.VeevaVaultWeb;
using RestUtility.VeevaVaultXml;

namespace RestUtility.Tests
{
    [TestClass()]
    public class ConnectionUtilitySessionTests
    {
        [TestClass()]
        public class ConnectionUtilitySessionTest
        {
            /// <summary>
            /// used sandbox for tests needing a response
            /// </summary>
            private static string testSiteUrl = "https://vaultpartners-valiance-etmf-sbx.veevavault.com";

            /// <summary>
            /// A test for OpenConnection with invalid connection parameters
            /// </summary>
            [TestMethod()]
            [ExpectedException(typeof(VaultApiException))]
            public void OpenConnectionDeniedTest()
            {
                IRestConnection target = new ConnectionUtility();
                ConnectionParameters connectionParameters = new ConnectionParameters
                {
                    Url = ConnectionUtilitySessionTest.testSiteUrl,
                    SubService = "v18.1",
                    Username = "a@vaultpartners.com",
                    Password = "b",
                    ClientId="valiance-valiance-etmf-client-veevadelete"
                };
                target.OpenConnection(connectionParameters);
            }

            /// <summary>
            /// A test for OpenConnection with valid connection parameters
            /// </summary>
            [TestMethod()]
            public void OpenConnectionSuccessTest()
            {
                IRestConnection target = new ConnectionUtility();
                ConnectionParameters connectionParameters = new ConnectionParameters
                {
                    Url = ConnectionUtilitySessionTest.testSiteUrl,
                    Username = "mpadden@vaultpartners.com",
                    Password = "Valiance000",
                    SubService = "v17.2",
                    ClientId = "valiance-valiance-etmf-client-veevadelete"
                };
                target.OpenConnection(connectionParameters);
            }
        }
    }
}