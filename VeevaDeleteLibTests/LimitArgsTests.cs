//-----------------------------------------------------------------------
// <copyright file="LimitArgsTest.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestUtility.Api;
using VeevaDeleteApi;

namespace VeevaDeleteLib.Tests
{
    /// <summary>
    /// This is a test class for LimitArgsTest and is intended
    /// to contain all LimitArgsTest Unit Tests
    /// </summary>
    [TestClass]
    public class LimitArgsTest
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
        /// A test for LimitArgs Constructor
        /// </summary>
        [TestMethod()]
        public void LimitArgsConstructorTest()
        {
            long burstLimit = 100;
            long dailyLimit = 1000;
            LimitEventArgs target = new LimitEventArgs(burstLimit, dailyLimit);
            Assert.AreEqual(burstLimit, target.BurstLimit);
            Assert.AreEqual(dailyLimit, target.DailyLimit);
        }
    }
}
