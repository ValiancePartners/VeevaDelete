//-----------------------------------------------------------------------
// <copyright file="AccessTableItemWalker.cs" company="Valiance Partners">
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
    /// Implementation of OleTableItemWalker for Access files
    /// </summary>
    public class AccessTableItemWalker : OleTableItemWalker
    {
        /// <summary>
        /// Provide the set of restrictions for finding the first table in an access file
        /// </summary>
        protected override string[] TableTypeRestrictions
        {
            get
            {
                string[] restrictions = new string[4] { null, null, null, "TABLE" };
                return restrictions;
            }
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
            string accessConnectionString =
                        "Provider=Microsoft.ACE.OLEDB.12.0;Data Source= " + this.Filename;
            OleDbConnection returnValue = new OleDbConnection(accessConnectionString);
            return returnValue;
        }
    }
}
