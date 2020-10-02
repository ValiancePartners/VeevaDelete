//-----------------------------------------------------------------------
// <copyright file="ItemWalkHelper.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using RestUtility.Api;
using RestUtility.VeevaVaultXml;
using TextTable.Reader;

namespace VeevaDeleteApi
{
    /// <summary>
    /// Helper class for walking a table of items
    /// </summary>
    public class ItemWalkHelper: IEnumerable<ItemReference>, IEnumerable
    {
        private readonly IDataReader reader;
        private readonly string tableName;
        private readonly DeletionRequest.ItemType itemType;

        /// <param name="`walker">a data table of item reference details</param>
        /// <param name="getVersion">indicate if the version number should be collected from the data table</param>
        /// <param name="majorVersion">the majorVersion number to use instead of from the data table</param>
        /// <param name="minorVersion">the minorVersion number to use instead of from the data table</param>
        public ItemWalkHelper(IItemWalker walker, DeletionRequest.ItemType itemType)
            :  this(walker.GetReader(), walker.TableName, itemType)
        {
        }

        public ItemWalkHelper(IDataReader reader, string tableName, DeletionRequest.ItemType itemType)
        {
            this.reader = reader;
            this.tableName = tableName;
            this.itemType = itemType;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <returns>the list of item references from the table, qualified by objectName and/or version number where relevant</returns>
        public IEnumerator<ItemReference> GetEnumerator()
        {
            (int idCol, int? majorVersionCol, int? minorVersionCol) = GetRequiredColumns(itemType,GetRequiredColumnNo);
            
            while (this.reader.Read())
            {
                object id = this.reader.GetValue(idCol);
                object majorRowVersion = !majorVersionCol.HasValue ? this.itemType.MajorVersion : this.reader.GetValue(majorVersionCol.Value);
                    ;

                // log while deleting instead of here               
                ////if (getVersion && string.IsNullOrEmpty(majorRowVersion))
                ////{
                ////    throw new InvalidOperationException(string.Format("Major Version Information not supplied for id: {0}", id));
                ////}

                object minorRowVersion = !minorVersionCol.HasValue ? this.itemType.MinorVersion : this.reader.GetValue(minorVersionCol.Value);

                // log while deleting instead of here
                ////if (getVersion && string.IsNullOrEmpty(minorRowVersion))
                ////{
                ////    throw new InvalidOperationException(string.Format("Minor Version Information not supplied for id: {0}", id));
                ////}

                ItemReference tempReference = new ItemReference
                {
                    Id = id?.ToString(),
                    WithVersion = itemType.WithVersion,
                    MajorVersion = majorRowVersion?.ToString(),
                    MinorVersion = minorRowVersion?.ToString()
                };

                yield return tempReference;
            }
            reader.Dispose();
            yield break;
        }


        /// <summary>
        /// Get a column from the data table.
        /// Throw an exception if the column is not in the table.
        /// </summary>
        /// <param name="columnName">the name of the column to get</param>
        /// <returns>Data Column for requested column</returns>
        private int GetRequiredColumnNo(string columnName)
        {
            Contract.EndContractBlock();
            // GetOrdinal may throw or may return -1 for unknown columns
            try {
                int colIndex = reader.GetOrdinal(columnName);
                if(colIndex<0)
                {
                    throw new ItemSourceException(string.Format("{0} column not found in {1}", columnName, tableName));
                }
                return colIndex;
            }
            catch (System.IndexOutOfRangeException ex)
            {
                throw new ItemSourceException(string.Format("{0} column not found in {1}", columnName, tableName), ex);
            }
        }

        public static (int,int?,int?) GetRequiredColumns(DeletionRequest.ItemType itemType, Func<string,int> GetRequiredColumnNo)
        {
            int idCol = GetRequiredColumnNo(VaultGrid.Column.IdFieldName);

            bool tableVersion = itemType != null && itemType.WithVersion && string.IsNullOrEmpty(itemType?.MajorVersion);

            int? majorVersionCol = tableVersion ? GetRequiredColumnNo(VaultGrid.Column.MajorVersionFieldName) : (int?)null;
            int? minorVersionCol = tableVersion ? GetRequiredColumnNo(VaultGrid.Column.MinorVersionFieldName) : (int?)null;;
            return (idCol, majorVersionCol, minorVersionCol);
        }

    }
}
