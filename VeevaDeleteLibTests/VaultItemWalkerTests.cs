//-----------------------------------------------------------------------
// <copyright file="VaultItemWalkerTest.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestUtility.Api;
using RestUtility.Tests;
using VeevaDeleteApi;


namespace VeevaDeleteLib.Tests
{
    /// <summary>
    /// This is a test class for VaultItemWalkerTest and is intended
    /// to contain all VaultItemWalkerTest Unit Tests
    /// </summary>
    [TestClass]
    public class VaultItemWalkerTest
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

        [TestMethod()]
        public void TableNameTest()
        {
            MockWebConnection connection = new MockWebConnection(
                new MockWebConnection.Transfer { Request = MockWebConnection.MockQueryCountRequest });
            VaultItemWalker target = new VaultItemWalker
            {
                Connection = connection,
                Query = MockWebConnection.MockQuery
            };
            string expected = "VQL Query";
            string actual = target.TableName;
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        /// A test for GetTable
        /// </summary>
        [TestMethod()]
        public void GetReaderTest()
        {
            MockWebConnection connection = new MockWebConnection(
                new MockWebConnection.Transfer { Request = MockWebConnection.MockQueryDataRequest });
            VaultItemWalker target = new VaultItemWalker
            {
                Connection = connection,
                Query = MockWebConnection.MockQuery
            };
            int expected = MockWebConnection.MockCount;
            IDataReader reader = target.GetReader();
            int actual = 0;
            while (reader.Read()) ++actual;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for GetItemCount
        /// </summary>
        [TestMethod()]
        public void GetItemCountTest()
        {
            MockWebConnection connection = new MockWebConnection(
                new MockWebConnection.Transfer { Request = MockWebConnection.MockQueryCountRequest });
            VaultItemWalker target = new VaultItemWalker
            {
                Connection = connection,
                Query = MockWebConnection.MockQuery
            };
            int expected = MockWebConnection.MockCount;
            int actual;
            actual = target.GetItemCount();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for VaultItemWalker Constructor
        /// </summary>
        [TestMethod()]
        public void VaultItemWalkerConstructorTest()
        {
            VaultItemWalker target = new VaultItemWalker();
            Assert.IsInstanceOfType(target, typeof(VaultItemWalker));
        }

   }
    
}