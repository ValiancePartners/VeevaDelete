//-----------------------------------------------------------------------
// <copyright file="CsvFileItemWalkerTest.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VeevaDeleteApi;

namespace VeevaDeleteLib.Tests
{
    /// <summary>
    /// This is a test class for CSV File Item Walker and is intended
    /// to contain all CSV File Item Walker Unit Tests
    /// </summary>
    [TestClass]
    public class CsvFileItemWalkerTest
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
        /// A test for GetTable
        /// </summary>
        [TestMethod()]
        [DeploymentItem("Data\\OneId.csv")]
        public void GetDataReaderTest()
        {
            CsvFileItemWalker target = new CsvFileItemWalker { Filename = "OneID.csv" };
            IDataReader actual;
            actual = target.GetReader();
            Assert.AreEqual(3, actual.FieldCount);
            List<string> columnNames = new List<string>();
            foreach (DataRow colRow in actual.GetSchemaTable().Rows)
            {
                columnNames.Add(colRow.ItemArray[0].ToString());
            }
            CollectionAssert.AreEquivalent(new string[] { VaultGrid.Column.IdFieldName, VaultGrid.Column.MajorVersionFieldName, VaultGrid.Column.MinorVersionFieldName }, columnNames);

            //TODO: Verify Content
        }

        /// <summary>
        /// A test for GetItemCount
        /// </summary>
        [TestMethod()]
        [DeploymentItem("Data\\OneId.csv")]
        public void GetItemCountTest()
        {
            CsvFileItemWalker target = new CsvFileItemWalker { Filename = "OneID.csv" };
            int expected = 1;
            int actual;
            actual = target.GetItemCount();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for GetItemCount with missing version
        /// </summary>
        [TestMethod()]
        [DeploymentItem("Data\\OneId.csv")]
        public void GetItemCountMissingVersionTest()
        {
            CsvFileItemWalker target = new CsvFileItemWalker
            {
                Filename = "OneID.csv",
                ItemType = new DeletionRequest.ItemType { WithVersion = true }
            };
            int expected = 1;
            int actual;
            actual = target.GetItemCount();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for GetItemCount
        /// </summary>
        [TestMethod()]
        [DeploymentItem("Data\\OneId3BlankLines.csv")]
        public void GetItemCountWithBlanksTest()
        {
            CsvFileItemWalker target = new CsvFileItemWalker { Filename = "OneId3BlankLines.csv" };
            int expected = 1;
            int actual;
            actual = target.GetItemCount();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for CSV File Item Walker Constructor
        /// </summary>
        [TestMethod()]
        public void CsvFileItemWalkerConstructorTest()
        {
            CsvFileItemWalker target = new CsvFileItemWalker();
            Assert.IsNotNull(target);
        }
    }
}
