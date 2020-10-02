//-----------------------------------------------------------------------
// <copyright file="DeleteHelperTest.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestUtility.Api;
using RestUtility.VeevaVaultXml;
using TextTable.Reader;
using RestUtility.Tests;
using VeevaDeleteApi;

namespace VeevaDeleteLib.Tests
{
    using IntPair = Tuple<int, int>;

    /// <summary>
    /// This is a test class for DeleteHelperTest and is intended
    /// to contain all DeleteHelperTest Unit Tests
    /// </summary>
    [TestClass]
    public class DeleteHelperTest
    {
        /// <summary>
        /// provides information and functionality to the current test run
        /// </summary>
        private TestContext testContextInstance;
        private BatchItemDeleteStatus lastBatchItemDeleteStatus;
        private int[]statusCounts;

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
        ////[ClassInitialize()]
        ////public static void MyClassInitialize(TestContext testContext)
        ////{
        ////}

        ////Use ClassCleanup to run code after all tests in a class have run
        ////[ClassCleanup()]
        ////public static void MyClassCleanup()
        ////{
        ////}

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


        [TestInitialize()]
        public void InitaliseTest()
        {
            this.lastBatchItemDeleteStatus = null;
            this.statusCounts = new int[2];
        }

        /// <summary>
        /// A test for DeleteSelection
        /// </summary>
        [TestMethod()]
        public void DeleteSelectTest()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            MockWebConnection conn = new MockWebConnection(MockWebConnection.MockQueryDeleteTransfers);
            DeletionRequest deletionRequest = new DeletionRequest
            {
                RelationConnection = conn,
                RelationType = Constants.SourceType.Query,
                SelectionFilter = string.Empty,
                Type = new DeletionRequest.ItemType
                {
                    Category = DeletionRequest.ItemCategory.Document
                }
            };

            int[] expected = new int[] { 0, MockWebConnection.MockCount };

            deletionRequest.ExecuteDeletionRequest(
                conn,
                BatchItemDeleteProcessed,
                tokenSource.Token);
            
            CollectionAssert.AreEqual(expected, this.statusCounts);
        }

