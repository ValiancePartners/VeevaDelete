//-----------------------------------------------------------------------
// <copyright file="IItemWalker.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using RestUtility.Api;

namespace VeevaDeleteApi
{
    /// <summary>
    /// Interface to support processing a list of items
    /// </summary>
    [ContractClass(typeof(ItemWalkerContract))]
    public interface IItemWalker : IDisposable
    {
        /// <summary>
        /// Gets or sets the table name
        /// </summary>
        /// <returns>Gets or sets the name of the table of items from the source</returns>
        string TableName { get; set; }

        /// <summary>
        /// Gets the data reader for the source
        /// </summary>
        /// <returns>the data reader for the source</returns>
        IDataReader GetReader();

        /// <summary>
        /// get the number of items in the list
        /// </summary>
        /// <returns>the number of items listed in the source</returns>
        int GetItemCount();
    }

    /// <summary>
    /// Contract class for IItemVisitor for non-null return value constraint
    /// </summary>
    [ContractClassFor(typeof(IItemWalker))]
    internal abstract class ItemWalkerContract : IItemWalker
    {
        /// <summary>
        /// get the number of items in this table
        /// </summary>
        /// <returns>the number of items</returns>
        public int GetItemCount()
        {
            return 0;
        }

        /// <summary>
        /// get the number of items in this table
        /// </summary>
        /// <returns>the number of items</returns>
        public virtual string TableName {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
                return null;
            }
            set { }
        }

        public virtual DeletionRequest.ItemType ItemType 
        {
            get{ return null; }
            set { }
        }


        /// <summary>
        /// get the data reader
        /// </summary>
        /// <returns>the file as a DataTable</returns>
        public IDataReader GetReader()
        {
            Contract.Ensures(Contract.Result<IDataReader>() != null);
            return null;
        }

        /// <summary>
        /// standard dispose implementation: call protected dispose in disposing mode
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
