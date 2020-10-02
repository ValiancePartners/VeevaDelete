//-----------------------------------------------------------------------
// <copyright file="Constants.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
// <summary>
// This file contains Constants class.
// </summary>
//-----------------------------------------------------------------------
using System;

namespace VeevaDeleteApi
{
    /// <summary>
    /// Project Constants
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// name of table to show to user for VQL Query
        /// </summary>
        public static readonly string QueryTableName = "VQL Query";

        /// <summary>
        /// Possible ContextType for selecting entries to delete
        /// </summary>
        public enum SourceType
        {
            /// <summary>
            /// Get list from data target using a query
            /// </summary>
            Query,

            /// <summary>
            /// Get list from CSV file
            /// </summary>
            Csv,

            /// <summary>
            /// Get list from first sheet in excel workbook
            /// </summary>
            Excel,

            /// <summary>
            /// Get list from first table in access database
            /// </summary>
            Access,

            /// <summary>
            /// The number of values in the enumeration.
            /// </summary>
            TypeCount
        }
    }
}
