//-----------------------------------------------------------------------
// <copyright file="DeleteHelper.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.XPath;
using System.Xml.Linq;
using Microsoft.VisualBasic.FileIO;
using RestUtility.Api;
using RestUtility.VeevaVaultXml;
using TextTable.Reader;

namespace VeevaDeleteApi
{
    using IntPair = Tuple<int, int>;

    /// <summary>
    /// Utility to support processing a table of ItemReferences
    /// </summary>
    public static class DeleteHelper
    {
        /// <summary>
        /// Delegate type for Action to be `performed after attempting to delete an item
        /// </summary>
        /// <param name="itemReference">Identifies the item where deletion was attempted</param>
        /// <param name="status">Identifies if the deletion succeeded</param>
        /// <param name="errors">Identifies errors details if the deletion failed</param>
        public delegate void VisitAction(ItemReference itemReference, bool status, IEnumerable<VaultXmlHelper.VaultError> errors);

        public delegate void BatchItemDeleteProcessed(BatchItemDeleteStatus progress);

        /// <summary>
        /// execute the <paramref name="deletionRequest"/> on <paramref name="connection"/>
        /// </summary>
        /// <param name="deletionRequest">object identifying what should be deleted</param>
        /// <param name="connection">the web connection from which the items are to be deleted</param>
        /// <param name="batchStatus">object to track number of successful/failed deletions</param>
        /// <param name="token">cancellation token to cancel the process</param>
        public static void ExecuteDeletionRequest(
            this DeletionRequest deletionRequest,
            IRestConnection connection,
            BatchItemDeleteProcessed progressReporter,
            CancellationToken token)
        {
            if (!(connection != null))
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (!(deletionRequest != null))
            {
                throw new ArgumentNullException(nameof(deletionRequest));
            }

            Contract.EndContractBlock();

            IItemWalker walker = deletionRequest.GetItemWalker();
            DeletionRequest.ItemCategory itemCategory = deletionRequest.Type.Category;
            bool withVersion = deletionRequest.Type.WithVersion;
            string objectName = deletionRequest.Type.ObjectName;
            string majorVersion = withVersion ? deletionRequest.Type.MajorVersion : null;
            string minorVersion = withVersion ? deletionRequest.Type.MinorVersion : null;

            using (IEnumerator<ItemReference> itemList = new ItemWalkHelper(walker, deletionRequest.Type).GetEnumerator())
            {

                IItemVisitor visitor = walker as IItemVisitor;

                ////Hashtable results = null;
                //                int successCount = 0;
                //                int failureCount = 0;


                void visitAction(ItemReference itemReference, bool successStatus, IEnumerable<VaultXmlHelper.VaultError> errors)
                {
                    string message = successStatus ?
                        string.Format("{0} {1} has been successfully deleted", deletionRequest.Type, itemReference) :
                        string.Format("Failed to delete {0} {1}: ", deletionRequest.Type, itemReference);
                    progressReporter(new BatchItemDeleteStatus { ItemSuccessStatus = successStatus, Message = message });
                    if (visitor != null)
                    {
                        int updated = visitor.Visited(itemReference, successStatus ? "Deleted" : "Error-Not deleted");
                        if (updated > 1)
                        {
                            message = "Duplicate id: " + itemReference.ToString();
                            progressReporter(new BatchItemDeleteStatus { Message = message });
                        }
                    }

                    if (errors != null)
                    {
                        foreach (VaultXmlHelper.VaultError error in errors)
                        {
                            message = "Api error : " + error.ToString();
                            progressReporter(new BatchItemDeleteStatus { Message = message });
                        }
                    }
                }

                deletionRequest.DeleteSet(connection, progressReporter, itemList, visitAction, token);
            }

        }

