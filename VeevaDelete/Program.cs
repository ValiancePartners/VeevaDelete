//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
// <summary>
// This file contains Program class.
// </summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace VeevaDelete
{
    /// <summary>
    /// Vault deletion tool main entry point
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            /* Always upgrades...
            if (Properties.Settings.Default.UpdateRequired)
            {
                Properties.Settings.Default.Upgrade();
                MessageBox.Show("Upgrading Settings");
                Properties.Settings.Default.UpdateRequired = false;
            } */
            Application.Run(new VeevaDelete());
        }
    }
}
