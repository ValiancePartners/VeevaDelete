//-----------------------------------------------------------------------
// <copyright file="IItemVisitor.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Text;

namespace VeevaDeleteApi
{
    /// <summary>
    /// Interface to support processing a list of items and optionally marking them as processed for updatable sources
    /// </summary>
    [ContractClass(typeof(ItemVisitorContract))]
    public interface IItemVisitor
    {
        /// <summary>
        /// open the database to walk and visit
        /// </summary>
        void Open();

        /// <summary>
        /// close the database
        /// </summary>
        void Close();

        /// <summary>
        /// mark the item as visited in the source
        /// </summary>
        /// <param name="itemReference">reference to the item to be marked</param>
        /// <param name="status">the status to mark the item with</param>
        int Visited(ItemReference itemReference, string status);
    }

    /// <summary>
    /// Contract class for IItemVisitor class for null argument checking
    /// </summary>
    [ContractClassFor(typeof(IItemVisitor))]
    internal abstract class ItemVisitorContract : IItemVisitor
    {
        /// <summary>
        /// Mark and item as visited/processed
        /// </summary>
        /// <param name="item">the item to mark as processed</param>
        /// <param name="status">the processed status to mark the item with</param>
        public int Visited(ItemReference item, string status)
        {
            ////if (!(item != null)) throw new ArgumentNullException(nameof(item));
            ////if (!(status != null)) throw new ArgumentNullException(nameof(status));
            ////Contract.EndContractBlock();
            return 0;
        }

        /// <summary>
        /// open the file
        /// </summary>
        public void Open()
        {
        }

        /// <summary>
        /// close the file
        /// </summary>
        public void Close()
        {
        }
    }
}
