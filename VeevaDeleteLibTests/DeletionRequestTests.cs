using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestUtility.Api;
using RestUtility.Tests;
using VeevaDeleteApi;

namespace VeevaDeleteLib.Tests
{
    [TestClass()]
    public class DeletionRequestTests
    {
        [TestMethod()]
        public void GetItemQueryDocumentTest()
        {
            DeletionRequest deleteParameters = new DeletionRequest
            {
                RelationConnection = new MockWebConnection(),
                RelationType = Constants.SourceType.Query,
                Type = new DeletionRequest.ItemType
                {
                    Category = DeletionRequest.ItemCategory.Document,
                },
            };

            string expected =
                "select id from documents where binder__v = false";
            string actual = deleteParameters.GetItemQuery();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetItemQueryDocumentFindTest()
        {
            DeletionRequest deleteParameters = new DeletionRequest
            {
                RelationConnection = new MockWebConnection(),
                RelationType = Constants.SourceType.Query,
                Type = new DeletionRequest.ItemType
                {
                    Category = DeletionRequest.ItemCategory.Document,
                },
                SelectionKeywordSearch = "'wibble'"
            };

            string expected =
                "select id from documents find ('wibble') where binder__v = false";
            string actual = deleteParameters.GetItemQuery();
            Assert.AreEqual(expected, actual);
        }
        [TestMethod()]
        public void GetItemQueryDocumentFindWhere()
        {
            DeletionRequest deleteParameters = new DeletionRequest
            {
                RelationConnection = new MockWebConnection(),
                RelationType = Constants.SourceType.Query,
                Type = new DeletionRequest.ItemType
                {
                    Category = DeletionRequest.ItemCategory.Document,
                },
                SelectionKeywordSearch = "'wibble'",
                SelectionFilter = "1=2"
            };

            string expected =
                "select id from documents find ('wibble') where binder__v = false and 1=2";
            string actual = deleteParameters.GetItemQuery();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetItemQueryBinderTest()
        {
            DeletionRequest deleteParameters = new DeletionRequest
            {
                RelationConnection = new MockWebConnection(),
                RelationType = Constants.SourceType.Query,
                Type = new DeletionRequest.ItemType
                {
                    Category = DeletionRequest.ItemCategory.Binder,
                    WithVersion = true,
                    MajorVersion = "1",
                    MinorVersion = "0"
                },
                SelectionFilter = "run_id__c='x'"
            };

            string expected =
                "select id, major_version_number__v, minor_version_number__v from documents where binder__v = true" +
                " and major_version_number__v = 1 and minor_version_number__v = 0 and " +
                deleteParameters.SelectionFilter;

            string actual = deleteParameters.GetItemQuery();
            Assert.AreEqual(expected, actual);
        }


        [TestMethod()]
        public void GetItemQueryObjectTest()
        {
            DeletionRequest deleteParameters = new DeletionRequest
            {
                RelationConnection= new MockWebConnection(),
                RelationType = Constants.SourceType.Query,
                Type = new DeletionRequest.ItemType
                {
                    Category = DeletionRequest.ItemCategory.Object,
                    ObjectName = "test_object__c"
                },
                SelectionFilter = "run_id__c='x'"
            };

            string expected =
                "select id from test_object__c where " + deleteParameters.SelectionFilter;
            string actual = deleteParameters.GetItemQuery();
            Assert.AreEqual(expected, actual);
        }
    }
}