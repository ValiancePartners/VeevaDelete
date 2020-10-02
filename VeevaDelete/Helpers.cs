//-----------------------------------------------------------------------
// <copyright file="Helpers.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VeevaDelete
{
    /// <summary>
    /// General class extensions utilities
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// execute an action affecting a UI control.
        /// If in the UI thread, execute the action directly, otherwise invoke it on the control
        /// </summary>
        /// <param name="control">the control to invoke the action on</param>
        /// <param name="action">the action to invoke on the control</param>
        public static void InvokeIfRequired(this Control control, Action action)
        {
            if (!(control != null))
            {
                throw new ArgumentNullException(nameof(control));
            }

            Contract.EndContractBlock();

            if (control.InvokeRequired)
            {
                control.Invoke(action);
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// Pluralize a noun according to how many there are
        /// </summary>
        /// <param name="singular">the noun</param>
        /// <param name="count">the number of items</param>
        /// <returns>the singular if count is 1, or the plural if count is not 1, where plural s the singluar plus 's'</returns>
        public static string Plurality(this string singular, long count, string plural=null)
        {
            string returnValue = count == 1 ? singular : singular + "s";
            return returnValue;
        }
    }
}