        /// <summary>
        /// delete the items identified by the selectObject
        /// </summary>
        /// <param name="deletionRequest">object identifying the the type of deletion to perform</param>
        /// <param name="connection">the web connection from which the items are to be deleted</param>
        /// <param name="itemEnumerator">a list of the items to delete</param>
        /// <param name="visitAction">action to perform after attempting to delete each item, to record success/failure</param>
        /// <param name="token">cancellation token to stop the process</param>
        public static void DeleteSet(
            this DeletionRequest deletionRequest,
            IRestConnection connection,
            BatchItemDeleteProcessed progressReporter,
            IEnumerator<ItemReference> itemEnumerator,
            VisitAction visitAction,
            CancellationToken token)
        {
            if (!(deletionRequest != null))
            {
                throw new ArgumentNullException(nameof(deletionRequest));
            }

            if (!(connection != null))
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (!(progressReporter != null))
            {
                throw new ArgumentNullException(nameof(progressReporter));
            }

            if (!(itemEnumerator != null))
            {
                throw new ArgumentNullException(nameof(itemEnumerator));
            }

            if (!(visitAction != null))
            {
                throw new ArgumentNullException(nameof(visitAction));
            }

            Contract.EndContractBlock();
            (bool more, bool completed) = TryGetNext(itemEnumerator, progressReporter);
            if (!more && completed)
            {
                string message = "No id(s) found to delete.";
                progressReporter(new BatchItemDeleteStatus { Message = message });
            }

            connection.TimeoutLength = deletionRequest.TimeoutLength ?? Timeout.Infinite;
            connection.TimeoutRetries = deletionRequest.TimeoutRetries ?? Timeout.Infinite;

            if (deletionRequest.Batched)
            {
                int maxBatchSize;
                {
                    int connectionMaxBatchSize = connection.MaxBatchSize;
                    if (connectionMaxBatchSize == 0)
                    {
                        throw new InvalidOperationException("Batch deletion not available on this connection");
                    }
                    maxBatchSize = deletionRequest.BatchSize ?? connectionMaxBatchSize;
                    if (deletionRequest.BatchSize.HasValue)
                    {
                        if (maxBatchSize < 1)
                        {
                            throw new InvalidOperationException($"Batch size {maxBatchSize} is smaller than minimum size 1");
                        }
                        if (maxBatchSize > connectionMaxBatchSize)
                        {
                            throw new InvalidOperationException($"Batch size {maxBatchSize} is larger than maximum size {connectionMaxBatchSize}");
                        }

                    }
                }

                ItemReference[] batch = new ItemReference[maxBatchSize];
                StringBuilder sb = new StringBuilder();
                while (more)
                {
                    token.ThrowIfCancellationRequested();
                    sb.Clear();
                    CollectBatchHeader(sb, deletionRequest);
                    int batchSize = 0;
                    for (; more && batchSize < maxBatchSize; ++batchSize)
                    {
                        batch[batchSize] = itemEnumerator.Current;
                        if (!CollectBatchItem(sb, deletionRequest, batch[batchSize]))
                        {
                            string message = DeleteHelper.ItemVersionError(batch[batchSize]);
                            // TODO: report different status for malformed request?
                            progressReporter(new BatchItemDeleteStatus { ItemSuccessStatus = false, Message = message });
                        }
                        (more, completed) = TryGetNext(itemEnumerator, progressReporter);
                    }


                    //try
                    //{
                    IEnumerable<VaultXmlHelper.VaultStatus> responses = deletionRequest.DeleteBatch(
                        connection,
                        progressReporter,
                        sb.ToString()
                        );
                    //} catch (Exception e)
                    //{ 
                    //
                    //}
                    using (
                        IEnumerator<VaultXmlHelper.VaultStatus> responseEnumerator = responses?.GetEnumerator())
                    {

                        for (int line = 0; line < batchSize; ++line)
                        {
                            if (responseEnumerator != null && responseEnumerator.MoveNext())
                            {
                                VaultXmlHelper.VaultStatus vaultStatus = responseEnumerator.Current;
                                if (!string.IsNullOrEmpty(vaultStatus.Id) && vaultStatus.Id != batch[line].Id)
                                {
                                    throw new UnexpectedResponseRestApiException(string.Format(
                                        "Expected response for {0}, found response for {1}",
                                        batch[line].Id,
                                        vaultStatus.Id));
                                }

                                visitAction(batch[line], vaultStatus.Success, vaultStatus.Errors);
                            }
                            else
                            {
                                visitAction(batch[line], false, null);
                            }
                        }
                    }

                }
            }
            else
            {
                while (more)
                {
                    ItemReference itemReference = itemEnumerator.Current;
                    token.ThrowIfCancellationRequested();
                    if (CheckVersionOk(deletionRequest, itemReference))
                    {
                        deletionRequest.DeleteItem(
                            connection,
                            progressReporter,
                            itemReference,
                            visitAction);
                    }
                    else
                    {
                        string message = DeleteHelper.ItemVersionError(itemReference);
                        // TODO: report different status for malformed request?
                        progressReporter(new BatchItemDeleteStatus { ItemSuccessStatus = false, Message = message });
                    }
                    (more,completed)=TryGetNext(itemEnumerator, progressReporter);
                }
            }
            if (completed)
            {
                progressReporter(new BatchItemDeleteStatus { Message = "Deletion completed." });
            }
        }


