//-----------------------------------------------------------------------
// <copyright file="CsvFileItemWalker.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using RestUtility.Api;
using TextTable.Reader;

namespace VeevaDeleteApi
{
    /// <summary>
    /// Implementation of IFileItemWalker for CSV files
    /// </summary>=
    public class CsvFileItemWalker : IFileItemWalker
    {
        /// <summary>
        /// Gets or sets the name of the CSV file
        /// </summary>
        public string Filename { get; set ; }

        /// <summary>
        /// Gets the table name
        /// </summary>
        /// <returns>the name of the csv file</returns>
        public string TableName {
            get { return this.Filename; }
            set { }
        }

        public DeletionRequest.ItemType ItemType { get; set; }

        /// <summary>
        /// Gets the items
        /// </summary>
        /// <returns>the table of items from the source</returns>
        public IDataReader GetReader()
        {
            var reader = new CsvDataReader(this.Filename);
            try
            {
                ItemWalkHelper.GetRequiredColumns(this.ItemType,
                (string columnName) =>
                {
                    int colIndex = reader.GetOrdinal(columnName);
                    if (colIndex < 0)
                    {
                        throw new ItemSourceException(string.Format("{0} column not found in {1}", columnName, this.Filename));
                    }
                    return colIndex;
                });
                return reader;
            }
            catch (Exception)
            {
                reader.Dispose();
                throw;
            }
        }

        /// <summary>
        /// standard dispose implementation: call protected dispose in disposing mode
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// get the number of items in the list
        /// </summary>
        /// <returns>the number of items listed in the source</returns>
        public int GetItemCount()
        {
            using (IDataReader reader = this.GetReader())
            {
                while (reader.Read()) ;
                return reader.RecordsAffected;
            }
        }

        /// <summary>
        /// standard Dispose pattern for disposing object with no unmanaged resources
        /// </summary>
        /// <param name="disposing">true when disposing of managed resources (false if called from GC)</param>
        protected virtual void Dispose(bool disposing)
        {
        }

    }
}
