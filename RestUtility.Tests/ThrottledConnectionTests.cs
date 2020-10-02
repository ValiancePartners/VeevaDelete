using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RestUtility.Api;
using RestUtility.VeevaVaultXml;

namespace RestUtility.Tests
{
    [TestClass()]
    public class ThrottledConnectionTests
    {
        [TestClass()]
        public class ThrottledConnectionTest
        {
            [TestMethod()]
            [ExpectedException(typeof(UnexpectedResponseRestApiException))]
            public void ExecuteApiTest()
            {
                string expected = "result";
                ThrottledConnection connectionTarget = new MockThrottledConnection
                {
                    ReturnContent = new string[] { expected }
                };
                VaultXmlHelper.VaultResponse response = connectionTarget.ExecuteXmlApi("GET", "http://localhost:");
            }


            /// <summary>
            /// A test for OnLimitUpdated
            /// </summary>
            [TestMethod()]
            [DeploymentItem("TextTable.dll")]
            public void OnLimitUpdatedTest()
            {
                ThrottledConnection connectionTarget = new MockThrottledConnection();
                var limits = new Tuple<long?, long?>(null, null);
                connectionTarget.LimitUpdated += (object sender, LimitEventArgs eventLimitArgs) =>
                {
                    limits = new Tuple<long?, long?>(eventLimitArgs.BurstLimit,eventLimitArgs.DailyLimit);
                };

                int burstLimit = 100;
                int dailyLimit = 10000;

                var expectedLimits = new Tuple<long?, long?>(burstLimit, dailyLimit);
                NameValueCollection headers = new NameValueCollection { ["Burst"]= burstLimit.ToString() , ["Daily"]=dailyLimit.ToString() } ;
                connectionTarget.PostValidate(headers);
                //..connectionTarget.OnLimitUpdated(fakeSender, expectedLimits);
                Assert.AreEqual(expectedLimits, limits);
            }

            /// <summary>
            /// A test for StartSession
            /// </summary>
            [TestMethod()]
            public void SetupConnectionTest()
            {
                ThrottledConnection connectionTarget = new MockThrottledConnection();
                Uri baseUrl = new Uri("https://localhost");
                string expectedUsername = "-username-";
                string expectedPassword = "-password-";
                connectionTarget.SetupConnection(1.2M, baseUrl, expectedUsername, expectedPassword);
                Assert.AreEqual((connectionTarget as IRestConnection).ApiVersion, 1.2M);
                Assert.AreEqual(connectionTarget.DefaultUrl, baseUrl);
                (connectionTarget as IRestConnection).GetCredentials(out string actualUsername, out string actualPassword);
                Assert.AreEqual(actualUsername, expectedUsername);
                Assert.AreEqual(actualPassword, expectedPassword);
            }


            /// <summary>
            /// A test for StartSession
            /// </summary>
            [TestMethod()]
            public void SessionTest()
            {
                ThrottledConnection connectionTarget = new MockThrottledConnection();
                string sessionId = "-SessionID-";
                connectionTarget.SessionId = sessionId;
                Assert.AreEqual(connectionTarget.SessionId, sessionId);
            }



            /// <summary>
            /// A test for milliseconds To next slice from the start of a five-minute slice
            /// </summary>
            [DataTestMethod()]
            [DataRow(0, 0, 300, 300)]
            [DataRow(4, 59, 1, 300)]
            [DataRow(0, 0, 3600, 3600)]
            [DataRow(59, 59, 1, 3600)]
            public void MilliSecondsToNextSliceTest(int fromMinute, int fromSecond, int seconds, int sliceSeconds)
            {
                DateTime from = new DateTime(1900, 1, 1, 0, fromMinute, fromSecond);
                int expected = seconds * 1000;
                int actual;
                actual = ThrottledConnection.MilliSecondsToNextSlice(from, sliceSeconds);
                Assert.AreEqual(expected, actual);
            }


            /// <summary>
            /// A test for next time slice calculation for five-minute slice
            /// </summary>
            [TestMethod()]
            public void NextTimeSliceBurstTest()
            {
                ThrottledConnection connection = new MockThrottledConnection();
                DateTime from = new DateTime(1900, 1, 1, 0, 39, 59);
                DateTime expected = new DateTime(1900, 1, 1, 0, 40, 0);
                DateTime actual;
                actual = connection.NextTimeSlice(from, false);
                Assert.AreEqual(expected, actual);
            }

            /// <summary>
            /// A test for next time slice calculation for hourly slice
            /// </summary>
            [TestMethod()]
            public void NextTimeSliceDailyTest()
            {
                ThrottledConnection connection = new MockThrottledConnection();
                DateTime from = new DateTime(1900, 1, 1, 0, 39, 58);
                DateTime expected = new DateTime(1900, 1, 1, 0, 40, 0);
                DateTime actual;
                actual = connection.NextTimeSlice(from, true);
                Assert.AreEqual(expected, actual);
            }

            [TestMethod]
            [ExpectedException(typeof(ArgumentNullException))]
            public void FindLimitUpdateTestThrowsArgumentNullException()
            {
                LimitEventArgs limitEventArgs;
                limitEventArgs = ThrottledConnection.FindLimitUpdate(null, null, null);
            }

            [TestMethod]
            public void FindLimitUpdateTestEmptyCollection()
            {
                var nameValueCollection = new NameValueCollection { };
                LimitEventArgs actual;
                actual = ThrottledConnection.FindLimitUpdate(nameValueCollection, "1", "2");
                Assert.IsNull(actual);
            }

            [TestMethod]
            public void FindLimitUpdateTestOneItemCollection()
            {
                var nameValueCollection = new NameValueCollection
                {
                    ["0"] = "a"
                };
                LimitEventArgs expected = new LimitEventArgs(null, null);
                LimitEventArgs actual;
                actual = ThrottledConnection.FindLimitUpdate(nameValueCollection, "1", "2");
                Assert.IsNull(actual);
            }

            [TestMethod]
            public void FindLimitUpdateTestWithLimitCollection()
            {
                LimitEventArgs expected = new LimitEventArgs(100, 1000);
                var nameValueCollection = new NameValueCollection
                {
                    ["1"] = expected.BurstLimit.ToString(),
                    ["2"] = expected.DailyLimit.ToString(),
                };
                LimitEventArgs actual;
                actual = ThrottledConnection.FindLimitUpdate(nameValueCollection, "1", "2");
                Assert.AreEqual(expected.DailyLimit, actual.DailyLimit);
                Assert.AreEqual(expected.BurstLimit, actual.BurstLimit);
            }

        }

        /// <summary>
        /// A test for OpenConnection with valid connection parameters
        /// </summary>
        [TestMethod()]
        public void OpenConnectionSuccessSessionID()
        {
            string expected = "--SessionID--";
            ThrottledConnection target = new MockThrottledConnection
            {
                Headers = new WebHeaderCollection { ["SessionId"] = "--SessionID--" },
                ReturnContent = new string[] { "<x><responseStatus>SUCCESS</responseStatus><sessionId>" + expected + "</sessionId></x>" }
            };
            ConnectionParameters connectionParameters = new ConnectionParameters
            {
                Url = "http://localhost",
                SubService = "v2",
                Username = "3",
                Password = "4",
                ClientId="5"
            };
            target.OpenConnection(connectionParameters);
            string actual = target.SessionId;
            Assert.AreEqual(target.SessionId, expected);
        }

    }
}