        private static (bool,bool)TryGetNext(IEnumerator<ItemReference> itemEnumerator, BatchItemDeleteProcessed progressReporter)
        {
            bool more;
            bool complete;
            try
            {
                more = itemEnumerator.MoveNext();
                complete = !more;
            }
            catch (Exception ex)
            {
                string msg = ex.FormatError();
                progressReporter(new BatchItemDeleteStatus { Message = "Cannot get next item to delete:" +  msg });
                more = false;
                complete = false;
            }
            return (more, complete);
        }

        /*
         * private not implemented method not required
         * presumably originally intended to help refactor excel/access datatable generator
        private string GetExcelConnectionString()
        {
            throw new NotImplementedException();
        }
        */

        /// <summary>
        /// Add the batch header for the deletion type
        /// </summary>
        /// <param name="sb">the string builder to add the batch header into</param>
        /// <param name="deleteParameters">the deletion type</param>
        public static void CollectBatchHeader(StringBuilder sb, DeletionRequest deleteParameters)
        {
            if (!(sb != null))
            {
                throw new ArgumentNullException(nameof(sb));
            }

            if (!(deleteParameters != null))
            {
                throw new ArgumentNullException(nameof(deleteParameters));
            }

            Contract.EndContractBlock();

            sb.Append("id");
            if (deleteParameters.Type.WithVersion)
            {
                sb.Append(",major_version_number__v,minor_version_number__v");
            }

            sb.AppendLine();
        }

        /// <summary>
        /// Add the batch item reference for the batch deletion
        /// </summary>
        /// <param name="sb">the string builder to add the batch header into</param>
        /// <param name="deleteParameters">the deletion type</param>
        /// <param name="itemReference">the item to add</param>
        public static bool CollectBatchItem(StringBuilder sb, DeletionRequest deleteParameters, ItemReference itemReference)
        {
            if (!(sb != null))
            {
                throw new ArgumentNullException(nameof(sb));
            }

            if (!(deleteParameters != null))
            {
                throw new ArgumentNullException(nameof(deleteParameters));
            }

            if (!(itemReference != null))
            {
                throw new ArgumentNullException(nameof(itemReference));
            }

            Contract.EndContractBlock();

            bool versionOk = CheckVersionOk(deleteParameters, itemReference);
            if (versionOk)
            {
                sb.Append(itemReference.Id);
                if (deleteParameters.Type.WithVersion)
                {
                    sb.Append(",");
                    sb.Append(itemReference.MajorVersion);
                    sb.Append(",");
                    sb.Append(itemReference.MinorVersion);
                }

                sb.AppendLine();
            }
            return versionOk;
        }

        /// <summary>
        /// Check if the item reference includes version number where required for the deletion type
        /// Include any exceptions in found in the log
        /// </summary>
        /// <param name="deleteParameters">the deletion type</param>
        /// <param name="itemReference">the item reference</param>
        /// <returns>true if version details are supplied, or not required</returns>
        public static bool CheckVersionOk(DeletionRequest deleteParameters, ItemReference itemReference)
        {
            if (!(deleteParameters != null))
            {
                throw new ArgumentNullException(nameof(deleteParameters));
            }
            if (!(deleteParameters.Type != null))
            {
                throw new ArgumentNullException(nameof(deleteParameters.Type));
            }

            Contract.EndContractBlock();

            bool versionOk = !deleteParameters.Type.WithVersion ||
                (!string.IsNullOrEmpty(itemReference.MajorVersion) && !string.IsNullOrEmpty(itemReference.MinorVersion));
            return versionOk;
        }

