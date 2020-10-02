//-----------------------------------------------------------------------
// <copyright file="LoggerTest.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VeevaDeleteApi;

namespace VeevaDeleteLib.Tests
{
    /// <summary>
    /// This is a test class for LoggerTest and is intended
    /// to contain all LoggerTest Unit Tests
    /// </summary>
    [TestClass]
    public class LoggerTest
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
        /// A test for WriteMessage
        /// </summary>
        [TestMethod()]
        public void WriteMessageTest()
        {
            string fileBasePath = Path.Combine(TestContext.TestDir, "testWriteErr");
            string fileFullPath = fileBasePath + ".log";
            string message1 = "Test Log Error1";
            string message2 = "Test Log Error2";
            DeleteIfExists(fileFullPath);
            Logger listener = new Logger(fileBasePath);

            Logger.WriteMessage(message1);
            Logger.WriteMessage(message2);

            listener.Dispose();
            listener = null;
            string[] loggedLines = File.ReadAllLines(fileFullPath);
            string[] expectedLines = new string[] { message1, message2 };

            CollectionAssert.AreEqual(expectedLines, loggedLines);
        }

        /// <summary>
        /// A test for Logger Constructor
        /// </summary>
        [TestMethod()]
        public void LoggerConstructorTest()
        {
            string fileBasePath = Path.Combine(TestContext.TestDir, "testctr");
            string fileFullPath = fileBasePath + ".log";
            DeleteIfExists(fileFullPath);
            Logger target = new Logger(fileBasePath);
            Logger.WriteMessage(string.Empty); // log file not created until a message is written
            Assert.IsTrue(File.Exists(fileFullPath), "log file not found");
        }

        /// <summary>
        /// A test for WriteError
        /// </summary>
        [TestMethod()]
        public void WriteErrorTest()
        {
            string fileBasePath = Path.Combine(TestContext.TestDir, "testWriteErr");
            string fileFullPath = fileBasePath + ".log";
            string message = "Test Log Error";
            Exception ex = new Exception("Test Log Exception");
            bool time = false; // don't include timestamp (how should we test that?!!

            DeleteIfExists(fileFullPath);
            Logger listener = new Logger(fileBasePath);

            // test for non-fatal and fatal
            Logger.WriteError(message, ex, false, time);
            Logger.WriteError(message, ex, true, time);

            listener.Dispose();
            listener = null;
            string[] loggedLines = File.ReadAllLines(fileFullPath);
            string[] expectedLines = new string[]
            {
                "****ERROR****", message, ex.ToString(), string.Empty, // blank line at end
                "****FATAL ERROR****", message, ex.ToString(), string.Empty // blank line at end
            };

            CollectionAssert.AreEqual(expectedLines, loggedLines);
        }

        /// <summary>
        /// A test for Close
        /// </summary>
        [TestMethod()]
        public void CloseTest()
        {
            string fileBasePath = Path.Combine(TestContext.TestDir, "testclose");
            string fileFullPath = fileBasePath + ".log";
            DeleteIfExists(fileFullPath);
            Logger target = new Logger(fileBasePath);
            target.Close();

            // file should exist and be closed
            File.Delete(fileFullPath);
        }

        /// <summary>
        /// if the file exists, delete it
        /// </summary>
        /// <param name="fileBasePath">the name of the file</param>
        private static void DeleteIfExists(string fileBasePath)
        {
            if (File.Exists(fileBasePath))
            {
                File.Delete(fileBasePath);
            }
        }
    }
}
