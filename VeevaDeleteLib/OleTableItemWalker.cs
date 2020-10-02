//-----------------------------------------------------------------------
// <copyright file="OleTableItemWalker.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics.Contracts;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using RestUtility.Api;
using TextTable.Reader;

namespace VeevaDeleteApi
{
    /// <summary>
    /// Partial implementation of IFileItemWalker and IItemVisitor for files that can be opened as an OLE database
    /// Concrete subtypes are expected to implement details of file type and data source provider
    /// </summary>
    public abstract class OleTableItemWalker : IFileItemWalker, IItemVisitor
    {
        /// <summary>
        /// the current connection, for walking and updating the file
        /// </summary>
        private OleDbConnection _connection;

        /// <summary>
        /// the command to get the data
        /// </summary>
        private OleDbCommand _getDataCommand;

        /// <summary>
        /// the command to get the number of rows
        /// </summary>
        private OleDbCommand _getCountCommand;

        /// <summary>
        /// the command to update the current item
        /// </summary>
        private OleDbCommand _visitItemCommand;

        /// <summary>
        /// the name of the (first) table
        /// </summary>
        private string _tableName;

        /// <summary>
        /// Data Type of first column for update query
        /// </summary>
        private Type _keyType;

        /// <summary>
        /// Gets or sets the name of the file
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets flag indicating whether the file includes version numbers
        /// </summary>
        //public bool WithVersion { get; set; }
        public DeletionRequest.ItemType ItemType { get; set; }

        /// <summary>
        /// Gets the set of restrictions for finding the first table
        /// </summary>
        protected abstract string[] TableTypeRestrictions { get; }