        private static string ItemVersionError(ItemReference itemReference)
        {
            string result = string.Format(
                "Missing verion number for document: {0}/{1}/{2} ",
                itemReference.Id,
                itemReference.MajorVersion,
                itemReference.MinorVersion);
            return result;
        }

        static readonly XPathExpression tableDataExpression = XPathExpression.Compile(VaultGrid.Table.Data);
        /// <summary>
        /// Delete a set of vault items, updating the table of items to mark as successfully deleted or not where possible
        /// </summary>
        /// <param name="connection">the vault connection</param>
        /// <param name="deleteParameters">the table of items to be deleted</param>
        /// <param name="references">a CSV formatted string containing the references to be deleted</param>
        /// <returns>success or failure status</returns>
        public static IEnumerable<VaultXmlHelper.VaultStatus> DeleteBatch(
            this DeletionRequest deleteParameters,
            IRestConnection connection,
            BatchItemDeleteProcessed progressReporter,
            string references)
        {
            if (!(deleteParameters != null))
            {
                throw new ArgumentNullException(nameof(deleteParameters));
            }

            if (!(connection != null))
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (deleteParameters.Cascade)
            {
                throw new InvalidOperationException("Batch cascade delete not supported");
            }

            Contract.EndContractBlock();

            string deleteUrn = GetDeleteUrl(
                deleteParameters.Type,
                deleteParameters.Cascade,
                null);

            IEnumerable<VaultXmlHelper.VaultStatus> returnValue;
            ////DeleteHelper.UpdateLogs("Deleting url : " + deleteUrn);
            try
            {

                for (; ; ) // retry while busy
                {

                    if (connection.TryExecuteXmlApi(out VaultXmlHelper.VaultResponse response, "DELETE", deleteUrn, references))
                    {
                        progressReporter(new BatchItemDeleteStatus { Message = "Deletion batch has been successfully processed" });
                        returnValue = response.GetStatusData(VaultGrid.Table.Data);
                        break;
                    }
                    else if (!response.Busy)
                    {

                        progressReporter(new BatchItemDeleteStatus { Message = "Deletion batch has failed" });
                        progressReporter(new BatchItemDeleteStatus { Message = "Failed url : " + deleteUrn });
                        foreach (VaultXmlHelper.VaultError error in response.Errors)
                        {
                            progressReporter(new BatchItemDeleteStatus { Message = "Api error : " + error.ToString() });
                        }
                        returnValue = null;
                        break;
                    }
                    else
                    {
                        progressReporter(new BatchItemDeleteStatus { Message = "Deletion batch request outside service limit" });
                        progressReporter(new BatchItemDeleteStatus { Message = "Retrying url : " + deleteUrn });
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.FormatError();
                progressReporter(new BatchItemDeleteStatus { Message = "API request failed on url: " + deleteUrn + " : " + msg });
                returnValue = new VaultXmlHelper.VaultStatus[] { };
            }

            return returnValue;
        }

        /// <summary>
        /// Delete a set of vault items, updating the table of items to mark as successfully deleted or not where possible
        /// </summary>
        /// <param name="deletionRequest">the table of items to be deleted</param>
        /// <param name="connection">the vault connection</param>
        /// <param name="itemReference">an individual item from the table of items to be deleted</param>
        /// <param name="visitAction">action to perform after attempting to delete each item (to record success/failure)</param>
        public static void DeleteItem(
            this DeletionRequest deletionRequest,
            IRestConnection connection,
            BatchItemDeleteProcessed progressReporter,
            ItemReference itemReference,
            VisitAction visitAction)
        {
            if (!(deletionRequest != null))
            {
                throw new ArgumentNullException(nameof(deletionRequest));
            }

            if (!(connection != null))
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (!(itemReference != null))
            {
                throw new ArgumentNullException(nameof(itemReference));
            }

            if (!(visitAction != null))
            {
                throw new ArgumentNullException(nameof(visitAction));
            }

            Contract.EndContractBlock();
            string deleteUrn = GetDeleteUrl(
                deletionRequest.Type,
                deletionRequest.Cascade,
                itemReference);

            ////DeleteHelper.UpdateLogs("Deleting url : " + deleteUrn);

            try
            {
                for (; ; )
                {

                    connection.TryExecuteXmlApi(out VaultXmlHelper.VaultResponse response, deletionRequest.Cascade ? "POST" : "DELETE", deleteUrn);
                    if (!response.Busy)
                    {
                        if (!response.Success)
                        {
                            progressReporter(new BatchItemDeleteStatus { Message = "Failed url : " + deleteUrn });
                        }
                        visitAction(itemReference, response.Success, response.Errors);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.FormatError();
                progressReporter(new BatchItemDeleteStatus { Message = "API request failed on url: " + deleteUrn + " : " + msg });
            }
        }



        /// <summary>
        /// count the number of items listed for deletion in the file
        /// </summary>
        /// <param name="selectType">the type of file to search for IDs</param>
        /// <param name="selectFile">the name of the file to search for IDS</param>
        /// <returns>the number of items listed for deletion, -1 if there was an error, or -2 if a file was not found</returns>
        public static int GetNumberOfRecsFromFile(Constants.SourceType selectType, DeletionRequest.ItemType itemType, string selectFile)
        {
            int numOfRecs;
            using (IFileItemWalker walker = DeleteHelper.GetFileWalker(selectType, itemType))
            {
                walker.Filename = selectFile;
                numOfRecs = walker.GetItemCount();
                return numOfRecs;
            }
        }

        /// <summary>
        /// Find the correct reader function for the specified file type.
        /// </summary>
        /// <param name="selectType">file type indicator</param>
        /// <returns>delegate referencing the correct reader</returns>
        public static IFileItemWalker GetFileWalker(Constants.SourceType selectType, DeletionRequest.ItemType itemType)
        {
            Contract.Ensures(Contract.Result<IFileItemWalker>() != null);
            IFileItemWalker returnValue;
            switch (selectType)
            {
                case Constants.SourceType.Csv:
                    returnValue = new CsvFileItemWalker();
                    break;
                case Constants.SourceType.Excel:
                    returnValue = new ExcelTableItemWalker { ItemType = itemType };
                    break;
                case Constants.SourceType.Access:
                    returnValue = new AccessTableItemWalker { ItemType = itemType };
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Unrecognised file type {0}", selectType));
            }

            return returnValue;
        }


        /// <summary>
        /// Check to see how many items will be deleted
        /// </summary>
        /// <param name="deletionRequest">The deletion request details</param>
        /// <returns>If the selection is OK, the selected and maximum number of items to be deleted or =1 for no maximum
        /// If the selection is not OK, throws an error describing the problem</returns>
        public static int GetSelectCount(this DeletionRequest deletionRequest)
        {

            int numOfRecs;
            Constants.SourceType selectType = deletionRequest.RelationType;
            IRestConnection connection = deletionRequest.RelationConnection;

            if (selectType == Constants.SourceType.Query)
            {
                string selectQuery = GetItemQuery(deletionRequest);
                Contract.Assume(connection != null);
                numOfRecs = connection.GetCount(selectQuery.Trim());
            }
            else
            {
                string selectFile = deletionRequest.RelationFile;
                numOfRecs = DeleteHelper.GetNumberOfRecsFromFile(selectType, deletionRequest.Type, selectFile);
            }

            return numOfRecs;
        }

        /// <summary>
        /// return the text for a VQL query where the source is VQL
        /// </summary>
        /// <param name="deletionRequest"></param>
        /// <returns></returns>
        public static string GetItemQuery(DeletionRequest deletionRequest)
        {
            return deletionRequest.RelationType == Constants.SourceType.Query ?
                deletionRequest.GetItemQuery() :
                string.Empty;
        }


        /// <summary>
        /// Get the UI name for the item type to be deleted
        /// </summary>
        /// <param name="connection">the connection to delete on</param>
        /// <param name="itemType">the item type to delete</param>
        /// <returns>the description of type type of item that will be deleted</returns>
        public static Tuple<string, string> GetUINames(this IRestConnection connection, DeletionRequest.ItemType itemType)
        {
            if (!(itemType != null))
            {
                throw new ArgumentNullException(nameof(itemType));
            }

            Contract.EndContractBlock();

            if (itemType.Category == DeletionRequest.ItemCategory.Object)
            {
                return connection.GetObjectUINames(itemType.ObjectName);
            }
            string itemCategoryName = itemType.Category.ToString();
            return new Tuple<string, string>(itemCategoryName, itemCategoryName + "s");
        }

        /// <summary>
        /// Get the version specification description for the item to be deleted
        /// </summary>
        /// <param name="itemType">the item type to delete</param>
        /// <returns>the description of what version be deleted according to the item type</returns>
        public static string GetVersionDescription(this DeletionRequest.ItemType itemType)
        {
            if (!(itemType != null))
            {
                throw new ArgumentNullException(nameof(itemType));
            }

            Contract.EndContractBlock();

            string returnValue =
                itemType.Category == DeletionRequest.ItemCategory.Object ?
                string.Empty :
                    (itemType.WithVersion ?
                    string.IsNullOrEmpty(itemType.MajorVersion) ?
                    "specific version" :
                    "version " + itemType.MajorVersion + "." + itemType.MinorVersion :
                    "all versions") + " of ";
            return returnValue;
        }

        /// <summary>
        /// Delegate type for Action to be `performed for each item reference
        /// </summary>
        /// <param name="withVersion">Identifies if the source specified a version</param>
        /// <param name="itemReference">Identifies the item where from the source</param>
        //public delegate void CollectAction(bool withVersion, ItemReference itemReference);

        /// <summary>
        /// calculate the Urn required to delete the specified item
        /// </summary>
        /// <param name="itemType">The type of item to be deleted</param>
        /// <param name="cascade">Should a cascade delete be performed?</param>
        /// <param name="itemReference">The object to be deleted except for batch delete</param>
        /// <returns>the deletion relative Url corresponding to docObject</returns>
        public static string GetDeleteUrl(
            DeletionRequest.ItemType itemType,
            bool cascade,
            ItemReference itemReference)
        {
            if (!(itemType != null))
            {
                throw new ArgumentNullException(nameof(itemType));
            }

            if (!(itemReference != null || itemType.Category != DeletionRequest.ItemCategory.Binder))
            {
                throw new InvalidOperationException("Batch delete not supported on Binders");
            }

            if (!(!cascade || itemType.Category == DeletionRequest.ItemCategory.Object))
            {
                throw new InvalidOperationException("Cascade delete only supported on Objects");
            }

            if (!(!string.IsNullOrEmpty(itemType.ObjectName) || itemType.Category != DeletionRequest.ItemCategory.Object))
            {
                throw new InvalidOperationException("Name required for Object");
            }

            if (!(itemReference != null || !cascade))
            {
                throw new InvalidOperationException("Batch cascade delete not supported");
            }

            if (!(itemReference == null || cascade || itemType.Category != DeletionRequest.ItemCategory.Object))
            {
                throw new InvalidOperationException("Item delete not supported on Objects");
            }

            if (!(!itemType.WithVersion || itemType.Category != DeletionRequest.ItemCategory.Object))
            {
                throw new InvalidOperationException("Version delete not supported on Objects");
            }

            Contract.EndContractBlock();

            string deleteUrl;
            switch (itemType.Category)
            {
                case DeletionRequest.ItemCategory.Object:
                    deleteUrl = "vobjects/" + itemType.ObjectName + (cascade ? "/{0}/actions/cascadedelete" : string.Empty);
                    break;
                //AK, 10/1/2020, adding Archived Documents type
                case DeletionRequest.ItemCategory.Archived_Document:
                    deleteUrl = "objects/documents/batch";
                    break;
                    //end changes by AK on 10/1/2020
                case DeletionRequest.ItemCategory.Document:
                case DeletionRequest.ItemCategory.Binder:
                    deleteUrl =
                        (itemType.Category == DeletionRequest.ItemCategory.Document ? "objects/documents" : "objects/binders") +
                        (itemReference != null ? "/{0}" : string.Empty) +
                        (itemType.WithVersion ? "/versions" : string.Empty) +
                        (itemReference == null ? "/batch" : itemType.WithVersion ? "/{1}/{2}" : string.Empty);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(itemType));
            }

            if (itemReference != null)
            {
                deleteUrl = string.Format(
                    deleteUrl,
                    itemReference.Id,
                    itemReference.MajorVersion,
                    itemReference.MinorVersion);
            }

            return deleteUrl;
        }

        public static String FormatError(this Exception ex)
        {
            return ex.Message + ex.InnerException?.ToString();
        }
    }
}
