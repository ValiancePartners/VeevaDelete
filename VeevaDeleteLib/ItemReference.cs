//-----------------------------------------------------------------------
// <copyright file="ItemReference.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
// <summary>
// This file contains the ItemReference class.
// </summary>
//-----------------------------------------------------------------------
using System;
using System.Diagnostics.Contracts;

namespace VeevaDeleteApi
{
    /// <summary>
    /// Vault item identifier
    /// </summary>
    public class ItemReference : IEquatable<ItemReference>
    {
        /// <summary>
        /// Gets or sets the ID
        /// </summary>
        /// <returns>the ID</returns>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the id refers to a particular version
        /// </summary>
        /// <returns>the WithVersion indicator</returns>
        public bool WithVersion { get; set; }

        /// <summary>
        /// Gets or sets the major version umber
        /// </summary>
        /// <returns>the major version number</returns>
        public string MajorVersion { get; set; }

        /// <summary>
        /// Gets or sets the minor version umber
        /// </summary>
        /// <returns>the minor version number</returns>
        public string MinorVersion { get; set; }

        /// <summary>
        /// polymorphic value comparison
        /// </summary>
        /// <param name="obj">object to compare to</param>
        /// <returns>true if the object is the same object or is of the same type and has the same value comparison</returns>
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

            bool returnValue = obj.GetType() == this.GetType() && this.Equals((ItemReference)obj);
            return returnValue;
        }

        /// <summary>
        /// implement value comparison
        /// </summary>
        /// <param name="p">the Item Reference to compare to</param>
        /// <returns>true if the item reference has the same value</returns>
        public bool Equals(ItemReference p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            bool returnValue =
                this.Id == p.Id &&
                this.WithVersion == p.WithVersion &&
                this.MajorVersion == p.MajorVersion &&
                this.MinorVersion == p.MinorVersion;
            return returnValue;
        }

        /// <summary>
        /// calculate a hash code for the item reference for sorting
        /// </summary>
        /// <returns>the ID hash code: The ID would normally vary more than the other values</returns>
        public override int GetHashCode()
        {
            int returnValue = this.Id.GetHashCode();
            return returnValue;
        }

        /// <summary>
        /// Convert to a string representation. Primarily useful for testing and debugging.
        /// </summary>
        /// <returns>String representing the content</returns>
        public override string ToString()
        {
            if (!this.WithVersion)
            {
                return this.Id.ToString();
            }

            Contract.Assume(this.MajorVersion != null, "Major Version Missing");
            Contract.Assume(this.MinorVersion != null, "Minor Version Missing");

            string[] parts = new string[]
            {
                    this.Id.ToString(),
                    this.MajorVersion.ToString(),
                    this.MinorVersion.ToString()
            };
            string returnValue = string.Join("/", parts);
            return returnValue;
        }
    }
}
