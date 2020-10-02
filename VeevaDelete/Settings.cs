//-----------------------------------------------------------------------
// <copyright file="Settings.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
// <summary>
// This file contains Settings class.
// </summary>
//-----------------------------------------------------------------------
using System.Windows.Forms;

namespace VeevaDelete.Properties
{
    /// <summary>
    /// This class allows you to handle specific events on the settings class:
    ///  The SettingChanging event is raised before a setting's value is changed.
    ///  The PropertyChanged event is raised after a setting's value is changed.
    ///  The SettingsLoaded event is raised after the setting values are loaded.
    ///  The SettingsSaving event is raised before the setting values are saved.
    /// </summary>
    internal sealed partial class Settings
    {
        /// <summary>
        /// record if any of the settings where changed
        /// </summary>
        private bool _dirty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings" /> class.
        /// </summary>
        public Settings()
        {
            // add event handlers for saving and changing settings
            this.SettingChanging += this.SettingChangingEventHandler;
            this.SettingsSaving += this.SettingsSavingEventHandler;
        }

        /// <summary>
        /// Gets a value indicating whether any of the settings have changed
        /// </summary>
        public bool Dirty
        {
            get { return this._dirty; }
        }

        /// <summary>
        /// Handler to respond to a setting being changed
        /// </summary>
        /// <param name="sender">the source of the event</param>
        /// <param name="e">the details of the setting being changed</param>
        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e)
        {
            // Add code to handle the SettingChangingEvent event here.
            this._dirty = true;
        }

        /// <summary>
        /// Handler to confirm if settings should be saved, for example on close
        /// </summary>
        /// <param name="sender">the source of the event</param>
        /// <param name="e">the event data, allowing the save event to be cancelled</param>
        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Add code to handle the SettingsSaving event here.
            this._dirty = false; // TODO: what happens if save fails???
        }
    }
}
