//-----------------------------------------------------------------------
// <copyright file="VaultItemWalker.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using RestUtility.Api;
using RestUtility.VeevaVaultXml;
using TextTable.Reader;

namespace VeevaDeleteApi
{
    /// <summary>
    /// Implementation of IItemWalker for Vault data
    /// </summary>
    public sealed class VaultItemWalker : IItemWalker
    {
        /// <summary>
        /// Gets or sets the vault connection
        /// </summary>
        public IRestConnection Connection { get; set; }

        /// <summary>
        /// Gets or sets the vault query
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Gets the table of items
        /// </summary>
        /// <returns>the table of items from the source</returns>
        public IDataReader GetReader()
        {
            Contract.Assume(this.Connection != null, "Vault Connection Required");
            Contract.Assume(this.Query != null, "Vault Query Required");

            IDataReader returnValue = new VaultXmlDataReader(this.Connection, this.Query);

            return returnValue;
        }

        /// <summary>
        /// get the number of items in the list
        /// </summary>
        /// <returns>the number of items listed in the source</returns>
        public int GetItemCount()
        {
            Contract.Assume(this.Connection != null, "Vault Connection Required");
            Contract.Assume(this.Query != null, "Vault Query Required");

            string vql = this.Query;
            return this.Connection.GetCount(vql);
        }

        /// <summary>
        /// Gets the table of items
        /// </summary>
        /// <returns>the table of items from the source</returns>
        public string TableName
        {
            get { return Constants.QueryTableName; }
            set { }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
