//-----------------------------------------------------------------------
// <copyright file="ItemWalkHelperTest.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestUtility.Api;
using RestUtility.VeevaVaultWeb;
using RestUtility.VeevaVaultXml;
using TextTable.Reader;
using RestUtility.Tests;
using VeevaDeleteApi;

namespace VeevaDeleteLib.Tests
{
    /// <summary>
    /// This is a test class for ItemWalkHelperTest and is intended
    /// to contain all ItemWalkHelperTest Unit Tests
    /// </summary>
    [TestClass]
    public class ItemWalkHelperTest
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
        /// Tests for GetItemReferencesFromRows using version in table
        /// </summary>
        [DeploymentItem("VeevaDeleteLib.Tests\\goodref.csv"), DataSource(
                "Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\goodref.csv",
                "goodref#csv",
                DataAccessMethod.Sequential), TestMethod]
        public void GetItemReferencesFromRowsVersionTableTest()
        {
            this.GetItemReferencesFromRowsVersionTest(true);
        }

        /// <summary>
        /// Tests for GetItemReferencesFromRows using version in spec
        /// </summary>
        [DeploymentItem("VeevaDeleteLib.Tests\\goodref.csv"), DataSource(
                "Microsoft.VisualStudio.TestTools.DataSource.CSV",
                "|DataDirectory|\\goodref.csv",
                "goodref#csv",
                DataAccessMethod.Sequential), TestMethod]
        public void GetItemReferencesFromRowsVersionOverrideTest()
        {
            this.GetItemReferencesFromRowsVersionTest(false);
        }
        /// <summary>
        /// run test for current data source and, filling out version either per item or globally
        /// </summary>
        /// <param name="itemVersion">indicate if version should be filled out per item or globally</param>
        private void GetItemReferencesFromRowsVersionTest(bool itemVersion)
        {
            string[] ids = TestContext.DataRow[0].ToString().Split(' ');
            bool withVersion = TestContext.DataRow[1].ToString() == "y";
            string majorVersion = TestContext.DataRow[2].ToString();
            string minorVersion = TestContext.DataRow[3].ToString();
            var itemType = new DeletionRequest.ItemType
            {
                WithVersion = withVersion,
                MajorVersion = majorVersion,
                MinorVersion = minorVersion
            };
            List<ItemReference> expected = ids.Select(
                id =>
                new ItemReference
                {
                    Id = id,
                    WithVersion = itemType.WithVersion,
                    MajorVersion = itemType.MajorVersion,
                    MinorVersion = itemType.MinorVersion
                }).ToList();
            expected.TrimExcess();
            IDataReader reader = new TestReader(expected, expected[0].WithVersion);
            List<ItemReference> actual = new List<ItemReference>();
            foreach (ItemReference item in new ItemWalkHelper(reader, "test", itemType))
            {
                actual.Add(item);
            }
            CollectionAssert.AreEquivalent(expected, actual);
        }