        /// <summary>
        /// A test for DeleteSet individual items
        /// </summary>
        [TestMethod()]
        public void DeleteSetItemsTest()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            DeletionRequest deletionRequest = new DeletionRequest
            {
                Type = new DeletionRequest.ItemType
                {
                    Category = DeletionRequest.ItemCategory.Binder
                }
            };
            List<ItemReference> items = new List<ItemReference>
            {
                new ItemReference { Id = "1" },
                new ItemReference { Id = "2" }
            };
            bool[] dialogResponseSteps = { false, true };
            IEnumerable<MockWebConnection.Transfer> transfers =
                items.Select(item => new MockWebConnection.Transfer { Request = "DELETE objects/binders/" + item.Id });
            IRestConnection conn = new MockWebConnection(transfers);
            List<ItemReference> processedItems = new List<ItemReference>();
            List<bool> processedStatuses = new List<bool>();
            int errorCount = 0;
            deletionRequest.DeleteSet(
                conn,
                BatchItemDeleteProcessed,
                items.GetEnumerator(),
                (ItemReference item, bool status, IEnumerable<VaultXmlHelper.VaultError> errors) =>
                {
                    processedItems.Add(item);
                    processedStatuses.Add(status);
                    if (errors != null)
                    {
                        ++errorCount;
                    };
                },
                tokenSource.Token);
            CollectionAssert.AreEquivalent(items, processedItems);
            Assert.AreEqual(2, processedStatuses.Count);
            Assert.IsTrue(processedStatuses.All(status => status));
            Assert.AreEqual(0, errorCount);
        }

        /// <summary>
        /// A test for DeleteSet batch with no version
        /// </summary>
        [TestMethod()]
        public void DeleteSetBatchNoVersionTest()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            DeletionRequest deletionRequest = new DeletionRequest
            {
                Type = new DeletionRequest.ItemType
                {
                    Category = DeletionRequest.ItemCategory.Document
                },
                Batched = true
            };
            string expectedContent =
                @"id
                10
                11
                ".Replace(" ", string.Empty);
            List<ItemReference> items = new List<ItemReference>
            {
                new ItemReference { Id = "10" },
                new ItemReference { Id = "11" }
            };
            MockWebConnection conn = new MockWebConnection(
                 new MockWebConnection.Transfer
                 {
                     Request = MockWebConnection.MockDeleteBatch,
                     Response = GetSuccessResponse(items),
                     Content = expectedContent
                 });
            List<ItemReference> processedItems = new List<ItemReference>();
            List<bool> processedStatuses = new List<bool>();
            int errorCount = 0;
            deletionRequest.DeleteSet(
                conn,
                BatchItemDeleteProcessed,
                items.GetEnumerator(),
                (ItemReference item, bool status, IEnumerable<VaultXmlHelper.VaultError> errors) =>
                {
                    processedItems.Add(item);
                    processedStatuses.Add(status);
                    if (errors != null)
                    {
                        ++errorCount;
                    }
                },
                tokenSource.Token);
            CollectionAssert.AreEquivalent(items, processedItems);
            Assert.AreEqual(2, processedStatuses.Count);
            Assert.IsTrue(processedStatuses.All(status => status));
            Assert.AreEqual(0, errorCount);
        }

        /// <summary>
        /// A test for DeleteSet batch with version
        /// </summary>
        [TestMethod()]
        public void DeleteSetBatchVersionTest()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            DeletionRequest deletionRequest = new DeletionRequest
            {
                Type = new DeletionRequest.ItemType
                {
                    Category = DeletionRequest.ItemCategory.Document,
                    WithVersion = true
                },
                Batched = true
            };
            string expectedContent =
                @"id,major_version_number__v,minor_version_number__v
                10,1,2
                11,3,1
                ".Replace(" ", string.Empty);
            List<ItemReference> items = new List<ItemReference>
            {
                new ItemReference { Id = "10", MajorVersion = "1", MinorVersion = "2" },
                new ItemReference { Id = "11", MajorVersion = "3", MinorVersion = "1" }
            };
            MockWebConnection conn = new MockWebConnection(
                new MockWebConnection.Transfer
                {
                    Request = "DELETE objects/documents/versions/batch",
                    Response = GetSuccessResponse(items),
                    Content = expectedContent
                });
            List<ItemReference> processedItems = new List<ItemReference>();
            List<bool> processedStatuses = new List<bool>();
            int errorCount = 0;
            deletionRequest.DeleteSet(
                conn,
                BatchItemDeleteProcessed,
                items.GetEnumerator(),
                (ItemReference item, bool status, IEnumerable<VaultXmlHelper.VaultError> errors) =>
                {
                    processedItems.Add(item);
                    processedStatuses.Add(status);
                    if (errors != null)
                    {
                        ++errorCount;
                    }
                },
                tokenSource.Token
                );
            CollectionAssert.AreEquivalent(items, processedItems);
            Assert.AreEqual(2, processedStatuses.Count);
            Assert.IsTrue(processedStatuses.All(status => status));
            Assert.AreEqual(0, errorCount);
        }


        /// <summary>
        /// A test for DeleteSet batch with batch size 1 
        /// </summary>
        [TestMethod()]
        public void DeleteSetBatchOnePerBatchTest()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            DeletionRequest deletionRequest = new DeletionRequest
            {
                Type = new DeletionRequest.ItemType
                {
                    Category = DeletionRequest.ItemCategory.Document
                },
                Batched = true,
                BatchSize = 1
            };
            string expectedContent =
                @"id
                10
                11
                ".Replace(" ", string.Empty);
            List<ItemReference> items = new List<ItemReference>
            {
                new ItemReference { Id = "10" },
                new ItemReference { Id = "11" }
            };
            IEnumerable<MockWebConnection.Transfer> transfers =
                from item in items
                 select
                     new MockWebConnection.Transfer
                     {
                         Request = MockWebConnection.MockDeleteBatch,
                         Response = GetSuccessResponse(new ItemReference[] { item }),
                         Content = $@"id
                                    {item.Id}
                                    ".Replace(" ", string.Empty)
                     };
            MockWebConnection conn = new MockWebConnection(transfers);
            List<ItemReference> processedItems = new List<ItemReference>();
            List<bool> processedStatuses = new List<bool>();
            int errorCount = 0;
            deletionRequest.DeleteSet(
                conn,
                BatchItemDeleteProcessed,
                items.GetEnumerator(),
                (ItemReference item, bool status, IEnumerable<VaultXmlHelper.VaultError> errors) =>
                {
                    processedItems.Add(item);
                    processedStatuses.Add(status);
                    if (errors != null)
                    {
                        ++errorCount;
                    }
                },
                tokenSource.Token);
            CollectionAssert.AreEquivalent(items, processedItems);
            Assert.AreEqual(2, processedStatuses.Count);
            Assert.IsTrue(processedStatuses.All(status => status));
            Assert.AreEqual(0, errorCount);
        }

        /// <summary>
        /// A test for DeleteSet batch with zero batch size
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void DeleteSetBatchNonePerBatchTest()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            DeletionRequest deletionRequest = new DeletionRequest
            {
                Type = new DeletionRequest.ItemType
                {
                    Category = DeletionRequest.ItemCategory.Document
                },
                Batched = true,
                BatchSize = 0
            };
            MockWebConnection conn = new MockWebConnection();
            List<ItemReference> processedItems = new List<ItemReference>();
            List<bool> processedStatuses = new List<bool>();
            deletionRequest.DeleteSet(
                conn,
                BatchItemDeleteProcessed,
                new List<ItemReference>().GetEnumerator(),
                (ItemReference item, bool status, IEnumerable<VaultXmlHelper.VaultError> errors) => { },
                tokenSource.Token);
        }

        /// <summary>
        /// A test for DeleteSet batch with excesss batch size
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void DeleteSetBatchTooManyPerBatchTest()
        {
            MockWebConnection conn = new MockWebConnection();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            DeletionRequest deletionRequest = new DeletionRequest
            {
                Type = new DeletionRequest.ItemType
                {
                    Category = DeletionRequest.ItemCategory.Document
                },
                Batched = true,
                BatchSize = conn.MaxBatchSize+1
            };
            List<ItemReference> processedItems = new List<ItemReference>();
            List<bool> processedStatuses = new List<bool>();
            deletionRequest.DeleteSet(
                conn,
                BatchItemDeleteProcessed,
                new List<ItemReference>().GetEnumerator(),
                (ItemReference item, bool status, IEnumerable<VaultXmlHelper.VaultError> errors) => { },
                tokenSource.Token);
        }


        /// <summary>
        /// A test for DeleteSet batch with multiple batches
        /// </summary>
        [TestMethod()]
        public void DeleteSetSplitBatch()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            List<ItemReference> items = new List<ItemReference>
            {
                new ItemReference { Id = "10" },
                new ItemReference { Id = "11" },
                new ItemReference { Id = "12" },
                new ItemReference { Id = "13" },
                new ItemReference { Id = "14" },
                new ItemReference { Id = "15" }
            };
            DeletionRequest deletionRequest = new DeletionRequest
            {
                Type = new DeletionRequest.ItemType
                {
                    Category = DeletionRequest.ItemCategory.Document
                },
                Batched = true
            };
            string expectedContent1 =
                @"id
                10
                11
                12
                ".Replace(" ", string.Empty);
            string expectedContent2 =
                @"id
                13
                14
                15
                ".Replace(" ", string.Empty);
            MockWebConnection conn = new MockWebConnection(
                new MockWebConnection.Transfer[]
                {
                    new MockWebConnection.Transfer
                    {
                        Request = MockWebConnection.MockDeleteBatch,
                        Response = GetSuccessResponse(items.Take(3)),
                        Content = expectedContent1
                    },
                    new MockWebConnection.Transfer
                    {
                        Request = MockWebConnection.MockDeleteBatch,
                        Response = GetSuccessResponse(items.Skip(3)),
                        Content = expectedContent2
                   }
                });
            List<ItemReference> processedItems = new List<ItemReference>();
            List<bool> processedStatuses = new List<bool>();
            int errorCount = 0;
            deletionRequest.DeleteSet(
                conn,
                BatchItemDeleteProcessed,
                items.GetEnumerator(),
               (ItemReference item, bool status, IEnumerable<VaultXmlHelper.VaultError> errors) =>
               {
                   processedItems.Add(item);
                   processedStatuses.Add(status);
                   if (errors != null)
                   {
                       ++errorCount;
                   }
               },
               tokenSource.Token);
            CollectionAssert.AreEquivalent(items, processedItems);
            Assert.AreEqual(6, processedStatuses.Count);
            Assert.IsTrue(processedStatuses.All(status => status));
            Assert.AreEqual(0, errorCount);
        }

        /// <summary>
        /// A test for DeleteItem Success
        /// </summary>
        [DeploymentItem("VeevaDeleteLib.Tests\\goodurn.csv"),
            DataSource(
                "Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\goodurn.csv", "goodurn#csv",
                DataAccessMethod.Sequential),
            TestMethod]
        public void DeleteItemsGoodTest()
        {
            bool batched = !string.IsNullOrEmpty(TestContext.DataRow[4].ToString());
            if (!batched)
            {
                ItemReference item = new ItemReference
                {
                    Id = "1",
                    MajorVersion = TestContext.DataRow[2].ToString(),
                    MinorVersion = TestContext.DataRow[3].ToString()
                };
                DeletionRequest delestionRequest = new DeletionRequest
                {
                    Type = new DeletionRequest.ItemType
                    {
                        Category = (DeletionRequest.ItemCategory)Enum.Parse(typeof(DeletionRequest.ItemCategory), TestContext.DataRow[0].ToString()),
                        ObjectName = TestContext.DataRow[1].ToString(),
                        WithVersion = !(string.IsNullOrEmpty(item.MajorVersion) && string.IsNullOrEmpty(item.MinorVersion))
                    },
                    Cascade = !string.IsNullOrEmpty(TestContext.DataRow[5].ToString())
                };
                string urn = string.Format(TestContext.DataRow[6].ToString(), item.Id);
                this.DeleteOneItemTest(delestionRequest, item, urn, true, false);
            }
        }

        /// <summary>
        /// A test for DeleteItem Success
        /// </summary>
        [DeploymentItem("VeevaDeleteLib.Tests\\goodurn.csv"),
            DataSource(
                "Microsoft.VisualStudio.TestTools.DataSource.CSV", "|DataDirectory|\\goodurn.csv", "goodurn#csv",
                DataAccessMethod.Sequential),
            TestMethod]
        public void DeleteItemsGoodBusyTest()
        {
            bool batched = !string.IsNullOrEmpty(TestContext.DataRow[4].ToString());
            if (!batched)
            {
                ItemReference item = new ItemReference
                {
                    Id = "1",
                    MajorVersion = TestContext.DataRow[2].ToString(),
                    MinorVersion = TestContext.DataRow[3].ToString()
                };
                DeletionRequest deletionRequest = new DeletionRequest
                {
                    Type = new DeletionRequest.ItemType
                    {
                        Category = (DeletionRequest.ItemCategory)Enum.Parse(typeof(DeletionRequest.ItemCategory), TestContext.DataRow[0].ToString()),
                        ObjectName = TestContext.DataRow[1].ToString(),
                        WithVersion = !(string.IsNullOrEmpty(item.MajorVersion) && string.IsNullOrEmpty(item.MinorVersion))
                    },
                    Cascade = !string.IsNullOrEmpty(TestContext.DataRow[5].ToString())
                };
                string urn = string.Format(TestContext.DataRow[6].ToString(), item.Id);
                this.DeleteOneItemTest(deletionRequest, item, urn, true, true);
            }
        }

        /// <summary>
        /// A test for DeleteItem Fail
        /// </summary>
        [TestMethod()]
        public void DeleteItemFailTest()
        {
            ItemReference item = new ItemReference
            {
                Id = "1"
            };
            DeletionRequest deletionRequest = new DeletionRequest
            {
                Type = new DeletionRequest.ItemType
                {
                    Category = DeletionRequest.ItemCategory.Document
                }
            };
            this.DeleteOneItemTest(deletionRequest, item, "objects/documents/1", false, false);
        }

        /// <summary>
        /// A test for DeleteItem Fail
        /// </summary>
        [TestMethod()]
        public void DeleteItemBusyTest()
        {
            ItemReference item = new ItemReference
            {
                Id = "1"
            };
            DeletionRequest deletionRequest = new DeletionRequest
            {
                Type = new DeletionRequest.ItemType
                {
                    Category = DeletionRequest.ItemCategory.Document
                }
            };
            this.DeleteOneItemTest(deletionRequest, item, "objects/documents/1", true, true);
        }

        /// <summary>
        /// A test for GetCount
        /// </summary>
        [TestMethod()]
        public void GetCountTest()
        {
            MockWebConnection connection = new MockWebConnection(
                new MockWebConnection.Transfer[]
                {
                    new MockWebConnection.Transfer
                    {
                        Response = "<x><responseStatus>SUCCESS</responseStatus><responseDetails><total>3</total></responseDetails></x>"
                    }
                });
            string query = "-query-";
            int expected = 3;
            int actual;
            actual = connection.GetCount(query);
            Assert.AreEqual(expected, actual);
        }

