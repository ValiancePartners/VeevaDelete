// MP, 07/01/2019, Mantis 0001782, Vault Binder to Binder Links, merge into MigrationUtils repository
//-----------------------------------------------------------------------
// <copyright file="LimitEventArgs.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
// <summary>
// This file contains LimitEventArgs class.
// </summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RestUtility.Api
{
    /// <summary>
    /// Vault limit event arguments
    /// </summary>
    public class LimitEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LimitEventArgs" /> class.
        /// </summary>
        /// <param name="burstLimit">the API burst limit</param>
        /// <param name="dailyLimit">the API daily limit</param>
        public LimitEventArgs(long? burstLimit, long? dailyLimit)
        {
            // TODO: Complete member initialization
            this.BurstLimit = burstLimit;
            this.DailyLimit = dailyLimit;
        }

        /// <summary>
        /// Gets the API burst limit associated with the vault
        /// </summary>
        public long? BurstLimit { get; private set; }

        /// <summary>
        /// Gets the API daily limit associated with the vault
        /// </summary>
        public long? DailyLimit { get; private set; }
    }
}
// end changes by MP on 07/01/2019