//-----------------------------------------------------------------------
// <copyright file="TRUMigrateException.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;

namespace TRUmigrate_1
{
	/// <summary>
	/// Summary description for TRUCompareException.
	/// </summary>
	public class TRUMigrateException:Exception
	{
        public string description = "";
        public Exception ex = null;

        public TRUMigrateException()
        {

        }

        // MP, 8/23/2018, Mantis #0001610 - include details in 'TRUmigrate_1.TRUMigrateException' error message [SF601]
        public TRUMigrateException(string errDesc) : this(errDesc, null)
        {

        }


        //end changes by MP on 8/23/2018
        public TRUMigrateException(string errDesc, Exception e)
            // MP, 8/23/2018, Mantis #0001610 - include details in 'TRUmigrate_1.TRUMigrateException' error message [SF601]
            : base(errDesc, e)
        //end changes by MP on 8/23/2018
        {
            description = errDesc;
            ex = e;
            // MP, 8/23/2018, Mantis #0001610 - include details in 'TRUmigrate_1.TRUMigrateException' error message [SF601]
            //
            //end changes by MP on 8/23/2018
        }



    }

}