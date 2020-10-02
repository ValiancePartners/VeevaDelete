// MP, 10/24/2019, Mantis 0001835, Add timeout handling
//-----------------------------------------------------------------------
// <copyright file="TimeoutEventArgs.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
// <summary>
// This file contains TimeoutEventArgs class.
// </summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RestUtility.Api
{
    /// <summary>
    /// Vault limit event arguments
    /// </summary>
    public class TimeoutEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeoutEventArgs" /> class.
        /// </summary>
        /// <param name="burstLimit">the retry number</param>
        public TimeoutEventArgs(int retry, bool cancel=false) : base(cancel)
        {
            this.Retry = retry;
        }

        public readonly int Retry;
    }
}
// end changes by MP on 10/24/2019