#if false
        /// <summary>
        /// A test for GetDataRows
        /// </summary>
        [TestMethod()]
        public void GetDataRowsTest()
        {

            // cannot read csv?
            // string selectFile = "
            string selectFile = "OneID.xlsx";

            // take the file from the project root
            // string selectfileBasePath =
            //    Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, selectFile);
            string selectfileBasePath =
                Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, selectFile);


            IEnumerable<DataRow> actual;
            actual = DeleteHelper.GetDataTableFromExcelFile(selectfileBasePath).AsEnumerable();

            string[] expected_ids = new string[] { "1" };
            string[] actual_ids = actual.Select(row => row["id"].ToString()).ToArray();
            CollectionAssert.AreEquivalent(expected_ids, actual_ids);
        }

        private IEnumerable<DataRow> GetExpectedRows()
        {
            DataTable datatable = new DataTable { TableName = "OneID" };
            DataColumn id_column = new DataColumn { ColumnName = "id", DataType = typeof(Decimal) };
            datatable.Columns.Add(id_column);
            DataRow datarow = datatable.NewRow();
            datarow["id"] = 1.0;
            datatable.Rows.Add(datarow);
            return datatable.AsEnumerable();
        }

#endif
        /// <summary>
        /// A test for GetNumberOfRecsFromFile for a CSV file
        /// </summary>
        [TestMethod()]
        [DeploymentItem("Data\\OneId.csv")]
        public void GetNumberOfRecsFromFileCsvTest()
        {
            Constants.SourceType selectType = Constants.SourceType.Csv;
            string selectFile = "OneId.csv";
            int expected = 1;
            int actual;
            var itemType = new DeletionRequest.ItemType { WithVersion = true };
            actual = DeleteHelper.GetNumberOfRecsFromFile(selectType, itemType, selectFile);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for GetNumberOfRecsFromFile for an Excel file
        /// </summary>
        [TestMethod()]
        [DeploymentItem("Data\\OneId.xlsx")]
        public void GetNumberOfRecsFromFileExcelTest()
        {
            Constants.SourceType selectType = Constants.SourceType.Excel;
            string selectFile = "OneId.xlsx";
            int expected = 1;
            var itemType = new DeletionRequest.ItemType { WithVersion = true };
            int actual;
            actual = DeleteHelper.GetNumberOfRecsFromFile(selectType, itemType, selectFile);
            Assert.AreEqual(expected, actual);
        }

#if false        
        /// <summary>
        /// A test for UpdateLogs
        /// </summary>
        [TestMethod()]
        public void UpdateLogsTest()
        {
            string fileBasePath = Path.Combine(TestContext.TestDir, "testWriteErr");
            string fileFullPath = fileBasePath + ".log";
            string message = "Test Log Error";
            Exception ex = new Exception("Test Log Exception");

            try
            {
                File.Delete(fileFullPath);
            }
            catch (Exception)
            {
                // assume the file isn't there
            }

            Logger listener = new Logger(fileBasePath);

            // test for non-fatal and fatal
            DeleteHelper.UpdateLogs(message, ex, false);
            DeleteHelper.UpdateLogs(message, ex, true);

            listener.Dispose();
            listener = null;
            string[] loggedLines = File.ReadAllLines(fileFullPath);
            string[] expectedLines = new string[]
            {
                "****ERROR****", message, string.Empty, ex.ToString(), string.Empty, // blank line after message and at end
                "****FATAL ERROR****", message, string.Empty, ex.ToString(), string.Empty // blank line after message and at end
            };

            // remove time stamp before messsages
            loggedLines[1] = loggedLines[1].Substring(loggedLines[1].Length - message.Length);
            loggedLines[6] = loggedLines[6].Substring(loggedLines[6].Length - message.Length);
            CollectionAssert.AreEqual(expectedLines, loggedLines);
        }
#endif 
        /// <summary>
        /// A test for TestSelect with no Query
        /// </summary>
        [TestMethod()]
        public void TestSelectMockTest()
        {
            IRestConnection connUtil = new MockWebConnection();
            ConnectionParameters requestParams = MockWebConnection.MockParameters;
            DeletionRequest deletionRequest = new DeletionRequest
            {
                RelationType = Constants.SourceType.Query,
                RelationConnection = connUtil,
                Type = new DeletionRequest.ItemType { Category = DeletionRequest.ItemCategory.Document }
            };
            int expected = MockWebConnection.MockCount;
            int actual;
            connUtil.OpenConnection(requestParams);
            actual = deletionRequest.GetSelectCount();
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        /// A test for TestSelect with dummy query
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(VaultApiException))]
        public void TestSelectDummyQueryTest()
        {
            ConnectionParameters requestParams = new ConnectionParameters
            {
                Url = "http://localhost",
                SubService = "v1",
                Username = string.Empty,
                Password = string.Empty,
                ClientId = string.Empty
            };
            IRestConnection connUtil = new MockThrottledConnection
            {
                ReturnContent = new string[]
                {
                    "<x><responseStatus>SUCCESS</responseStatus><sessionId>z</sessionId></x>",
                    "<x><responseStatus>FAILED</responseStatus><errors><error><type/></error></errors></x>"
                }
            };
            DeletionRequest deletionRequest = new DeletionRequest {
                RelationType = Constants.SourceType.Query,
                RelationConnection = connUtil,
                Type = new DeletionRequest.ItemType { }
            };
            connUtil.OpenConnection(requestParams);
            int actual = deletionRequest.GetSelectCount();
            Assert.IsInstanceOfType(connUtil, typeof(MockThrottledConnection));
        }


        /// <summary>
        /// generic test to verify deletion URN calculation
        /// </summary>
        /// <param name="deletionRequest">the deletion context</param>
        /// <param name="itemReference">the individual item reference to use</param>
        /// <param name="expectedUrn">the expected urn</param>
        /// <param name="expectedStatus">indicates whether to test for success or failure</param>
        private void DeleteOneItemTest(DeletionRequest deletionRequest, ItemReference itemReference, string expectedUrn, bool expectedStatus, bool simulateBusy)
        {
            List<MockWebConnection.Transfer> dialog = new List<MockWebConnection.Transfer>();
            if (simulateBusy)
            {
                dialog.Add(
                    new MockWebConnection.Transfer
                    {
                        Request = (deletionRequest.Cascade ? "POST " : "DELETE ") + expectedUrn,
                        Response = MockWebConnection.MockBusyResponse
                    }
                );
            }
            dialog.Add(
                    new MockWebConnection.Transfer
                    {
                        Request = (deletionRequest.Cascade ? "POST " : "DELETE ") + expectedUrn,
                        Response = expectedStatus ? MockWebConnection.MockSuccessReponse : MockWebConnection.MockFailResponse
                    }
            );

            MockWebConnection conn = new MockWebConnection(dialog);
            ItemReference processedItem = null;
            bool? processedStatus = null;
            int errorCount = 0;
            deletionRequest.DeleteItem(
                conn,
                BatchItemDeleteProcessed,
                itemReference,
                (ItemReference item, bool status, IEnumerable<VaultXmlHelper.VaultError> errors) =>
                {
                    processedItem = item;
                    processedStatus = status;
                    if (errors != null)
                    {
                        ++errorCount;
                    }
                });
            Assert.AreEqual(itemReference, processedItem);
            Assert.AreEqual(expectedStatus, processedStatus);
            Assert.AreEqual(expectedStatus ? 0 : 1, errorCount);
        }

        private void BatchItemDeleteProcessed(BatchItemDeleteStatus status)
        {
            this.lastBatchItemDeleteStatus = status;
            if(status.ItemSuccessStatus.HasValue)
            {
                ++ this.statusCounts[status.ItemSuccessStatus.Value ? 1 : 0];
            }
        }

        /// <summary>
        /// A test for GetCount with total
        /// </summary>
        [TestMethod()]
        public void GetCountTotalTest()
        {
            string vql = MockWebConnection.MockQuery;
            int expected = 0;
            MockWebConnection connection = new MockWebConnection(
                new MockWebConnection.Transfer[]
                {
                    new MockWebConnection.Transfer
                    {
                        Request = MockWebConnection.MockQueryCountRequest,
                        Response = RestUtility.Tests.VaultXmlHelperTest.GetXmlString(TableResponseDetails(new ItemReference[] { }))
                    }
                });
            int actual;
            actual = connection.GetCount(vql);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for CollectItemsFromResponsePage
        /// </summary>          -
        [TestMethod()]
        public void CollectItemsFromResponsePageTest()
        {
            List<ItemReference> expectedItemRefs = new List<ItemReference>
            {
                new ItemReference { Id = "1", MajorVersion = "2", MinorVersion = "3" },
                new ItemReference { Id = "2", MajorVersion = "4", MinorVersion = "5" },
                new ItemReference { Id = "3", MajorVersion = "6", MinorVersion = "7" },
            };

            int expectedCount = expectedItemRefs.Count();
            XElement responseXml = TableResponseDetails(expectedItemRefs);
            string[] fieldNames = null;
            var tableParser= new VaultXmlHelper.VaultTableParser(VaultSymbols.ResponseDataRow);
            List<string[]> items = tableParser.CollectItemsFromResponse(ref fieldNames, new VaultXmlHelper.VaultResponse(AsResponse(responseXml)));
            List<ItemReference> actual = ToItemList(new List<List<string[]>> { new List<string[]> { fieldNames }, items });
            Assert.AreEqual(expectedCount, actual.Count);
            CollectionAssert.AreEquivalent(expectedItemRefs, actual);
        }

        /// <summary>
        /// A test for GetTableFromQuery with one column
        /// </summary>
        [TestMethod()]
        public void GetTableFromQueryNarrowTest()
        {
            List<ItemReference> itemRefs = new List<ItemReference>
            {
                new ItemReference { Id = "1" },
                new ItemReference { Id = "2" },
            };
            MockWebConnection connection = CreateWebDialog(itemRefs);
            List<ItemReference> actual;
            actual = ToItemList(connection.GetQueryPages(MockWebConnection.MockQuery).ToList());
            Assert.AreEqual(itemRefs.Count, actual.Count);
            CollectionAssert.AreEquivalent(itemRefs, actual);
        }

        /// <summary>
        /// A test for GetTableFromQuery with all columns
        /// </summary>
        [TestMethod()]
        public void GetTableFromQueryFullTest()
        {
            List<ItemReference> itemRefs = new List<ItemReference>
            {
                new ItemReference { Id = "1", MajorVersion = "3", MinorVersion = "4" },
                new ItemReference { Id = "2", MajorVersion = "5", MinorVersion = "6" },
            };
            MockWebConnection connection = CreateWebDialog(itemRefs);
            List<List<string[]>> actual;
            actual = connection.GetQueryPages(MockWebConnection.MockQuery).ToList();
            List<ItemReference> actualRefs = ToItemList(actual);
            string[] columns = actual[0][0];
            Assert.AreEqual(3, columns.Length);
            Assert.AreEqual(VaultSymbols.IdFieldName, columns[0]);
            Assert.AreEqual(VaultSymbols.MajorVersionFieldName, columns[1]);
            Assert.AreEqual(VaultSymbols.MinorVersionFieldName, columns[2]);
            CollectionAssert.AreEquivalent(itemRefs, actualRefs);
        }

        /// <summary>
        /// A test for GetTableFromQuery with inconsistent columns
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ItemSourceException))]
        public void GetTableFromQueryFailInconsistentTest()
        {
            List<ItemReference> itemRefs = new List<ItemReference>
            {
                new ItemReference { Id = "1" },
                new ItemReference { Id = "2", MajorVersion = "4", MinorVersion = "5" },
                new ItemReference { Id = "3" },
            };
            MockWebConnection connection = CreateWebDialog(itemRefs);
            connection.GetQueryPages(MockWebConnection.MockQuery).Count();
        }
