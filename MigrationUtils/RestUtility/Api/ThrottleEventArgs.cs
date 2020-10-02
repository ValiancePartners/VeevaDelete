// MP, 07/01/2019, Mantis 0001782, Vault Binder to Binder Links, merge into MigrationUtils repository
//-----------------------------------------------------------------------
// <copyright file="ThrottleEventArgs.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
// <summary>
// This file contains ThrottleEventArgs class.
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
    public class ThrottleEventArgs : LimitEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottleEventArgs" /> class.
        /// </summary>
        /// <param name="resumeAt">the expected resumption time</param>
        /// <param name="burstLimit">the API burst limit</param>
        /// <param name="dailyLimit">the API daily limit</param>
        public ThrottleEventArgs(DateTime resumeAt, long? burstLimit, long? dailyLimit) :
            base(burstLimit, dailyLimit)
        {
            this.ResumeAt = resumeAt;
        }

        /// <summary>
        /// Gets the time at which the task will be resumed after throttling
        /// </summary>
        public DateTime ResumeAt { get; private set; }
    }
}
// end changes by MP on 07/01/2019