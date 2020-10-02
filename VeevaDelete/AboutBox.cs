//-----------------------------------------------------------------------
// <copyright file="AboutBox.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace VeevaDelete
{
    /*  Custom attributes are processed with the AssemblyDescription property
     * TRUconsole™ © 2013-2014 Valiance Partners
     * 
     * The image for this panel is in c:\work\graphics\pics\truConsoleabout.png
     * edit the date with Calibri 16 pt font
     * 
     * Also update the installer splash screen graphic, Calibri 10 pt I think (SplashScrTRUcomp-LOGOS-2008.jpg).
     */

    // custom attribute info source http://stackoverflow.com/questions/1936953/custom-assembly-attributes
    // this is neat: http://stackoverflow.com/questions/62353/what-are-the-best-practices-for-using-assembly-attributes?rq=1
    public partial class AboutBox : Form
    {
        #region Declarations

        #endregion Declarations

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutBox"/> class.
        /// This is an information window so dialog result is not meaningful.
        /// </summary>
        public AboutBox()
        {
            this.InitializeComponent();
            this.SetupAboutInformation();
        }

        #region Assembly Attribute Accessors

        /// <summary>
        /// Gets the assembly title information
        /// </summary>
        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != string.Empty)
                    {
                        return titleAttribute.Title;
                    }
                }

                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        /// <summary>
        /// Gets the assembly major.minor version number
        /// </summary>
        public string AssemblyVersion
        {
            get
            {
                // Mantis 1200 7/20/2015 DI : shorten the displayed version
                string vr = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                return vr;
            }
        }

        /// <summary>
        /// Gets the assembly description information
        /// </summary>
        public string AssemblyDescription
        {
            get
            {
                string result = string.Empty;
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    result = string.Empty;
                }
                else
                {
                    result += ((AssemblyDescriptionAttribute)attributes[0]).Description + Environment.NewLine;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the assembly product information
        /// </summary>
        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }

                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        /// <summary>
        /// Gets the assembly copyright information
        /// </summary>
        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }

                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        /// <summary>
        /// Gets the assembly company details
        /// </summary>
        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }

                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        #region Private methods

        // Mantis 1200 7/24/2015

        /// <summary>
        /// Fetch and display all the version information
        /// </summary>
        private void SetupAboutInformation()
        {
            this.Text = string.Format("About {0}", this.AssemblyTitle);
            this.labelProductName.Text = this.AssemblyProduct;
            this.labelVersion.Text = string.Format("Version {0}", this.AssemblyVersion);
            this.labelCopyright.Text = this.AssemblyCopyright;
            this.labelCompanyName.Text = this.AssemblyCompany;
            this.textBoxDescription.Text = this.AssemblyDescription;
        }

        #endregion Private methheads

    }
}