#if false
        ////now treated as non-fatal errors
        /// <summary>
        /// A test for GetTableFromQuery with missing major version column
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ItemSourceException))]
        public void GetTableFromQueryFailMissingMajorTest()
        {
            List<ItemReference> itemRefs = new List<ItemReference>
            {
                new ItemReference { Id = "1", MajorVersion = "3", MinorVersion = "4" },
                new ItemReference { Id = "2", MinorVersion = "6" },
            };
            MockWebConnection connection = CreateWebDialog(itemRefs);
            connection.GetQueryPages(MockWebConnection.MockQuery).Count();
        }

        /// <summary>
        /// A test for GetTableFromQuery with missing major version column
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ItemSourceException))]
        public void GetTableFromQueryFailMissingMinorTest()
        {
            List<ItemReference> itemRefs = new List<ItemReference>
            {
                new ItemReference { Id = "1", MajorVersion = "3", MinorVersion = "4" },
                new ItemReference { Id = "2", MajorVersion = "5" },
            };
            MockWebConnection connection = CreateWebDialog(itemRefs);
            connection.GetQueryPages(MockWebConnection.MockQuery).Count();
        }
#endif
        /// <summary>
        /// Create Dialog for series of pages containing the listed items, one item per page
        /// </summary>
        /// <param name="itemRefs">the list of items to be returns</param>
        /// <returns>a set of exchanges corresponding to a series of requests/responses for the items
        /// with a page size of 1</returns>
        private static MockWebConnection CreateWebDialog(IEnumerable<ItemReference> itemRefs)
        {
            int total = itemRefs.Count();
            string totalResponse = total.ToString();
            int nextPage = 1;
            List<MockWebConnection.Transfer> transfers = new List<MockWebConnection.Transfer>();

            string request = MockWebConnection.MockQueryDataRequest;
            foreach (ItemReference itemRef in itemRefs)
            {
                string nextPageUrl = string.Empty;
                XElement responseDetails;
                if (++nextPage > total)
                {
                    responseDetails = new XElement("responseDetails", new XElement("total", totalResponse));
                }
                else
                {
                    nextPageUrl = "/" + nextPage.ToString();
                    responseDetails = new XElement(
                        "responseDetails",
                        new XElement("next_page", nextPageUrl),
                        new XElement("total", totalResponse));
                }

                XElement responseData = new XElement("data", ItemRow(itemRef));
                transfers.Add(new MockWebConnection.Transfer
                {
                    Request = request,
                    Response = SuccessResponse(responseDetails, responseData).ToString()
                });
                request = "GET " + nextPageUrl;
            }

            return new MockWebConnection(transfers);
        }

        /// <summary>
        /// Create xml string for a successful response with the given xml content
        /// </summary>
        /// <param name="responseDetails">details about the successful response </param>
        /// <param name="responseData">the successful response data</param>
        /// <returns>the success response with the attached responseDetails and data</returns>
        private static XDocument SuccessResponse(XElement responseDetails, XElement responseData)
        {
            XDocument responseXml = new XDocument(new XElement(
                "x",
                new XElement("responseStatus", "SUCCESS"),
                responseDetails,
                responseData));
            return responseXml;
        }


        /// <summary>
        /// Create Dialog for series of pages containing the listed items, one item per page
        /// </summary>
        /// <param name="itemRefs">the list of items to be returns</param>
        /// <returns>a set of exchanges corresponding to a series of requests/responses for the items
        /// with a page size of 1</returns>
        private static List<ItemReference> ToItemList(IEnumerable<List<string[]>> itemPages)
        {
            List<List<string[]>> itemPageList = itemPages.ToList();
            Assert.AreEqual(1, itemPageList[0].Count, "Expected 1 FieldName row in first page");
            string[] fieldNames = itemPageList[0][0];

            var convertToItemRef = (fieldNames.Length == 1) ?
                    (Func<string[], ItemReference>)((string[] r) => new ItemReference { Id = r[0] }) :
                    ((string[] r) => new ItemReference { Id = r[0], MajorVersion = r[1], MinorVersion = r[2] });
            return itemPageList.Skip(1).SelectMany(page => page.Select(item => item)).Select(convertToItemRef).ToList();
        }

        /// <summary>
        /// Create responseDetails xml string for single-page response containing the requested items
        /// </summary>
        /// <param name="itemRefs">the items to include in the response</param>
        /// <returns>A responseDetails xml node corresponding to the list of items</returns>
        private static XElement TableResponseDetails(IList<ItemReference> itemRefs)
        {
            XElement responseDetails = new XElement("responseDetails", new XElement("total", itemRefs.Count().ToString()));
            XElement responseData = new XElement("data", itemRefs.Select(ItemRow));
            XDocument responseDocument = SuccessResponse(responseDetails, responseData);
            XElement returnValue = responseDocument.Root;
            return returnValue;
        }

        private static XPathNavigator AsResponse(XElement response)
        {
            XmlReader reader = response.CreateReader();
            XPathNavigator returnValue = new XPathDocument(reader).CreateNavigator();
            returnValue.MoveToFirstChild();
            return returnValue;
        }

        /// <summary>
        /// Create row xml node for one item reference
        /// </summary>
        /// <param name="itemRef">the item reference</param>
        /// <returns>an xml row element for the item</returns>
        private static XElement ItemRow(ItemReference itemRef)
        {
            List<XElement> nodes = new List<XElement> { new XElement(VaultSymbols.IdFieldName, itemRef.Id) };
            if (!string.IsNullOrEmpty(itemRef.MinorVersion))
            {
                nodes.Add(new XElement(VaultSymbols.MajorVersionFieldName, itemRef.MajorVersion));
                nodes.Add(new XElement(VaultSymbols.MinorVersionFieldName, itemRef.MinorVersion));
            }

            XElement returnValue = new XElement("row", nodes.ToArray());
            return returnValue;
        }

        /// <summary>
        /// verify the deletion urn for the details in the data source, with item id "1"
        /// </summary>
        private void GetDeleteUrnTry()
        {
            ItemReference item = new ItemReference
            {
                Id = "1",
                MajorVersion = TestContext.DataRow[2].ToString(),
                MinorVersion = TestContext.DataRow[3].ToString()
            };

            DeletionRequest deleteParameters = new DeletionRequest
            {
                Type = new DeletionRequest.ItemType
                {
                    Category = (DeletionRequest.ItemCategory)Enum.Parse(typeof(DeletionRequest.ItemCategory), TestContext.DataRow[0].ToString()),
                    ObjectName = TestContext.DataRow[1].ToString(),
                    WithVersion = !(string.IsNullOrEmpty(item.MajorVersion) && string.IsNullOrEmpty(item.MinorVersion))
                },
                Batched = !string.IsNullOrEmpty(TestContext.DataRow[4].ToString()),
                Cascade = !string.IsNullOrEmpty(TestContext.DataRow[5].ToString())
            };
            string expected = string.Format(TestContext.DataRow[6].ToString(), item.Id);
            string actual;
            actual = DeleteHelper.GetDeleteUrl(
                deleteParameters.Type,
                deleteParameters.Cascade,
                deleteParameters.Batched ? null : item);
            Assert.AreEqual(expected, actual);
        }
        /// <summary>
        /// A test for GetDeleteUrn Documents
        /// </summary>
        [DeploymentItem("VeevaDeleteLib.Tests\\goodurn.csv"), DeploymentItem("goodurn.csv"), DataSource(
            "Microsoft.VisualStudio.TestTools.DataSource.CSV",
            "|DataDirectory|\\goodurn.csv",
            "goodurn#csv",
            DataAccessMethod.Sequential), TestMethod]
        public void GetDeleteUrnGoodTest()
        {
            this.GetDeleteUrnTry();
        }

        /// <summary>
        /// A test for GetDeleteUrn Documents
        /// </summary>
        [DeploymentItem("VeevaDeleteLib.Tests\\badurn.csv"), DeploymentItem("badurn.csv"), DataSource(
            "Microsoft.VisualStudio.TestTools.DataSource.CSV",
            "|DataDirectory|\\badurn.csv",
            "badurn#csv",
            DataAccessMethod.Sequential), TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetDeleteUrnBadTest()
        {
            this.GetDeleteUrnTry();
        }


        /// <summary>
        /// Get a mock XML success response for a list of items
        /// </summary>
        /// <param name="items">the list of item references</param>
        /// <returns>XML response containing list of successful responses</returns>
        public static string GetSuccessResponse(IEnumerable<ItemReference> items)
        {
            if (!(items != null))
            {
                throw new ArgumentNullException(nameof(items));
            }

            Contract.EndContractBlock();

            string returnValue = "<x><responseStatus>SUCCESS</responseStatus><data>" +
                string.Join(
                string.Empty,
                items.Select(item => "<data><responseStatus>SUCCESS</responseStatus><id>" + item.Id + "</id></data>"))
                + "</data></x>";
            return returnValue;
        }



    }
}
