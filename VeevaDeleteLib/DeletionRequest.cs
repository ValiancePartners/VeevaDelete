//-----------------------------------------------------------------------
// <copyright file="RelationParameters.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
// <summary>
// This file contains RelationParameters class.
// </summary>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using RestUtility.Api;

namespace VeevaDeleteApi
{
    /// <summary>
    /// Object to describe what should be deleted
    /// </summary>
    public class DeletionRequest : IEnumerable<ItemReference>, IEnumerable
    {
        /// <summary>
        /// Gets or sets the item selection type
        /// </summary>
        /// <returns>the item selection type</returns>
        public Constants.SourceType RelationType { get; set; }

        /// <summary>
        /// Gets or sets the vault connection
        /// </summary>
        /// <returns>the vault connection</returns>
        public IRestConnection RelationConnection { get; set; }

        /// <summary>
        /// Gets or sets the item selection file source
        /// </summary>
        /// <returns>the item selection file source</returns>
        public string RelationFile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not a cascade delete should be performed
        /// </summary>
        public bool Cascade { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether bulk delete should be used rather than individual delete
        /// </summary>
        public bool Batched { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the batch size to use
        /// </summary>
        public int? BatchSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the number of milliseconds of time to wait for API responses
        /// </summary>
        public int? TimeoutLength { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the number of times to retry after an API request times out
        /// </summary>
        public int? TimeoutRetries { get; set; }

        /// <summary>
        /// Gets or sets the item selection text search
        /// </summary>
        /// <returns>the item selection filter</returns>
        public string SelectionKeywordSearch { get; set; }
        /// <summary>
        /// Gets or sets the item selection filter
        /// </summary>
        /// <returns>the item selection filter</returns>
        public string SelectionFilter { get; set; }

        /// <summary>
        /// Gets or sets the type of item to be deleted
        /// </summary>
        /// <returns>the type of item to delete</returns>
        public ItemType Type { get; set; }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// for every last stinking type, create a list of the items +found in the source ( VQL or file )
        /// </summary>
        /// <param name="connection">the vault connection</param>
        /// <param name="this">which items should be deleted</param>
        /// <returns>The list of Item References found in the source</returns>
        public IEnumerator<ItemReference> GetEnumerator()
        {
            if (!(this.RelationType != Constants.SourceType.Query || this.RelationConnection != null))
            {
                throw new ArgumentNullException(nameof(RelationConnection));
            }
            Contract.EndContractBlock();

            IEnumerator<ItemReference> returnValue = new ItemWalkHelper(this.GetItemWalker(), this.Type).GetEnumerator();
            return returnValue;
        }

        /// <summary>
        /// create an object to provdie a list of the items found in the source ( VQL or file )
        /// </summary>
        /// <returns>Object to generate the list of Item References for the source</returns>
        public IItemWalker GetItemWalker()
        {
            if (!(this.RelationType != Constants.SourceType.Query || this.RelationConnection != null))
            {
                throw new ArgumentNullException(nameof(RelationConnection));
            }

            Contract.EndContractBlock();

            Constants.SourceType selectType = this.RelationType;
            IItemWalker returnValue;
            if (selectType == Constants.SourceType.Query)
            {
                VaultItemWalker vaultwalker = new VaultItemWalker
                {
                    Query = this.GetItemQuery(),
                    Connection = this.RelationConnection
                };
                returnValue = vaultwalker;
            }
            else
            {
                IFileItemWalker itemfilewalker = DeleteHelper.GetFileWalker(this.RelationType, this.Type);
                itemfilewalker.Filename = this.RelationFile;
                returnValue = itemfilewalker;
            }

            return returnValue;
        }

        /// <summary>
        /// calculate the VQL query requried corresponding to a DeleteParameters set
        /// </summary>
        /// <returns>VQL query string to generate the list of Item References for the source</returns>
        public string GetItemQuery()
        {
            if (!(this.RelationConnection != null))
            {
                throw new ArgumentNullException(nameof(this.RelationConnection));
            }
            if (!(this.RelationType == Constants.SourceType.Query))
            {
                throw new ArgumentOutOfRangeException(nameof(this.RelationType));
            }
            //AK, 10/1/2020, adding Archived Documents type
            if (!(this.Type.Category == DeletionRequest.ItemCategory.Object ||
                 this.Type.Category == DeletionRequest.ItemCategory.Binder ||
                 this.Type.Category == DeletionRequest.ItemCategory.Document ||
                 this.Type.Category == DeletionRequest.ItemCategory.Archived_Document))
            {
                throw new ArgumentOutOfRangeException(nameof(this.Type.Category));
            }
            if (!(this.Type.Category != DeletionRequest.ItemCategory.Object ||
                !string.IsNullOrEmpty(this.Type.ObjectName)))
            {
                throw new ArgumentNullException(nameof(this.Type.ObjectName));
            }

            Contract.EndContractBlock();

            string returnValue = string.Empty;
            string source;
            string typeFilter;
            switch (this.Type.Category)
            {
                case DeletionRequest.ItemCategory.Binder:
                    if (this.RelationConnection.ApiVersion >= 18.2M)
                    {
                        source = "binders";
                        typeFilter = string.Empty;
                    }
                    else
                    { 
                        source = "documents";
                        typeFilter = "binder__v = true";
                    }
                    break;
                //AK, 10/1/2020, adding archived documents
                case DeletionRequest.ItemCategory.Archived_Document:
                    source = "archived_documents";
                    typeFilter = "binder__v = false";
                    break;
                    //end changes by AK on 10/1/2020
                case DeletionRequest.ItemCategory.Document:
                    source = "documents";
                    typeFilter = "binder__v = false";
                    break;

                case DeletionRequest.ItemCategory.Object:
                    source = this.Type.ObjectName;
                    typeFilter = string.Empty;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(this.Type.Category));
            }
            string versionFilter = string.Empty;
            if (!string.IsNullOrWhiteSpace(this.Type.MajorVersion))
            {
                versionFilter = string.Format("major_version_number__v = {0} and minor_version_number__v = {1}",
                    this.Type.MajorVersion,
                    this.Type.MinorVersion
                    );
            }

            string projection = "id";
            if (this.Type.WithVersion)
            {
                projection += ", major_version_number__v, minor_version_number__v";
            }

            IEnumerable<string> filters =
                (new string[] { typeFilter, versionFilter, this.SelectionFilter }).
                Where(x => !string.IsNullOrWhiteSpace(x));

            string selection = string.Empty;
            if(!string.IsNullOrEmpty(this.SelectionKeywordSearch))
            {
                selection += " find (" + this.SelectionKeywordSearch +")";
            }
            if (filters.Any())
            {
                selection += " where " + string.Join(" and ", filters);
            }          

            returnValue = $"select {projection} from {source}{selection}";
            return returnValue;
        }

        /// <summary>
        /// Type of vault items
        /// </summary>
        public enum ItemCategory
        {
            /// <summary>
            /// Document: ordinary vault document
            /// </summary>
            Document,

            //AK, 10/1/2020, add Archived Doc
            Archived_Document,
            //end changes by AK on 10/1/2020

            /// <summary>
            /// Binder: vault binder document
            /// </summary>
            Binder,

            /// <summary>
            /// Object: vault virtual object
            /// </summary>
            Object
               
        }

        /// <summary>
        /// Object to describe the type of item to be deleted
        /// </summary>
        public class ItemType
        {
            /// <summary>
            /// Gets or sets the item type
            /// </summary>
            /// <returns>the item type</returns>
            public ItemCategory Category { get; set; }

            /// <summary>
            /// Gets or sets the object type name
            /// </summary>
            /// <returns>the object type type</returns>
            public string ObjectName { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether to delete a specific version or all versions
            /// </summary>
            /// <returns>true if one version should be deleted</returns>
            public bool WithVersion { get; set; }

            /// <summary>
            /// Gets or sets the major version
            /// </summary>
            /// <returns>the major version</returns>
            public string MajorVersion { get; set; }

            /// <summary>
            /// Gets or sets the minor version
            /// </summary>
            /// <returns>the minor version</returns>
            public string MinorVersion { get; set; }

            /// <summary>
            /// Get the object details as a string, for display
            /// </summary>
            /// <returns>a string describing the object</returns>
            public override string ToString()
            {
                string returnValue = this.Category == ItemCategory.Object ? this.ObjectName : this.Category.ToString() +
                    (this.WithVersion ? " (all versions)" :
                    string.IsNullOrEmpty(this.MajorVersion) ? "(one version)" :
                    "(version " + this.MajorVersion + "." + this.MinorVersion + ")");
                return returnValue;
            }
        }
    }
}
