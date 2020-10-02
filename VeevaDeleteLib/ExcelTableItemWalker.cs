//-----------------------------------------------------------------------
// <copyright file="ExcelTableItemWalker.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace VeevaDeleteApi
{
    /// <summary>
    /// Implementation of OleTableItemWalker for Excel files
    /// </summary>
    public class ExcelTableItemWalker : OleTableItemWalker
    {
        /// <summary>
        /// Provide the set of restrictions for finding the first table in an excel file
        /// </summary>
        protected override string[] TableTypeRestrictions
        {
            get { return null; }
        }


        /// <summary>
        /// Provide a new OleDBConnection to this Access file
        /// </summary>
        /// <returns>an OleDB connection to the Access file</returns>
        protected override OleDbConnection OpenConnection()
        {
            if (!(this.Filename != null))
            {
                throw new InvalidOperationException("Filename required");
            }

            Contract.Ensures(Contract.Result<OleDbConnection>() != null);

            Contract.EndContractBlock();
            string excelConnectionString =
                    "Provider=Microsoft.ACE.OLEDB.12.0;Data Source= " + this.Filename + ";Extended Properties=\"Excel 12.0 XML;HDR=YES\"";
            OleDbConnection returnValue = new OleDbConnection(excelConnectionString);
            return returnValue;
        }

        /// <summary>
        /// Gets or sets the table name
        /// </summary>
        /// <returns>Gets or sets the name of the table of items from the source</returns>
        public override string TableName
        {
            get { return base.TableName; }
            set {
                bool needDollar = !string.IsNullOrEmpty(value) && value[value.Length - 1] != '$';
                base.TableName = needDollar? value + "$" : value ;
            }
        }


    }
}