        ////now treated as non-fatal errors
#if false
        /// <summary>
        /// Tests for GetItemReferencesFromRows with blank version in table
        /// </summary>
        [DeploymentItem("VeevaDeleteLib.Tests\\badref.csv"), DataSource(
            "Microsoft.VisualStudio.TestTools.DataSource.CSV",
            "|DataDirectory|\\badref.csv", "badref#csv",
            DataAccessMethod.Sequential), TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetItemReferencesFromRowsVersionTableMissingTest()
        {
            this.GetItemReferencesFromRowsVersionTest(true);
        }

        /// <summary>
        /// Tests for GetItemReferencesFromRows with no version in table and missing in spec
        /// </summary>
        [DeploymentItem("VeevaDeleteLib.Tests\\badref.csv"), DataSource(
            "Microsoft.VisualStudio.TestTools.DataSource.CSV",
            "|DataDirectory|\\badref.csv", "badref#csv",
            DataAccessMethod.Sequential), TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetItemReferencesFromRowsVersionOverrideMissingTest()
        {
            this.GetItemReferencesFromRowsVersionTest(false);
        }
#endif
        /// <summary>
        /// A test for GetItemReferences for CSV files
        /// </summary>
        [TestMethod()]
        [DeploymentItem("Data\\OneId.csv")]
        public void GetItemReferencesCsvTest()
        {
            this.GetItemReferencesFileTypeVersionTest(
                Constants.SourceType.Csv,
                "OneId.csv",
                "10",
                true);
        }

        /// <summary>
        /// A test for GetItemReferences for CSV files for one version but only major version number
        /// </summary>
        [TestMethod()]
        [DeploymentItem("Data\\OneIdNoMinorVersion.csv")]
        public void GetItemReferencesCsvNoMinorVersionTest()
        {
            this.GetItemReferencesFileTypeVersionTest(
                Constants.SourceType.Csv,
                "OneIdNoMinorVersion.csv",
                "11",
                false);
        }

        /// <summary>
        /// A test for GetItemReferences for CSV files for one version with missing minor version number
        /// </summary>
        [TestMethod()]
        [DeploymentItem("Data\\OneIdNoMinorVersion.csv")]
        [ExpectedException(typeof(ItemSourceException))]
        public void GetItemReferencesCsvMissingMinorBinderVersionTest()
        {
            this.GetItemTypeReferencesFileTypeTest(
                Constants.SourceType.Csv,
                "OneIdNoMinorVersion.csv",
                "11",
                new DeletionRequest.ItemType
                {
                    Category = DeletionRequest.ItemCategory.Binder,
                    WithVersion = true
                });
        }

        /// <summary>
        /// A test for GetItemReferences for CSV files all versions (with no version)
        /// </summary>
        [TestMethod()]
        [DeploymentItem("Data\\OneIdNoVersion.csv")]
        public void GetItemReferencesCsvNoVersionTest()
        {
            this.GetItemReferencesFileTypeVersionTest(
                Constants.SourceType.Csv,
                "OneIdNoVersion.csv",
                "12",
                false);
        }

        /// <summary>
        /// A test for GetItemReferences for CSV files for one version but no version numbers
        /// </summary>
        [TestMethod()]
        [DeploymentItem("Data\\OneIdNoVersion.csv")]
        [ExpectedException(typeof(ItemSourceException))]
        public void GetItemReferencesCsvMissingDocumentVersionTest()
        {
            this.GetItemTypeReferencesFileTypeTest(
                Constants.SourceType.Csv,
                "OneIdNoVersion.csv",
                "12",
                new DeletionRequest.ItemType
                {
                    Category = DeletionRequest.ItemCategory.Document,
                    WithVersion = true
                });
        }

        /// <summary>
        /// A test for GetItemReferences for Excel files
        /// </summary>
        [TestMethod()]
        [DeploymentItem("Data\\OneId.xlsx")]
        public void GetItemReferencesExcelTest()
        {
            this.GetItemReferencesFileTypeVersionTest(
                Constants.SourceType.Excel,
                "OneId.xlsx",
                "20",
                true);
        }

        /// <summary>
        /// A test for GetItemReferences for Excel files for all versions
        /// </summary>
        [TestMethod()]
        [DeploymentItem("Data\\OneIdNoVersion.xlsx")]
        public void GetItemReferencesExcelNoVersionTest()
        {
            this.GetItemReferencesFileTypeVersionTest(
                Constants.SourceType.Excel,
                "OneIdNoVersion.xlsx",
                "22",
                false);
        }

        /// <summary>
        /// A test for GetItemReferences for Access files
        /// </summary>
        [TestMethod()]
        [DeploymentItem("Data\\OneId.accdb")]
        public void GetItemReferencesAccessTest()
        {
            this.GetItemReferencesFileTypeVersionTest(
                Constants.SourceType.Access,
                "OneId.accdb",
                "30",
                true);
        }

        /// <summary>
        /// A test for GetItemReferences for Access files for msising versions
        /// </summary>
        [TestMethod()]
        [DeploymentItem("Data\\OneIdNoVersion.accdb")]
        [ExpectedException(typeof(ItemSourceException))]
        public void GetItemReferencesAccessMissingVersionTest()
        {
            this.GetItemReferencesFileTypeVersionTest(
                Constants.SourceType.Access,
                "OneIdNoVersion.accdb",
                "32",
                true);
        }

        /// <summary>
        /// A test for GetItemReferences for Access files for all versions
        /// </summary>
        [TestMethod()]
        [DeploymentItem("Data\\OneIdNoVersion.accdb")]
        public void GetItemReferencesAccessNoVersionTest()
        {
            this.GetItemReferencesFileTypeVersionTest(
                Constants.SourceType.Access,
                "OneIdNoVersion.accdb",
                "32",
                false);
        }


        /// <summary>
        /// test extract for file with version details
        /// </summary>
        /// <param name="sourceType">the type of file to extract from</param>
        /// <param name="testFileName">the name of the file to extract from</param>
        /// <param name="haveVersion">does file file contain version info</param>
        /// <param name="objectName">the object name to use (default "a") </param>
        /// <param name="id">the item id to use (default "10")</param>
        private void GetItemReferencesFileTypeVersionTest(
            Constants.SourceType sourceType,
            string testFileName,
            string id,
            bool haveVersion,
            string objectName = "a")
        {
            this.GetItemTypeReferencesFileTypeTest(
                sourceType,
                testFileName,
                id,
                new DeletionRequest.ItemType
                {
                    Category = DeletionRequest.ItemCategory.Object,
                    ObjectName = objectName
                });
            this.GetVersionedItemReferencesFileTypeTest(
                sourceType,
                testFileName,
                id,
                haveVersion,
                DeletionRequest.ItemCategory.Binder);
            this.GetVersionedItemReferencesFileTypeTest(
                sourceType,
                testFileName,
                id,
                haveVersion,
                DeletionRequest.ItemCategory.Document);
        }

        /// <summary>
        /// Test extract from file for references requiring version details
        /// </summary>
        /// <param name="sourceType">the type of file to extract from</param>
        /// <param name="testFileName">the name of the file to extract from</param>
        /// <param name="itemCategory">binder or document</param>
        /// <param name="haveVersion">does file file contain version info</param>
        /// <param name="id">the item id to use (default "10")</param>
        private void GetVersionedItemReferencesFileTypeTest(
            Constants.SourceType sourceType,
            string testFileName,
            string id,
            bool haveVersion,
            DeletionRequest.ItemCategory itemCategory)
        {
            this.GetItemTypeReferencesFileTypeTest(
                sourceType,
                testFileName,
                id,
                new DeletionRequest.ItemType
                {
                    Category = itemCategory,
                    WithVersion = true,
                    MajorVersion = "2",
                    MinorVersion = "3"
                });
            // have version => test w & w/o version.
            // no version => test w/o version.
            bool withVersion = haveVersion;
            do
            {
                this.GetItemTypeReferencesFileTypeTest(
                    sourceType,
                    testFileName,
                    id,
                    new DeletionRequest.ItemType
                    {
                        Category = itemCategory,
                        WithVersion = withVersion,
                    });
                withVersion = !withVersion;
            } while (!withVersion);
        }

        /// <summary>
        /// Test if references extracted from file are correct
        /// </summary>
        /// <param name="sourceType">the type of file to extract from</param>
        /// <param name="testFileName">the name of the file to extract from</param>
        /// <param name="id">the item id to use</param>
        /// <param name="itemType">the type of item to delete</param>
        private void GetItemTypeReferencesFileTypeTest(
            Constants.SourceType sourceType,
            string testFileName,
            string id,
            DeletionRequest.ItemType itemType)
        {
            IRestConnection target = new MockWebConnection();
            string testfileBasePath = Path.Combine(TestContext.TestDir, "out", testFileName);
            DeletionRequest deleteParameters = new DeletionRequest
            {
                RelationType = sourceType,
                RelationFile = testfileBasePath,
                Type = itemType
            };
            bool fileVersion = itemType.WithVersion && string.IsNullOrEmpty(itemType.MajorVersion);
            List<ItemReference> expected = new List<ItemReference>
            {
                new ItemReference
                {
                    Id = id,
                    WithVersion = itemType.WithVersion,
                    MajorVersion = fileVersion ? "2": itemType.MajorVersion,
                    MinorVersion = fileVersion ? "3": itemType.MinorVersion
                }
            };
            List<ItemReference> actual = new List<ItemReference>();
            foreach(ItemReference item in deleteParameters)
            {
                actual.Add(item);
            }
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// mimic the expected table for testing table retrieval
        /// </summary>
        /// <param name="ids">the item ids to be included in the table</param>
        /// <param name="majorVersion">the major version number for each item, if non-empty</param>
        /// <param name="minorVersion">the minor version number for each item, if non-empty</param>
        /// <returns>the expected table</returns>
        private DataTable GetExpectedRows(IEnumerable<string> ids, string majorVersion, string minorVersion)
        {
            DataTable data = new DataTable();
            data.Columns.Add(VaultGrid.Column.IdFieldName, typeof(string));
            bool version = !string.IsNullOrEmpty(majorVersion) || !string.IsNullOrEmpty(minorVersion);
            if (version)
            {
                data.Columns.Add(VaultGrid.Column.MajorVersionFieldName, typeof(string));
                data.Columns.Add(VaultGrid.Column.MinorVersionFieldName, typeof(string));
            }

            foreach (string id in ids)
            {
                DataRow row = data.NewRow();
                row[0] = id;
                if (version)
                {
                    row[1] = majorVersion.ToString();
                    row[2] = minorVersion.ToString();
                }

                data.Rows.Add(row);
            }

            return data;
        }

        class TestReader : TextDataReader
        {
            private readonly IEnumerator<ItemReference> itemEnumerator;
            private readonly bool withVersion;
            public TestReader(List<ItemReference> items, bool withVersion)
            {
                this.itemEnumerator = items.GetEnumerator();
                this.withVersion = withVersion;
                this.SetFieldNames(this.withVersion ?
                    new string[] { VaultSymbols.IdFieldName } :
                    new string[] { VaultSymbols.IdFieldName, VaultSymbols.MajorVersionFieldName, VaultSymbols.MinorVersionFieldName }
                    );
            }
            public override bool IsClosed { get; }
            public override void Close() { }
            protected override void Dispose(bool disposing) { }
            protected override string[] ReadFields() {
                string[] result = null;
                if(this.itemEnumerator.MoveNext())
                {
                    ItemReference item = this.itemEnumerator.Current;
                    result = this.withVersion ?
                        new string[] { item.Id } :
                        new string[] { item.Id, item.MajorVersion, item.MinorVersion };
                }
                return result;
            }
        }

    }
}