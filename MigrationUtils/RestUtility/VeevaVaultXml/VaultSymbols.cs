// MP, 07/01/2019, Mantis 0001782, Vault Binder to Binder Links, merge into MigrationUtils repository
//-----------------------------------------------------------------------
// <copyright file="VaultSymbols.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
// <summary>
// This file contains the VaultSymbols class.
// </summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestUtility.VeevaVaultXml
{
    public class VaultSymbols
    {
        // MP, 09/16/2019, Mantis 0001817, Support Vault API Client ID, define client id prefix/suffix components
        /// <summary>
        /// symbolic name for response object
        /// </summary>
        public static readonly string ClientIdCompanyPrefix = "valiance-";

        /// <summary>
        /// symbolic name for response object
        /// </summary>
        public static readonly string ClientIdVeevaDeleteSuffix = "client-veevadelete";
        // end changes by MP on 09/16/2019

        /// <summary>
        /// symbolic name for response object
        /// </summary>
        public static readonly string ResponseObject = "object";

        /// <summary>
        /// symbolic name for response object label
        /// </summary>
        public static readonly string ResponseLabel = "label";

        /// <summary>
        /// symbolic name for response object plural label
        /// </summary>
        public static readonly string ResponseLabelPlural = "label_plural";

        /// <summary>
        /// symbolic name for responseDetails
        /// </summary>
        public static readonly string ResponseDetails = "responseDetails";

        /// <summary>
        /// symbolic name for response total
        /// </summary>
        public static readonly string ResponseTotal = "total";

        /// <summary>
        /// symbolic name for response data
        /// </summary>
        public static readonly string ResponseDataRow = "data/row";
                        
        /// <summary>
        /// symbolic name for id field
        /// </summary>
        public static readonly string IdFieldName = "id";

        /// <summary>
        /// symbolic name for major version number field
        /// </summary>
        public static readonly string MajorVersionFieldName = "major_version_number__v";

        /// <summary>
        /// symbolic name for minor version number field
        /// </summary>
        public static readonly string MinorVersionFieldName = "minor_version_number__v";

    }
}
// end changes by MP on 07/01/2019