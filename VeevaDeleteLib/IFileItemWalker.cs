//-----------------------------------------------------------------------
// <copyright file="IFileItemWalker.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace VeevaDeleteApi
{
    /// <summary>
    /// Base abstract class for walkers that process a file of items
    /// </summary>
    public interface IFileItemWalker : IItemWalker
    {
        /// <summary>
        /// Sets the file to be walked
        /// </summary>
        string Filename { set; }

        DeletionRequest.ItemType ItemType { get; set; }

    }
}