        /// <summary>
        /// open the file
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2100: Review SQL queries for security vulnerabilities",Justification = "User input is validated")]
        public void Open()
        {
            Contract.Ensures(this._getDataCommand != null);
            Contract.Ensures(this._visitItemCommand != null);
            Contract.Assume(this.Filename != null, "Filename required");
            if (this._connection == null)
            {
                if (!File.Exists(this.Filename))
                {
                    throw new ItemSourceException(string.Format("File not found: {0}", Path.GetFullPath(this.Filename)));
                }

                this._connection = this.OpenConnection();
                this._connection.Open();
                try
                {
                    // Get all sheets
                    using (DataTable docTables = this._connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, this.TableTypeRestrictions))
                    {
                    
                        IEnumerable <DataRow> enumTables = docTables.Select();
                        if (!enumTables.Any())
                        {
                            throw new ItemSourceException(
                                string.Format("File does not contain any tables: {0}", Path.GetFullPath(this.Filename)));
                        }

                        DataRow dr = enumTables.First();
                        this.TableName = dr["TABLE_NAME"].ToString();
                        this._getDataCommand = new OleDbCommand()
                        {
                            Connection = this._connection,
                            CommandText = GetTableContentQueryCommandText()
                        };
                        this._getDataCommand.Prepare();

                        this._getCountCommand = new OleDbCommand()
                        {
                            Connection = this._connection,
                            CommandText = "SELECT COUNT(*) FROM [" + this._tableName + "]"
                        };
                        this._getCountCommand.Prepare();
                    }
                    // verify connection is ok -- ExecuteReader below silently returns nothing where there is an problem with the connection
                    using (DataTable docColumns = this._connection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new string[4] { null, null, this._tableName, null }))
                    {
                    };
                    using (DataTable dt = new DataTable())
                    {
                        var x = this._getDataCommand.ExecuteReader(CommandBehavior.SchemaOnly);
                        dt.Load(x);

                        if (dt.Columns.Count == 0)
                        {
                            throw new ItemSourceException(string.Format("Table does not contain any columns: {0}", this._tableName));
                        }
                        if (!dt.Columns.Contains("id"))
                        {
                            throw new ItemSourceException($"Table {this._tableName} does not contain an id column");
                        }

                        if (!dt.Columns.Contains("status"))
                        {
                            throw new ItemSourceException($"Table {this._tableName} does not contain a status column\nA status column is required on this source selection type");
                        }
                        this._keyType = dt.Columns["id"].DataType;
                        ItemWalkHelper.GetRequiredColumns(this.ItemType,
                            (string columnName) =>
                            {
                                if (!dt.Columns.Contains(columnName))
                                {
                                    throw new ItemSourceException(string.Format("{0} column not found in {1}", columnName, this._tableName));
                                }
                                return -1;
                            });
                    }
                    this._visitItemCommand = new OleDbCommand
                    {
                        Connection = this._connection,
                        CommandText = "Update [" + this._tableName + "] set Status=? where ID=?"
                    };
                }                                                                                                           
                catch (Exception)
                {
                    this.Close();
                    throw;
                }
            }
        }

        /// <summary>
        /// return the query command text to get the table content
        /// </summary>
        /// <returns></returns>
        public string GetTableContentQueryCommandText()
        {
            // suppress until WithVersion is global major/minor version is incorporated itno WithVersion rule
            //string returnValue = "SELECT id," + (this.WithVersion ? "major_version_number__v,minor_version_number__v," : "") + "status FROM [" + this.tableName + "]";
            string returnValue = "SELECT * FROM [" + this._tableName + "]";
            return returnValue;
        }

        /// <summary>
        /// close the file
        /// </summary>
        public void Close()
        {
            if (this._visitItemCommand != null)
            {
                this._visitItemCommand.Dispose();
                this._visitItemCommand = null;
            }

            if (this._getDataCommand != null)
            {
                this._getDataCommand.Dispose();
                this._getDataCommand = null;
            }

            if (this._connection != null)
            {
                this._connection.Close();
                this._connection.Dispose();
                this._connection = null;
            }
        }

        /// <summary>*
        /// get the first table from the OLEDB for the file
        /// </summary>
        /// <returns>the file as a DataTable</returns>
        public IDataReader GetReader()
        {
            Contract.Ensures(Contract.Result<DataTable>() != null);
            Contract.Assume(this.Filename != null);
            this.Open();
            Contract.Assert(this._getDataCommand != null);
            try {
                OleDbDataReader result = _getDataCommand.ExecuteReader();
                return result;
            }
            catch (OleDbException e)
            {
                if (e.ErrorCode == unchecked((int)0x80040e10))
                {
                    throw new ItemSourceException("Missing Columns in Item Source", e);
                }
                throw;
            }
        }

        /// <summary>
        /// get the number of items in this table
        /// </summary>
        /// <returns>the number of items</returns>
        public int GetItemCount()
        {
            this.Open();
            int result = (int)_getCountCommand.ExecuteScalar();
            return result;
        }
        
        /// <summary>
        /// Mark and item as visited/processed
        /// </summary>
        /// <param name="item">the item to mark as processed</param>
        /// <param name="status">the processed status to mark the item with</param>
        public int Visited(ItemReference item, string status)
        {
            if (!(item != null))
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!(status != null))
            {
                throw new ArgumentNullException(nameof(status));
            }

            Contract.EndContractBlock();

            this._visitItemCommand.Parameters.Clear();
            OleDbParameter statusParameter = new OleDbParameter { ParameterName = "status", Value = status };
            OleDbParameter idParameter = new OleDbParameter { ParameterName = "id", Value = Convert.ChangeType(item.Id, this._keyType) };
            this._visitItemCommand.Parameters.Add(statusParameter);
            this._visitItemCommand.Parameters.Add(idParameter);
            ////this.visitItemCommand.Prepare(); cannot prepare if we are not yet sure of idParameter type

            int updated = this._visitItemCommand.ExecuteNonQuery();
            Contract.Assert(updated > 0);
            return updated;
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
        /// Gets or sets the table name
        /// </summary>
        /// <returns>Gets or sets the name of the table of items from the source</returns>
        public virtual string TableName
        {
            get { return this._tableName; }
            set { this._tableName = value; }
        }


        /// <summary>
        /// standard Dispose pattern for disposing object with no unmanaged resources
        /// </summary>
        /// <param name="disposing">true when disposing of managed resources (false if called from GC)</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Close();
            }
        }

        /// <summary>
        /// Provide a new OleDBConnection to the file
        /// </summary>
        /// <returns>an OleDB connection to the Access file</returns>
        protected virtual OleDbConnection OpenConnection()
        {
            if (!(this.Filename != null))
            {
                throw new InvalidOperationException("Filename required");
            }

            Contract.Ensures(Contract.Result<OleDbConnection>() != null);
            Contract.EndContractBlock();

            OleDbConnection returnValue = new OleDbConnection();
            return returnValue;
        }

        /// <summary>
        /// an open connection should have the associated commands available
        /// </summary>
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._connection == null || (this._visitItemCommand != null && this._getDataCommand != null));
        }
    }
}
