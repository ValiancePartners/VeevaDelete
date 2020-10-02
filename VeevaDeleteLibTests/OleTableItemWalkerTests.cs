//-----------------------------------------------------------------------
// <copyright file="OleTableItemWalkerTest.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestUtility.Api;
using TextTable.Reader;
using VeevaDeleteApi;

namespace VeevaDeleteApi.Tests
{
    /// <summary>
    /// This is a test class for OleTableItemWalkerTest and is intended
    /// to contain all OleTableItemWalkerTest Unit Tests
    /// </summary>
    [TestClass]
    public class OleTableItemWalkerTests
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
        ////You can use the following additional attributes as you write your tests:
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
        #endregion

        /// <summary>
        /// A test for Open incomplete table. verify that an exception is thrown on opening if there is no status column
        /// </summary>
        [TestMethod()]
        [DeploymentItem("Data\\OneIdNoStatus.xlsx")]
        [ExpectedException(typeof(ItemSourceException))]
        public void OpenExcelNoStatusTest()
        {
            OleTableItemWalker privateTarget = this.CreateOleTableItemWalker("OneIDNoStatus.xlsx", true);
            privateTarget.Open();
        }


        /// <summary>
        /// A test for Close
        /// </summary>
        [TestMethod()]
        [DeploymentItem("Data\\OneId.xlsx")]
        public void CloseExcelTest()
        {
            OleTableItemWalker privateTarget = this.CreateOleTableItemWalker();
            privateTarget.Open();
            privateTarget.Close();
        }

        /// <summary>
        /// A test for Close
        /// </summary>
        [TestMethod()]
        [DeploymentItem("Data\\OneId.xlsx")]
        public void OpenExcelWithStatusTest()
        {
            OleTableItemWalker privateTarget = this.CreateOleTableItemWalker("OneID.xlsx",true);
            privateTarget.Open();
            privateTarget.Close();
        }
        /// <summary>
        /// A test for GetItemCount
        /// </summary>
        [TestMethod()]
        [DeploymentItem("Data\\OneId.xlsx")]
        public void GetItemCountTest()
        {
            OleTableItemWalker target = this.CreateOleTableItemWalker();
            int expected = 1;
            int actual;
            actual = target.GetItemCount();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for GetTable
        /// </summary>
        [TestMethod()]
        [DeploymentItem("Data\\OneId.xlsx")]
        public void GetReaderTest()
        {
            OleTableItemWalker target = this.CreateOleTableItemWalker();
            IDataReader actual;
            actual = target.GetReader();
            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// A test for Visited
        /// </summary>
        [TestMethod()]
        [DeploymentItem("Data\\OneId.xlsx")]
        public void VisitedTest()
        {
            OleTableItemWalker target = this.CreateOleTableItemWalker();
            ItemReference item = new ItemReference { Id = "20" };
            string expectedStatus = "Updated";
            target.Visited(item, expectedStatus);
            IDataReader reader = target.GetReader();
            reader.Read();
            int statusColNo = reader.GetOrdinal("status");
            string actualStatus = null;
            if (!reader.IsDBNull(statusColNo)) {
                actualStatus = reader.GetString(statusColNo);
            }
            Assert.AreEqual(expectedStatus, actualStatus);
        }

        /// <summary>
        /// Test that proejction is calculated correctly per table name and withVersion
        /// 
        /// </summary>
        [TestMethod()]
        [DeploymentItem("Data\\OneId.xlsx")]
        public void GetTableContentQueryCommandTextTest()
        {
            OleTableItemWalker walker= this.CreateOleTableItemWalker();
            //string expected = "SELECT id,status FROM [OneID$]";
            string expected = "SELECT * FROM [OneID$]";
            string actual = walker.GetTableContentQueryCommandText();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// create and open a standard excel file walker
        /// </summary>
        /// <param name="filename">the name of the excel file, default OneId</param>
        /// <returns>an opened excel file walker</returns>
        internal virtual OleTableItemWalker CreateOleTableItemWalker(string filename = "OneID.xlsx", bool withVersion = false)
        {
            var itemType = new DeletionRequest.ItemType { WithVersion = withVersion };
            OleTableItemWalker target = new ExcelTableItemWalker { Filename = filename, ItemType = itemType } as OleTableItemWalker;
            target.Open();
            return target;
        }
    }
}
