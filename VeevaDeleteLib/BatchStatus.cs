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
    public class BatchItemDeleteStatus : IEquatable<BatchItemDeleteStatus>
    {
        /// <summary>
        /// Gets or sets a status message
        /// </summary>
        public string Message { get; set; } = null;

        /// <summary>
        /// Gets or sets the ItemSuccessStatus indicator for an individual item
        /// </summary>
        public bool? ItemSuccessStatus { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            bool returnValue = (obj as BatchItemDeleteStatus)?.Equals(this) ?? false;
            return returnValue;
        }

        /// <summary>
        /// implement value comparison
        /// </summary>
        /// <param name="p">the SatusTicks to compare to</param>
        /// <returns>true if the the parameter has the same value</returns>
        public bool Equals(BatchItemDeleteStatus p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            bool returnValue =
                this.Message == p.Message &&
                this.ItemSuccessStatus== p.ItemSuccessStatus;
            return returnValue;
        }

        /// <summary>
        /// calculate a hash code for the item reference for sorting
        /// </summary>
        /// <returns>combine succes & failure count</returns>
        public override int GetHashCode()
        {
            int returnValue = (this.ItemSuccessStatus.HasValue? ItemSuccessStatus.Value ? 1 : 0 :1)| 
                this.Message.GetHashCode();
            return returnValue;
        }

        public override string ToString()
        {
            return string.Format("Item Success:{0}; {1}", this.ItemSuccessStatus, this.Message);
        }
    }
}
