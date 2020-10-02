//-----------------------------------------------------------------------
// <copyright file="ExcelTableItemWalkerTest.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Data.OleDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VeevaDeleteApi;

namespace VeevaDeleteLib.Tests
{
    /// <summary>
    /// This is a test class for ExcelTableItemWalkerTest and is intended
    /// to contain all ExcelTableItemWalkerTest Unit Tests
    /// </summary>
    [TestClass]
    public class ExcelTableItemWalkerTest
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

        /// <summary>
        /// A test for ExcelTableItemWalker Constructor
        /// </summary>
        [TestMethod()]
        public void ExcelTableItemWalkerConstructorTest()
        {
            ExcelTableItemWalker target = new ExcelTableItemWalker();
        }

        /// <summary>
        /// A test for OpenConnection
        /// </summary>
        [TestMethod()]
        [DeploymentItem("VeevaDeleteLib.dll")]
        public void OpenConnectionTest()
        {
            ExcelTableItemWalker excelWalker = new ExcelTableItemWalker { Filename = "OneID.xlsx"};
            //TODO: Provide ItemType?
            excelWalker.Open();
        }
    }
}
