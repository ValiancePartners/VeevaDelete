//-----------------------------------------------------------------------
// <copyright file="StatusTicks.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
// <summary>
// This file contains StatusTicks class.
// </summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VeevaDeleteApi
{
    /// <summary>
    /// Class to record success and failure count
    /// </summary>
    public class StatusTicks
    {
        /// <summary>
        /// Gets or sets the status SuccessCount
        /// </summary>
        public long SuccessCount { get; set; }

        /// <summary>
        /// Gets or sets the status FailureCount
        /// </summary>
        public long FailureCount { get; set; }
    }
}
