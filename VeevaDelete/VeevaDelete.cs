//-----------------------------------------------------------------------
// <copyright file="VeevaDelete.cs" company="Valiance Partners">
// Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;        /// <param name="count">the number of items</param>
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using RestUtility.Api;
using RestUtility.VeevaVaultWeb;
using RestUtility.VeevaVaultXml;
using TextTable.Reader;
using VeevaDeleteApi;

namespace VeevaDelete
{
    using IntPair = Tuple<int, int>;
    using StringPair = Tuple<string, string>;
    /*  Revision Log.
     *
     *  This application was originally developed by Bharadwaz, an intern during the summer adn fall of 2017.
     *  When using an Excel file for input, we only look on the first worksheet/tab for document ids.
     *
     *  10/24/2017 DI : Changed the size of the type box to add the 'Parent' object option.
     *   This will use the Api for parent-grandparent deletes:
     *   /api/{version}/vobjects/{object_name}/{object_record_id}/actions/cascadedelete
     *   Dangerous: will only support Excel/Access where deletion is recorded; two columns, first row is names
     *   Where my files at??? C:\Work\VeevaDelete\VeevaDelete\bin\Debug\Documents
     */

    /// <summary>
    /// Main UI Form for Delete Tool
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA2213: Disposable fields should be disposed", Justification = "fields are disposed in disposed event")]
    public partial class VeevaDelete : Form
    {
        //// <summary>
        //// the width of the form with the
        //// </summary>
        ////private readonly int displayLogWidth = 1162, hideLogWidth = 590;

        private static Regex versionPattern = new Regex(@"^(\d+).(\d+)$");

        /// <summary>
        /// the value entered for the burst manual reserve
        /// </summary>
        private long? burstReserveOpt;

        /// <summary>
        /// the value entered for the burst manual reserve
        /// </summary>
        private long? dailyReserveOpt;

        /// <summary>
        /// the value entered for the major version number
        /// </summary>
        private long? majorVersionOpt;

        /// <summary>
        /// the value entered for the minor version number
        /// </summary>
        private long? minorVersionOpt;

        /// <summary>
        /// the number of records to include in each deletion batch
        /// </summary>
        private int? batchSizeOpt;

        /// <summary>
        /// the number of minutes to wait for a response
        /// </summary>
        private int? timeoutMinutesOpt;

        /// <summary>
        /// the number of times to retry an API Request where the request times out
        /// </summary>
        private int? timeoutRetriesOpt;

        /// <summary>
        /// indicates if any settings have been changed
        /// </summary>
        private bool dirty;

        /// <summary>
        /// indicates if a deletion is in progress
        /// </summary>
        private bool running;

        /// <summary>
        /// the width of the log panel, for expanding/collapsing the form along with the panel
        /// </summary>
        private int logPanelWidth;

        /// <summary>
        /// the file trace logger to log progress messages
        /// </summary>
        private Logger logUtil;

        /// <summary>
        /// hook to stop a deletion while in progress
        /// </summary>
        private CancellationTokenSource tokenSource;

        /// <summary>
        /// the progress bar
        /// </summary>
        private IProgress<int> progressReporter;

        /// <summary>
        /// the textbox trace logger to log progress messages
        /// </summary>
        private TextBoxTraceListener txtListener;

        /// <summary>
        /// avoid race condition on killing text listener
        /// </summary>
        private volatile int _killingTextListener;

        /// <summary>
        /// avoid race condition on killing listeners
        /// </summary>
        private volatile int _killingAllListeners;

        private int SuccessCount;

        private int FailureCount;

        private DeletionRequest currentDeletionRequest;

        /// <summary>
        ///  Initializes a new instance of the <see cref="VeevaDelete" /> class.
        /// </summary>
        public VeevaDelete()
        {
            this.Font = this.Font = System.Drawing.SystemFonts.MessageBoxFont;
            this.InitializeComponent();
            foreach (Control c in this.Controls)
            {
                c.Font = System.Drawing.SystemFonts.MessageBoxFont;
            }
            ////this.Size = new Size(this.hideLogWidth, this.Size.Height);
            this.HideLog(true);
            this.progressReporter = new Progress<int>(this.ReportProgress);
            // don't do this: the numbers are fake and riskmisleading the user.
            ////this.UpdateLimits(this, new LimitEventArgs(200,100000));
        }

        /// <summary>
        /// check if an enabled checkbox has text
        /// </summary>
        /// <param name="tbx">the textbox to check</param>
        /// <returns>true unless the textbox is enabled and empty</returns>
        private static bool HasRequiredText(TextBox tbx)
        {
            bool returnValue = !tbx.Enabled || !string.IsNullOrEmpty(tbx.Text);
            return returnValue;
        }

        /// <summary>
        /// Show Delete or Stop button
        /// </summary>
        /// <param name="hide">true if the log panel should be hidden, false if it should be displayed</param>
        private void EnableStop(bool running)
        {
            this.running = running;
            if (running)
            {
                this.StartStop.Text = "Stop";
            }
            else
            {
                this.StartStop.Text = "Delete";
            }
        }

        /// <summary>
        /// respond to the Hide or show log button click
        /// </summary>
        /// <param name="sender">the source of the event</param>
        /// <param name="e">the event details</param>
        private void DisplayLog_Click(object sender, EventArgs e)
        {
            ////(this.Size.Width == this.displayLogWidth)
            this.HideLog(!this.splitContainer.Panel2Collapsed);
        }

        /// <summary>
        /// Hide or show the log details
        /// </summary>
        /// <param name="hide">true if the log panel should be hidden, false if it should be displayed</param>
        private void HideLog(bool hide)
        {
            // collapse form before panel but expand panel before form to minimize flicker
            this.SuspendLayout();
            if (hide)
            {
                this.displayLog.Text = "Display Log >>";
                ////this.Size = new Size(this.hideLogWidth, this.Size.Height);
                this.logPanelWidth = this.splitContainer.Panel2.Width + this.splitContainer.SplitterWidth;
                this.Size = new Size(this.Size.Width - this.logPanelWidth, this.Size.Height);
                this.splitContainer.Panel2Collapsed = true;
                this.KillTextListener();
            }
            else
            {
                this.OpenTextListener();
                this.displayLog.Text = "Hide Log <<";
                ////this.Size = new Size(this.displayLogWidth, this.Size.Height);
                this.splitContainer.Panel2Collapsed = false;
                this.Size = new Size(this.Size.Width + this.logPanelWidth, this.Size.Height);
            }

            this.ResumeLayout();
        }

        /// <summary>
        /// check the vault connection
        /// </summary>
        /// <param name="sender">the event source</param>
        /// <param name="e">the event details</param>
        private void TestConnection_Click(object sender, EventArgs e)
        {
            if (this.ValidateGroup(this.connection))
            {
                this.Cursor = Cursors.WaitCursor;
                try
                {
                    this.StartConnection();
                    this.UserAdvicePause("Connection Successful", MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    this.UserAdvicePause(
                        "Could not connect to the data source. Please check your connection parameters.\n" + ex.FormatError());
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        /// <summary>
        /// Get a new vault connector and wire in the limit update event handles to the form
        /// </summary>
        /// <returns>the new vault connector, with the wired limit events handlers</returns>
        private ConnectionUtility NewConnection()
        {
            ConnectionUtility connectionUtil = new ConnectionUtility();
            this.SetupLimits(connectionUtil);
            return connectionUtil;
        }

        /// <summary>
        /// open a connection to the vault
        /// </summary>
        private void StartConnection()
        {
            ConnectionUtility connectionUtil = this.NewConnection();
            ////<<<<>>>>>
            connectionUtil.OpenConnection(this.GetRequestParams());
        }

        /// <summary>
        /// Get Request Parameters
        /// </summary>
        /// <returns>the array of request parameters</returns>
        private ConnectionParameters GetRequestParams()
        {
            ConnectionParameters returnValue = new ConnectionParameters
            {
                Url = vaultUrl.Text.Trim(),
                Username = vaultUsername.Text.Trim(),
                Password = vaultPassword.Text,
                SubService = vaultApiVersion.Text.Trim(),
                ClientId = VaultSymbols.ClientIdCompanyPrefix + clientOrganisation.Text.Trim() + "-" + vaultProduct.Text.Trim() + VaultSymbols.ClientIdVeevaDeleteSuffix
            };
            return returnValue;
        }

        /// <summary>
        /// perform validation on controls in the group
        /// </summary>
        /// <param name="parentControl">the parent control containing the controls to validate</param>
        /// <returns>true if there are no validation errors</returns>
        private bool ValidateGroup(Control parentControl)
        {
            this.SuspendLayout();

            // clear any existing errors
            errorProvider.Clear();

            // validate our controls
            foreach (Control c in parentControl.Controls)
            {
                c.Focus();
            }

            Control firstErrorControl = null;

            // summarise the errors
            StringBuilder errorSummary = new StringBuilder();
            foreach (Control c in parentControl.Controls)
            {
                string error = errorProvider.GetError(c);
                if (error != string.Empty)
                {
                    errorSummary.AppendFormat("{0}{1}", errorProvider.GetError(c), Environment.NewLine);
                    firstErrorControl = c;
                }
            }

            this.ResumeLayout();
            if (firstErrorControl != null)
            {
                firstErrorControl.Focus();
                this.UserAdvicePause(errorSummary.ToString());
            }

            Application.DoEvents();
            return firstErrorControl == null;
        }

        /// <summary>
        /// check how many records have been selected for deletion
        /// </summary>
        /// <param name="sender">the event source</param>
        /// <param name="e">the event data</param>
        private void TestSelection_Click(object sender, EventArgs e)
        {
            if (this.ValidateSelection())
            {
                this.Cursor = Cursors.WaitCursor;
                int numOfRecs;
                //IntPair counts;
                try
                {
                    ConnectionUtility connUtil;
                    Constants.SourceType sourceType = this.GetItemListSourceType();
                    if (sourceType == Constants.SourceType.Query)
                    {
                        connUtil = this.NewConnection();
                        ConnectionParameters requestParameters = this.GetRequestParams();
                        connUtil.OpenConnection(requestParameters);
                    }
                    else
                    {
                        connUtil = null;
                    }
                    DeletionRequest deletionRequest = this.GetDeletionRequest(connUtil);
                    numOfRecs = deletionRequest.GetSelectCount();
                    //counts = connUtil.TestSelect(parameters, deletionRequest);
                }
                catch (Exception ex)
                {
                    this.UserAdvicePause(ex.FormatError());
                    return;
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }

                //int numOfRecs = counts.Item1;

                // int limitRecs = counts.Item2;
                /* we can delete all records 
                // Evaluate number of records and if LIMIT keyword exists for proper message.
                string userMsg = userMsg = numOfRecs + " records identified for deletion.";
                if (limitRecs > -1 && limitRecs < numOfRecs)
                {
                    userMsg = userMsg + Environment.NewLine + Environment.NewLine +
                                "Maximum number of records that can be processed is " + limitRecs + "." + Environment.NewLine +
                                "To overide max value use \"LIMIT\" keyword + desired value in expression.";
                }
                 */
                string userMsg = numOfRecs.ToString() + " record".Plurality(numOfRecs) + " identified for deletion.";

                this.UserAdvicePause(userMsg, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Strip trailing log extension from file name
        /// this is for handing the file over to the logger which adds the log extension to all file names
        /// </summary>
        /// <returns>the file name, less the log extension if this is the file extension</returns>
        private string StrippedLogFilePath()
        {
            string returnValue = Path.GetExtension(this.logFilePath.Text) == Logger.LogExtension ?
                Path.ChangeExtension(this.logFilePath.Text, string.Empty).TrimEnd('.') :
                this.logFilePath.Text;
            return returnValue;
        }

        /// <summary>
        /// add trailing log file extension if there isn't one there
        /// this is to ensure we have the same file name as will be use by the Logger when given the StrippedLogFilePath above
        /// </summary>
        /// <returns>the log file name including the log extension</returns>
        private string FullLogFilePath()
        {
            string returnValue =
                    string.IsNullOrEmpty(this.logFilePath.Text) ?
                    string.Empty :
                    Path.GetExtension(this.logFilePath.Text) == Logger.LogExtension ?
                    this.logFilePath.Text :
                    this.logFilePath.Text + Logger.LogExtension;
            return returnValue;
        }

        /// <summary>
        /// start deleting!
        /// </summary>
        /// <param name="sender">the event sender</param>
        /// <param name="e">the event data</param>
        private void StartStop_Click(object sender, EventArgs e)
        {
            if (running)
            {
                tokenSource.Cancel();
                return;
            }

            ////if (!this.ValidateCredentials())
            if (!this.ValidateChildren())
            {
                return;
            }

            if (File.Exists(this.FullLogFilePath()))
            {
                // Prompt user to overwrite existing log file.
                DialogResult dr = this.UserAdviceRequest(
                    this.FullLogFilePath() + " already exists. " + "Would you like to overwrite this file?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                // Delete existing file.
                if (dr != DialogResult.Yes)
                {
                    // Let user change file name.
                    this.logFilePath.Focus();
                    return;
                }
                else
                {
                    try
                    {
                        File.Delete(this.FullLogFilePath());
                    }
                    catch (Exception ex)
                    {
                        // Let user change file name or close an open file.
                        this.UserAdviceInputError("Cannot delete " + this.FullLogFilePath() + ". \r\n" + ex.FormatError(), this.logFilePath);
                        return;
                    }
                }
            }

            this.logUtil = new Logger(this.StrippedLogFilePath());
            this.SetupLinkLog();
            this.textLog.ResetText();
            this.OpenTextListener();

            ////txtListener.Owner = this.textLog;
            Trace.Listeners.Add(this.txtListener);

            // clean up resources properly if there is a problem starting the background process
            bool started = false;
            try
            {
                started = this.StartDelete();
            }
            finally
            {
                if (!started)
                {
                    this.DeleteFinished(false);
                }
            }
        }

        /// <summary>
        /// Warn user of input error and set the focus to the relevant control
        /// </summary>
        /// <param name="message">the error message</param>
        /// <param name="control">the control the error refers to</param>
        private void UserAdviceInputError(string message, Control control)
        {
            this.UserAdvicePause(message);
            control.Focus();
        }

        /// <summary>
        /// Notify user of error
        /// </summary>
        /// <param name="message">the error message</param>
        /// <param name="icon">the icon to display, defaults to Exclamation</param>
        private void UserAdvicePause(string message, MessageBoxIcon icon = MessageBoxIcon.Exclamation)
        {
            this.UserAdviceRequest(message, MessageBoxButtons.OK, icon);
        }

        /// <summary>
        /// Provide information to the user and wait for user response
        /// </summary>
        /// <param name="message">the message</param>
        /// <param name="buttons">the allowed responses</param>
        /// <param name="icon">the message icon</param>
        /// <returns>the user response</returns>
        private DialogResult UserAdviceRequest(
            string message,
            MessageBoxButtons buttons,
            MessageBoxIcon icon = MessageBoxIcon.None,
            MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
        {
            DialogResult returnValue = MessageBox.Show(this, message, this.Text, buttons, icon, defaultButton);
            return returnValue;
        }

        /// <summary>
        /// Stop the text box listener if still active
        /// </summary>
        private void StopListeners()
        {
            if (Interlocked.Increment(ref _killingAllListeners) == 1)
            {
                if (this.logUtil != null)
                {
                    this.logUtil.Dispose();
                    this.logUtil = null;
                }
                if (this.tokenSource != null)
                {
                    this.tokenSource.Dispose();
                    this.tokenSource = null;
                }
            }
            Interlocked.Decrement(ref _killingAllListeners);

            this.KillTextListener();
        }

        /// <summary>
        /// Teardown text box listener
        /// </summary>
        private void KillTextListener()
        {
            if (Interlocked.Increment(ref _killingTextListener) == 1)
            {
                if (this.txtListener != null)
                {
                    Trace.Listeners.Remove(this.txtListener);
                    this.txtListener.Close();
                    this.txtListener = null;
                }
            }
            Interlocked.Decrement(ref _killingTextListener);
        }

        /// <summary>
        /// Setup text box listener
        /// </summary>
        private void OpenTextListener()
        {
            if (this.txtListener == null)
            {
                this.txtListener = new TextBoxTraceListener(this.textLog);
            }
        }

        /// <summary>
        /// Setup vault limit handling
        /// </summary>
        /// <param name="c">The connection to vault</param>
        private void SetupLimits(ConnectionUtility c)
        {
            c.LimitUpdated += new EventHandler<LimitEventArgs>(
                (sender, limits) => this.InvokeIfRequired(() => this.UpdateLimits(sender, limits)));
            c.LimitThrottling += new EventHandler<ThrottleEventArgs>(this.LimitThrottling);
            c.LimitThrottled += new EventHandler<ThrottleEventArgs>(this.LimitThrottled);
            if (this.burstReserveOpt.HasValue)
            {
                c.BurstReserve = this.burstReserveOpt.Value;
            }

            if (this.dailyReserveOpt.HasValue)
            {
                c.DailyReserve = this.dailyReserveOpt.Value;
            }
        }

        /// <summary>
        /// Create deletion request from form input
        /// </summary>
        /// <returns>Deletion request object for data entered on the form
        private DeletionRequest GetDeletionRequest(IRestConnection connection)
        {
            Constants.SourceType type = this.GetItemListSourceType();
            string file = this.itemListFile.Text.Trim();
            string textfind = this.itemListKeywordSearch.Text.Trim();
            string filter = this.itemListFilter.Text.Trim();
            DeletionRequest.ItemCategory itemType = (DeletionRequest.ItemCategory)Enum.Parse(
                typeof(DeletionRequest.ItemCategory),
                this.GetGroupButtonTag(itemCategoryGroup),
                true);

            string objectName = this.objectName.Text;
            bool withVersion = this.withVersion.Checked;
            string majorVersion = this.majorVersionOpt.ToString();
            string minorVersion = this.minorVersionOpt.ToString();

            DeletionRequest deletionRequest = new DeletionRequest
            {
                RelationConnection = connection,
                RelationType = type,
                RelationFile = file,
                Cascade = cascade.Checked,
                Batched = batched.Checked,
                SelectionKeywordSearch = textfind,
                SelectionFilter = filter,
                BatchSize = this.batchSizeOpt,
                TimeoutLength = this.timeoutMinutesOpt.HasValue ? this.timeoutMinutesOpt.Value * 60000 : (int?)null,
                TimeoutRetries = this.timeoutRetriesOpt,
                Type = new DeletionRequest.ItemType
                {
                    Category = itemType,
                    WithVersion = withVersion,
                    ObjectName = objectName,
                    MajorVersion = majorVersion,
                    MinorVersion = minorVersion
                }
            };
            return deletionRequest;
        }

        /// <summary>
        /// start logged deletion process!
        /// </summary>
        /// <returns>true if process was started successfully</returns>
        private bool StartDelete()
        {
            string productLogHeader = $"Veeva Delete {Assembly.GetExecutingAssembly().GetName().Version.ToString()} Log";
            this.UpdateLogs(productLogHeader);

            this.UpdateLogs("*************Starting Deletion Process*************");
            ConnectionUtility connectionUtil = this.NewConnection();
            try
            {
                ////<<<<>>>>
                connectionUtil.OpenConnection(this.GetRequestParams());
            }
            catch (Exception ex)
            {
                this.UserAdvicePause(
                    "Could not connect to the data source. Please check your connection parameters and try again\n" + ex.FormatError(),
                    MessageBoxIcon.Exclamation);
                return false;
            }

            DeletionRequest deletionRequest = GetDeletionRequest(connectionUtil);

            try
            {
                int numOfRecs = deletionRequest.GetSelectCount();
                //IntPair testResult = connectionUtil.TestSelect(parameters, deletionRequest);
                //int numOfRecs = testResult.Item1;

                /*
                string recsToDel = numOfRecs.ToString();
                int limitRecs = testResult.Item2;
                if (limitRecs < numOfRecs && numOfRecs > 1000)
                {
                    recsToDel = limitRecs.ToString() + " of " + recsToDel;
                }*/

                StringPair labels = connectionUtil.GetUINames(deletionRequest.Type);

                if (numOfRecs == 0)
                {
                    this.UserAdvicePause(
                        string.Format("No {0} to delete", labels.Item2),
                            MessageBoxIcon.Information);
                    return false;
                }

                string versionDescription = deletionRequest.Type.GetVersionDescription();

                if (this.UserAdviceRequest(
                    string.Format(
                        "This will {0}delete {1}{2} {3}.\n" +
                        "Are you absolutely sure you want to delete {4} {3}?",
                        deletionRequest.Batched ? "batch " :
                        deletionRequest.Cascade ? "cascade " :
                        string.Empty,
                        versionDescription,
                        numOfRecs,
                        numOfRecs == 1 ? labels.Item1 : labels.Item2,
                        numOfRecs == 1 ? "this" : "these"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    // Start the delete process in background thread.
                    this.tokenSource = new CancellationTokenSource();
                    this.EnableStop(true);
                    this.Cursor = Cursors.WaitCursor;

                    // Update UI control.
                    labelProgress.Text = "Processing Path Deletion . . .";

                    this.BatchProgress.Minimum = 0;
                    this.BatchProgress.Step = 1;
                    this.BatchProgress.Value = 0;
                    this.BatchProgress.Maximum = numOfRecs;
                    this.SuccessCount = 0;
                    this.FailureCount = 0;
                    this.currentDeletionRequest = deletionRequest;
                    this.UpdateLogs("Process Started...", null, false);
                    this.UpdateLogs("Login credentials: ");
                    this.UpdateLogs("Vault: " + vaultUrl.Text);
                    this.UpdateLogs("Username: " + vaultUsername.Text);
                    this.UpdateLogs("Api Version: " + vaultApiVersion.Text);
                    this.StartCurrentRequest();
                    return true;
                }
                else
                {
                    // Log cancellation.
                    this.UpdateLogs("User cancelled deletion.");
                    //// UpdateFinalLogs();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.FormatError();
                this.UpdateLogs(msg);
                this.UserAdvicePause(msg);
            }

            return false;
        }

        private void StartCurrentRequest()
        {
            Task.Factory.StartNew(() =>
                        this.currentDeletionRequest.ExecuteDeletionRequest(this.currentDeletionRequest.RelationConnection,BatchItemDeleteProcessed, this.tokenSource.Token),
                        this.tokenSource.Token).
                ContinueWith(
                 antecedent => this.DeletionRequestCompleted(antecedent),
                 TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// get tag of selected radio button in a group
        /// </summary>
        /// <param name="radioGroup">the group box containing the radio buttons</param>
        /// <returns>the tag of the selected button</returns>
        private string GetGroupButtonTag(GroupBox radioGroup)
        {
            string returnValue = null;
            foreach (RadioButton radioButton in radioGroup.Controls.OfType<RadioButton>())
            {
                if (radioButton.Checked)
                {
                    returnValue = (string)radioButton.Tag;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// set button in a radio button group
        /// </summary>
        /// <param name="radioGroup">the group box containing the radio buttons</param>
        /// <param name="buttonTag">the tag of the button to be checked</param>
        private void SetGroupButtonByTag(GroupBox radioGroup, string buttonTag)
        {
            RadioButton buttonToCheck = null;
            foreach (RadioButton radioButton in radioGroup.Controls.OfType<RadioButton>())
            {
                if ((string)radioButton.Tag == buttonTag)
                {
                    buttonToCheck = radioButton;
                }
                else if (buttonToCheck == null && radioButton.Enabled)
                {
                    buttonToCheck = radioButton;
                }
            }

            if (buttonToCheck != null)
            {
                buttonToCheck.Checked = true;
            }
        }

        /// <summary>
        /// clean up the deletion process when the background deletion task is completed or cancelled
        /// </summary>
        /// <param name="sender">the event source</param>
        /// <param name="e">the completed worker event data</param>
        private void DeletionRequestCompleted(Task antecedent)
        {
            {
                String statusLogMessage;
                int recordsToProcess;
                bool completed;
                if (antecedent.IsCanceled)
                {
                    statusLogMessage = "Canceled!";
                    recordsToProcess = 0;
                    completed = false;
                }
                else
                {
                    recordsToProcess = BatchProgress.Maximum - this.SuccessCount - this.FailureCount;
                    completed = recordsToProcess <= 0; // original count could be less than the number of records actually deleted
                    if (completed)
                    {
                        statusLogMessage = "Process completed.";
                    }
                    else
                    {
                        statusLogMessage = $"Unprocessed records: {recordsToProcess}";
                    }
                }

                // Notify when complete.
                this.BatchItemDeleteProcessed(new BatchItemDeleteStatus { Message = statusLogMessage });

                if (antecedent.IsFaulted)
                {
                    statusLogMessage = "Error: " + antecedent.Exception.ToString();
                    this.BatchItemDeleteProcessed(new BatchItemDeleteStatus { Message = statusLogMessage });
                }

                if (!completed)
                {
                    DeletionRequest deletionRequest = this.currentDeletionRequest;
                    int numOfRecs = 0;
                    try
                    {
                        numOfRecs = deletionRequest.GetSelectCount();
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.FormatError();
                        this.BatchItemDeleteProcessed(new BatchItemDeleteStatus { Message = "Cannot retry deletion: " + msg });
                        numOfRecs = 0;
                    }
                    if (numOfRecs == recordsToProcess)
                    {
                        statusLogMessage = "Retrying delete";
                        this.BatchItemDeleteProcessed(new BatchItemDeleteStatus { Message = statusLogMessage });
                        this.StartCurrentRequest();
                    }
                    else
                    {
                        if (numOfRecs > 0)
                        {
                            statusLogMessage = $"Remaining records: {numOfRecs}";
                            this.BatchItemDeleteProcessed(new BatchItemDeleteStatus { Message = statusLogMessage });
                        }
                        recordsToProcess = 0;
                    }
                }

                if (recordsToProcess == 0)
                {
                    this.DeleteFinished(completed);
                    string statusUserMesssage = string.Format(
                            "\r\n{0} {1} deleted.\n{2} {3} could not be deleted.\nPlease refer to the log file for details",
                                this.SuccessCount,
                                this.GetGroupButtonTag(this.itemCategoryGroup).ToString().Plurality(this.SuccessCount),
                                this.FailureCount,
                                this.GetGroupButtonTag(this.itemCategoryGroup).ToString().Plurality(this.FailureCount));
                    this.UserAdvicePause(statusUserMesssage, antecedent.IsCompleted ? MessageBoxIcon.Information : MessageBoxIcon.Exclamation);
                    this.BatchProgress.Value = 0;
                }

            }
        }

        /// <summary>
        /// Enable the log file link if the file exists
        /// </summary>
        private void SetupLinkLog()
        {
            if (this.logFilePath.Text != string.Empty && File.Exists(this.FullLogFilePath()))
            {
                this.linkLog.Text = "Open " + this.FullLogFilePath();
                this.linkLog.Visible = true;
            }
            else
            {
                this.linkLog.Visible = false;
            }
        }

        private void BatchItemDeleteProcessed(BatchItemDeleteStatus status)
        {
            if (status.ItemSuccessStatus.HasValue)
            {
                if (status.ItemSuccessStatus.Value)
                {
                    ++this.SuccessCount;
                }
                else
                {
                    ++this.FailureCount;
                }
            }
            this.UpdateLogs(status.Message);
            int processedCount = this.SuccessCount + this.FailureCount;
            this.progressReporter.Report(processedCount);
        }

        private void ReportProgress(int processedCount)
        {
            this.BatchProgress.Value = processedCount;
        }


        /// <summary>
        /// Finalize progress reporting and trace logs and revert UI elements
        /// </summary>
        /// <param name="successful">Set if the job completed successfully</param>
        private void DeleteFinished(bool successful)
        {
            this.SetupLinkLog();

            // Update log file.
            this.UpdateLogs("****************Finished Processing****************");

            // Update user interface.
            this.labelProgress.Text = successful ? "Processing complete" : "Processing abandoned";

            // this.labelProgress.Refresh();
            this.Cursor = Cursors.Default;

            //// btnStart.Enabled = true
            this.StopListeners();
            this.Cursor = Cursors.Default;
            this.EnableStop(false);
        }


        /// <summary>
        /// Add the specified message and any exception details to the trace logs
        /// </summary>
        /// <param name="message">the message to add</param>
        /// <param name="ex">the exception to add or null for no exception</param>
        /// <param name="fatal">indicates whether a supplied exception is fatal</param>
        private void UpdateLogs(string message, System.Exception ex = null, bool fatal = false)
        {
            string timestamp = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss:ffff");
            message = timestamp + " " + message;
            if (ex == null)
            {
                Logger.WriteMessage(message, false);
            }
            else
            {
                Logger.WriteError(message, ex, fatal, false);
            }
        }

        /// <summary>
        /// Choose a file name to log to
        /// </summary>
        /// <param name="sender">the event source</param>
        /// <param name="e">the event data</param>
        private void ChooseLogFile_Click(object sender, EventArgs e)
        {
            /*
            logFileDialog.SelectedPath = logFilePath.Text;
            DialogResult dr = logFileDialog.ShowDialog();
            if (dr.Equals(DialogResult.OK))
            {
                logFilePath.Text = string.Format(
                    "{0}\\{1}{2}",
                    logFileDialog.SelectedPath.ToString(),
                    DateTime.Now.ToString("yyyyMMdd_hhmmss.ffff"),
                    Logger.LogExtension);
            }*/
            if (string.IsNullOrEmpty(this.logFilePath.Text) ||
                Directory.Exists(this.logFilePath.Text))
            {
                this.queryFileDialog.InitialDirectory = this.logFilePath.Text;
                this.queryFileDialog.FileName = DateTime.Now.ToString("yyyyMMdd_hhmmss.ffff");
            }
            else
            {
                this.queryFileDialog.InitialDirectory = Path.GetDirectoryName(this.logFilePath.Text);
                this.queryFileDialog.FileName = Path.GetFileName(this.logFilePath.Text);
            }
            this.queryFileDialog.CheckPathExists = true;
            this.queryFileDialog.CheckFileExists = false;
            this.queryFileDialog.AddExtension = true;
            this.queryFileDialog.DefaultExt = Logger.LogExtension;
            DialogResult dr = this.queryFileDialog.ShowDialog();
            if (dr.Equals(DialogResult.OK))
            {
                this.logFilePath.Text = this.queryFileDialog.FileName.ToString();
            }
        }

        /// <summary>
        /// open the log file
        /// </summary>
        /// <param name="sender">the event source</param>
        /// <param name="e">the event data</param>
        private void LinkLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // DI 10/16/2017 DI : better missing file check
            if (string.IsNullOrEmpty(logFilePath.Text))
            {
                return;
            }

            if (!logFilePath.Equals(string.Empty))
            {
                string filepath = this.FullLogFilePath();
                try
                {
                    Process.Start(filepath);
                }
                catch (Exception ex)
                {
                    this.UserAdvicePause("Cannot Open Log: " + filepath + "\n" + ex.Message);
                }
            }
        }

        /// <summary>
        /// disable and uncheck a radio button, and check the default radio button if the group does not contain another enabled button
        /// </summary>
        /// <param name="btn">the radio button to disable and uncheck</param>
        /// <param name="defaultBtn">the radio button to check if the group has no other checked radio button</param>
        private void ExcludeRadioButton(RadioButton btn, RadioButton defaultBtn)
        {
            btn.Enabled = false;
            if (btn.Checked)
            {
                if (defaultBtn == null)
                {
                    btn.Checked = false;
                }

                {
                    defaultBtn.Checked = true;
                }
            }
        }

        /// <summary>
        /// disabled and uncheck a check box
        /// </summary>
        /// <param name="cbx">the check box to disable and uncheck</param>
        private void ExcludeCheckBox(CheckBox cbx)
        {
            cbx.Enabled = false;
            cbx.Checked = false;
        }

        /// <summary>
        /// disable and clear a text box
        /// </summary>
        /// <param name="tbx">the textbox to disable and clear</param>
        private void ExcludeTextBox(TextBox tbx)
        {
            tbx.Text = string.Empty;
            errorProvider.SetError(tbx, string.Empty);
            tbx.Enabled = false;
        }

        /// <summary>
        /// respond to choosing a different select type
        /// </summary>
        /// <param name="sender">the event source</param>
        /// <param name="e">the event data</param>
        private void ItemListSourceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.dirty = true;
            this.ItemListSourceTypeValidate();
            this.itemListFile.Text = string.Empty;
            this.itemListKeywordSearch.Text = string.Empty;
            this.itemListFilter.Text = string.Empty;
        }

        /// <summary>
        /// get the select type as an enumeration
        /// </summary>
        /// <returns>the select type</returns>
        private Constants.SourceType GetItemListSourceType()
        {
            int selectedValue = itemListSourceType.SelectedIndex;
            if (selectedValue < 0 || selectedValue >= (int)Constants.SourceType.TypeCount)
            {
                throw new InvalidOperationException(string.Format("Not a valid data provider number: {0}", selectedValue));
            }

            Constants.SourceType returnValue = (Constants.SourceType)selectedValue;
            return returnValue;
        }

        /// <summary>
        /// validation required when selectType is changed
        /// </summary>
        private void ItemListSourceTypeValidate()
        {
            int selectedIndex = this.itemListSourceType.SelectedIndex;
            if (this.GetItemListSourceType() == Constants.SourceType.Query)
            {
                this.itemListFile.Text = string.Empty;
                this.itemListSource.SelectedTab = this.querySource;
            }
            else
            {
                this.itemListKeywordSearch.Text = string.Empty;
                this.itemListFilter.Text = string.Empty;
                this.itemListSource.SelectedTab = this.fileSource;
            }

            this.ItemTypeValidate();
        }

        /// <summary>
        /// check if the connection details are complete
        /// </summary>
        /// <returns>true if all the connection details are non-blank</returns>
        private bool ConnectionComplete()
        {
            bool returnValue = this.vaultUrl.Text.Length > 0 &&
                this.vaultUsername.Text.Length > 0 &&
                this.vaultPassword.Text.Length > 0 &&
                this.vaultApiVersion.Text.Length > 0 &&
                this.clientOrganisation.Text.Length > 0 &&
                this.vaultProduct.Text.Length > 0;
            return returnValue;
        }

        /// <summary>
        /// validate details needed to test the selection
        /// </summary>
        /// <returns>true if validated successfully</returns>
        private bool ValidateSelection()
        {
            bool valid = sourceContainer.ValidateChildren();
            //if (valid)
            //{
            //    valid = this.ValidateGroup(this.selection);
            //}

            return valid;
        }

        /// <summary>
        /// Note that an input value has changed so we know to prompt the user to save the setting
        /// </summary>
        /// <param name="sender">the event source</param>
        /// <param name="e">the event data</param>
        private void Setting_Changed(object sender, EventArgs e)
        {
            this.dirty = true;
        }

        /// <summary>
        /// respond to item type radio button being pressed
        /// </summary>
        /// <param name="sender">the event source</param>
        /// <param name="e">the event data</param>
        private void ItemType_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton btn = sender as RadioButton;
            if (btn.Checked)
            {
                this.dirty = true;
                this.ItemTypeValidate();
            }
        }

        /// <summary>
        /// Validate form of import when item type is changed
        /// </summary>
        private void ItemTypeValidate()
        {
            Constants.SourceType itemListSourceType = this.GetItemListSourceType();
            DeletionRequest.ItemCategory itemType = (DeletionRequest.ItemCategory)Enum.Parse(
                typeof(DeletionRequest.ItemCategory),
                this.GetGroupButtonTag(itemCategoryGroup),
                true);
            switch (itemType)
            {
                case DeletionRequest.ItemCategory.Document:
                //AK, 10/1/2020, add Archived Document item type
                case DeletionRequest.ItemCategory.Archived_Document:
                    //end changes by AK on 10/1/2020
                case DeletionRequest.ItemCategory.Binder:
                    this.withVersion.Enabled = true;
                    this.eachItem.Enabled = true;
                    this.ExcludeTextBox(this.objectName);
                    //AK, 10/1/2020, adding archived documents
                    if (itemType == DeletionRequest.ItemCategory.Archived_Document)
                    {
                        this.batched.Enabled = true;
                        this.batched.Checked = true;
                        this.eachItem.Enabled = false;
                    }
                    else //end changes by AK on 10/1/2020
                    {
                        if (itemType == DeletionRequest.ItemCategory.Document)
                        {
                            this.batched.Enabled = true;
                        }
                        else
                        {
                            this.ExcludeRadioButton(this.batched, this.eachItem);
                        }
                    }

                    this.ExcludeRadioButton(this.cascade, this.batched);
                    break;
                case DeletionRequest.ItemCategory.Object:
                    this.ExcludeCheckBox(this.withVersion);
                    this.objectName.Enabled = true;
                    switch (itemListSourceType)
                    {
                        case Constants.SourceType.Excel:
                        case Constants.SourceType.Access:
                            this.cascade.Enabled = true;
                            break;
                        default:
                            this.ExcludeRadioButton(this.cascade, this.eachItem);
                            break;
                    }

                    this.batched.Enabled = true;
                    this.ExcludeRadioButton(this.eachItem, this.batched);
                    break;
            }

            this.WithVersionValidate();
        }

        /// <summary>
        /// respond to deletion type radio button being pressed
        /// </summary>
        /// <param name="sender">the event source</param>
        /// <param name="e">the event data</param>
        private void DeletionType_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton btn = sender as RadioButton;
            if (btn.Checked)
            {
                this.dirty = true;
            }
        }

        /// <summary>
        /// Collect the API limits
        /// </summary>
        /// <param name="sender">the event source</param>
        /// <param name="e">the limit event data</param>
        private void UpdateLimits(object sender, LimitEventArgs e)
        {
            this.burstLimit.Text = e.BurstLimit.HasValue ? e.BurstLimit.Value.ToString("N0", CultureInfo.CurrentCulture) : string.Empty;
            this.dailyLimit.Text = e.DailyLimit.HasValue ? e.DailyLimit.Value.ToString("N0", CultureInfo.CurrentCulture) : string.Empty;
        }

        /// <summary>
        /// Response to API Limit throttling event
        /// </summary>
        /// <param name="sender">the event source</param>
        /// <param name="e">the limit event data</param>
        private void LimitThrottling(object sender, ThrottleEventArgs e)
        {
            this.UpdateLogs(string.Format("Pausing until {0} on API limit reached", e.ResumeAt));
        }

        /// <summary>
        /// Response to API Limit throttling event
        /// </summary>
        /// <param name="sender">the event source</param>
        /// <param name="e">the limit event data</param>
        private void LimitThrottled(object sender, ThrottleEventArgs e)
        {
            this.UpdateLogs(string.Format("Resuming at {0} after API throttling pause", e.ResumeAt));
        }

        /// <summary>
        /// Form initialization not set up by the designer
        /// </summary>
        /// <param name="sender">the event source </param>
        /// <param name="e">the event data</param>
        private void VeevaDelete_Load(object sender, EventArgs e)
        {
            this.EnableStop(false);
            this.splitContainer.FixedPanel = FixedPanel.Panel1;
            this.LoadState();
            // this.LoadExtendedState();
        }

        /// <summary>
        /// respond to save settings
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">the event data</param>
        private void SaveStateMenuItem_Click(object sender, EventArgs e)
        {
            this.SaveState();
            if (this.dirty)
            {
                this.LoadState();
            }
            // save the current screen settings to an Xml file
            // string appworkfolder = ConfigUtil.MakeAppWorkFolder();
        }

        /// <summary>
        /// respond to load settings
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">the event data</param>
        private void LoadStateMenuItem_Click(object sender, EventArgs e)
        {
            // N.B Leave this check here. 
            // It helps to verify that this.dirty/load/save covers all neccessary settings
            if (this.dirty)
            {
                this.LoadState();
            }

            // load a saved TRUseries Xml file
            // Add the truseries serialization class
        }

        /// <summary>
        /// save any changed settings on closing the form
        /// </summary>
        /// <param name="sender">the event source</param>
        /// <param name="e">the event data</param>
        private void VeevaDelete_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = false;
            if (this.dirty)
            {
                DialogResult dr = this.UserAdviceRequest(
                    "Application settings have changed.\nSave current values for application settings?",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);
                switch (dr)
                {
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                    case DialogResult.Yes:
                        this.SaveState();
                        break;
                }
            }
            if (!e.Cancel)
            {
                if (running)
                {
                    tokenSource.Cancel();
                }
            }
        }

        /// <summary>
        /// load settings that cannot be loaded automatically
        /// </summary>
        private void LoadExtendedState()
        {
            this.SetGroupButtonByTag(this.itemCategoryGroup, global::VeevaDelete.Properties.Settings.Default.Category);
            this.SetGroupButtonByTag(this.deletionTypeGroup, global::VeevaDelete.Properties.Settings.Default.DeletionType);
            this.SetupLinkLog();
        }

        /// <summary>
        /// save the settings
        /// </summary>
        private void SaveState()
        {
            if (this.dirty)
            {
                PropertyHelper.AssignIfChanged(
                    Properties.Settings.Default,
                    nameof(Properties.Settings.Default.Category),
                    this.GetGroupButtonTag(itemCategoryGroup));
                PropertyHelper.AssignIfChanged(
                    Properties.Settings.Default,
                    nameof(Properties.Settings.Default.DeletionType),
                    this.GetGroupButtonTag(deletionTypeGroup));
                Properties.Settings.Default.Save();
            }

            if (Properties.Settings.Default.Dirty)
            {
                // tell the settings manager that we are clean
                Properties.Settings.Default.Reload();
            }
        }

        /// <summary>
        /// load the settings and update the form controls accordingly
        /// </summary>
        private void LoadState()
        {
            Properties.Settings.Default.Reload();

            // silently assume first value if we don't recognise it.
            if (this.itemListSourceType.SelectedIndex < 0)
            {
                this.itemListSourceType.SelectedIndex = 0;
            }

            this.LoadExtendedState();
            this.ItemListSourceTypeValidate();
            this.dirty = false;
        }

        /// <summary>
        /// react to version being checked/unchecked
        /// </summary>
        /// <param name="sender">the event sender</param>
        /// <param name="e">the event data</param>
        private void WithVersion_CheckedChanged(object sender, EventArgs e)
        {
            this.dirty = true;
            this.WithVersionValidate();
        }

        /// <summary>
        /// Update form input when version is checked or unchecked
        /// </summary>
        private void WithVersionValidate()
        {
            // workaround for issue with settings reset
            bool withVersionChecked = this.withVersion.Checked;
            if (withVersionChecked)
            {
                this.version.Enabled = true;
            }
            else
            {
                this.ExcludeTextBox(this.version);
            }

            this.withVersion.Checked = withVersionChecked;
        }

        /// <summary>
        /// react to major version text box changing
        /// </summary>
        /// <param name="sender">the event source</param>
        /// <param name="e">the event data</param>
        private void Version_TextChanged(object sender, EventArgs e)
        {
            this.dirty = true;
        }

        /// <summary>
        /// react to the select file button being pressed
        /// </summary>
        /// <param name="sender">the event source</param>
        /// <param name="e">the event data</param>
        private void ChooseItemListFile_Click(object sender, EventArgs e)
        {
            this.queryFileDialog.InitialDirectory =
                this.itemListFile.Text == string.Empty ? string.Empty : Path.GetDirectoryName(this.itemListFile.Text);
            this.queryFileDialog.FileName = Path.GetFileName(this.itemListFile.Text);
            this.queryFileDialog.CheckPathExists = true;
            this.queryFileDialog.CheckFileExists = true;
            switch (this.GetItemListSourceType())
            {
                case Constants.SourceType.Csv:
                    queryFileDialog.DefaultExt = ".csv";
                    break;
                case Constants.SourceType.Excel:
                    queryFileDialog.DefaultExt = ".xlsx";
                    break;
                case Constants.SourceType.Access:
                    queryFileDialog.DefaultExt = ".accdb";
                    break;
            }

            DialogResult dr = this.queryFileDialog.ShowDialog();
            if (dr.Equals(DialogResult.OK))
            {
                this.itemListFile.Text = this.queryFileDialog.FileName.ToString();
            }
        }

        // these methods serialize the screen to Xml
        #region TRUseries Xml methods

        /// <summary>
        /// This is application specific stuff, all hard coded,
        /// grabs known screen elements and saves the settings to Xml
        /// </summary>
        /// <param name="element">XElement to add the resulting Xml into</param>
        private void ToTruScreenScraperXml(XElement element)
        {
            // this name is ony important to this aplication
            XElement result = new XElement("TRUscreenScraper");

            // now get the settings from the different controls
            result.Add(new XElement("veevaVaultUrl", this.vaultUrl.Text));
            result.Add(new XElement("veevaVaultUserid", this.vaultUsername.Text));

            result.Add(new XElement("veevaVaultPwd", this.vaultPassword.Text));
            result.Add(new XElement("veevaVaultApiver", this.vaultApiVersion.Text));

            result.Add(new XElement("logFileName", this.logFilePath.Text));

            string inputX = string.Empty;
            if (this.itemListSourceType.SelectedIndex == -1)
            {
                inputX = string.Empty;
            }
            else
            {
                inputX = itemListSourceType.Text;
            }

            XElement inputEx = new XElement("queryType", inputX);
            inputEx.Add(new XAttribute("note", "valid values are- VQL, CSV, Excel, Accss. VQL is default"));
            result.Add(new XElement("queryType", inputEx));

            result.Add(new XElement("queryText", this.itemListFilter.Text));

            // RadioButtonsValue stores this when it's clicked
            XElement operationEx = new XElement("operationType", this.GetGroupButtonTag(itemCategoryGroup));
            operationEx.Add(new XAttribute("note", "Value may be blank"));
            result.Add(new XElement("operationType", operationEx));

            element.Add(result);
        }
        #endregion

        /// <summary>
        /// Validate that a text box is not empty if it is enabled
        /// </summary>
        /// <param name="textBox">the text box to check</param>
        /// <param name="message">the error message to use if the textbox is enabled and empty</param>
        /// <param name="e">the error provider event to cancel if there is an error</param>
        private void ValidateNonEmpty(TextBox textBox, string message, CancelEventArgs e)
        {
            if (textBox.Enabled && string.IsNullOrEmpty(textBox.Text))
            {
                e.Cancel = true;
                this.errorProvider.SetError(textBox, message);
            }
            else
            {
                this.errorProvider.SetError(textBox, string.Empty);
            }
        }

        /// <summary>
        /// validate that a url has been entered
        /// </summary>
        /// <param name="sender">the source of the event</param>
        /// <param name="e">the event details</param>
        private void VaultUrl_Validating(object sender, CancelEventArgs e)
        {
            this.ValidateNonEmpty(this.vaultUrl, "Url is missing!", e);
        }

        /// <summary>
        /// validate that a username has been entered
        /// </summary>
        /// <param name="sender">the source of the event</param>
        /// <param name="e">the event details</param>
        private void VaultUsername_Validating(object sender, CancelEventArgs e)
        {
            this.ValidateNonEmpty(this.vaultUsername, "Username is missing!", e);
        }

        /// <summary>
        /// validate that a password has been entered
        /// </summary>
        /// <param name="sender">the source of the event</param>
        /// <param name="e">the event details</param>
        private void VaultPassword_Validating(object sender, CancelEventArgs e)
        {
            this.ValidateNonEmpty(this.vaultPassword, "Password is missing!", e);
        }

        /// <summary>
        /// validate that an API version has been entered
        /// </summary>
        /// <param name="sender">the source of the event</param>
        /// <param name="e">the event details</param>
        private void VaultApiVersion_Validating(object sender, CancelEventArgs e)
        {
            this.ValidateNonEmpty(this.vaultApiVersion, "Api version is missing!", e);
        }

        /// <summary>
        /// validate that a VQL query has been entered if VQL source is selected
        /// </summary>
        /// <param name="sender">the source of the event</param>
        /// <param name="e">the event details</param>
        private void ItemListFilter_Validating(object sender, CancelEventArgs e)
        {
            if (this.GetItemListSourceType() == Constants.SourceType.Query &&
                string.IsNullOrEmpty(this.itemListKeywordSearch.Text))
            {
                this.ValidateNonEmpty(this.itemListFilter, "Please enter a VQL WHERE condition or keywords to FIND", e);
            }
            else
            {
                this.errorProvider.SetError(this.itemListFilter, string.Empty);
            }
        }

        /// <summary>
        /// validate that a file name has been entered if a file source has been selected
        /// </summary>
        /// <param name="sender">the source of the event</param>
        /// <param name="e">the event details</param>
        private void ItemListFile_Validating(object sender, CancelEventArgs e)
        {
            if (this.GetItemListSourceType() != Constants.SourceType.Query)
            {
                this.ValidateNonEmpty(this.itemListFile, "Please set the inputfile path", e);
            }
            else
            {
                this.errorProvider.SetError(this.itemListFile, string.Empty);
            }
        }

        /// <summary>
        /// validate that an object name has been entered if the box is enabled (for object deletion)
        /// </summary>
        /// <param name="sender">the source of the event</param>
        /// <param name="e">the event details</param>
        private void ObjectName_Validating(object sender, CancelEventArgs e)
        {
            this.ValidateNonEmpty(this.objectName, "Please enter an object type name", e);
        }

        /// <summary>
        /// validate that an item type has been selected
        /// should not fail item type should default to document
        /// and it should only be possible to change this to another type
        /// </summary>
        /// <param name="sender">the source of the event</param>
        /// <param name="e">the event details</param>
        private void ItemType_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(this.GetGroupButtonTag(this.itemCategoryGroup)))
            {
                e.Cancel = true;
                this.errorProvider.SetError(this.itemCategoryGroup, "Please select appropriate item type to delete");
            }
            else
            {
                this.errorProvider.SetError(this.itemCategoryGroup, string.Empty);
            }
        }

        /// <summary>
        /// validate that a deletion type has been entered
        /// should never fail as a deletion type compatible with the item should be selected automatically
        /// and it should only be possible to change this to another type
        /// </summary>
        /// <param name="sender">the source of the event</param>
        /// <param name="e">the event details</param>
        private void DeletionType_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(this.GetGroupButtonTag(this.deletionTypeGroup)))
            {
                e.Cancel = true;
                this.errorProvider.SetError(this.deletionTypeGroup, "Please select appropriate deletion type");
            }
            else
            {
                this.errorProvider.SetError(this.deletionTypeGroup, string.Empty);
            }
        }

        /// <summary>
        /// validate major version number entry
        /// </summary>
        /// <param name="sender">the source of the event</param>
        /// <param name="e">the event details</param>
        private void Version_Validating(object sender, CancelEventArgs e)
        {
            (this.majorVersionOpt, this.minorVersionOpt) =
                this.ValidateVersionEntry(this.version, e);
        }

        /// <summary>
        /// validate major version number entry
        /// </summary>
        /// <param name="sender">the source of the event</param>
        /// <param name="e">the event details</param>
        private (long? majorVersionOpt, long? minorVersionOpt) ValidateVersionEntry(
            TextBox textbox, CancelEventArgs e)
        {
            string versionText = textbox.Text;
            if (string.IsNullOrEmpty(versionText))
            {
                this.errorProvider.SetError(textbox, string.Empty);
            }
            else
            {
                Match versionMatch = versionPattern.Match(versionText);
                if (versionMatch.Success)
                {
                    this.errorProvider.SetError(textbox, string.Empty);
                    string majorVersionText = versionMatch.Groups[1].Value;
                    if (long.TryParse(majorVersionText, out long majorVersion) &&
                        majorVersion >= 0)
                    {
                        string minorVersionText = versionMatch.Groups[2].Value;
                        if (long.TryParse(minorVersionText, out long minorVersion) &&
                            minorVersion >= 0)
                        {
                            this.errorProvider.SetError(textbox, string.Empty);
                            return (majorVersion, minorVersion);
                        }
                        else
                        {
                            this.errorProvider.SetError(textbox, "Minor version nust be numeric");
                        }
                    }
                    else
                    {
                        this.errorProvider.SetError(textbox, "Major version nust be numeric");
                    }
                }
                else
                {
                    this.errorProvider.SetError(textbox, "Please enter major.minor version number");
                }
            }
            return (null, null);
        }


        /// <summary>
        /// validate that a log file name has been entered
        /// </summary>
        /// <param name="sender">the source of the event</param>
        /// <param name="e">the event details</param>
        private void LogFilePath_Validating(object sender, CancelEventArgs e)
        {
            this.ValidateNonEmpty(this.logFilePath, "Please set the Log file path.", e);
            this.SetupLinkLog();
        }

        /// <summary>
        /// validate entry of a burst reserve size
        /// </summary>
        /// <param name="sender">the source of the event</param>
        /// <param name="e">the event details</param>
        private void BurstReserve_Validating(object sender, CancelEventArgs e)
        {
            this.burstReserveOpt = this.ValidatePositiveLongEntry(this.burstReserve, e);
        }

        /// <summary>
        /// validate entry of a counting number
        /// if no value was entered, allow the validation and return null
        /// if a numeric value was entered. pass the validation and return the value
        /// if a non-numeric value was entered, fail the validation and return null
        /// </summary>
        /// <param name="textbox">the textbox containing the number</param>
        /// <param name="e">the error provider validation event to cancel if the value is not a number</param>
        /// <returns>an optional number, blank if a number was not entered</returns>
        private long? ValidatePositiveLongEntry(TextBox textbox, CancelEventArgs e)
        {
            long? returnValue;
            string errorMessage;
            string enteredText = textbox.Text;
            if (enteredText.Length == 0)
            {
                returnValue = null;
                errorMessage = string.Empty;
            }
            else if (long.TryParse(enteredText, out long enteredValue) && enteredValue > 0)
            {
                returnValue = enteredValue;
                errorMessage = string.Empty;
            }
            else
            {
                returnValue = null;
                errorMessage = "Please enter a positive number";
            }
            SetEntryError(errorMessage, textbox, e);
            return returnValue;
        }

        /// <summary>
        /// validate entry of a counting number
        /// if no value was entered, allow the validation and return null
        /// if a numeric value was entered. pass the validation and return the value
        /// if a non-numeric value was entered, fail the validation and return null
        /// </summary>
        /// <param name="textbox">the textbox containing the number</param>
        /// <param name="e">the error provider validation event to cancel if the value is not a number</param>
        /// <returns>an optional number, blank if a number was not entered</returns>
        private (int?, string) ValidatePositiveIntEntry(string enteredText)
        {
            int? entryValue;
            string errorMessage;
            if (enteredText.Length == 0)
            {
                entryValue = null;
                errorMessage = string.Empty;
            }
            else if (int.TryParse(enteredText, out int enteredValue) && enteredValue > 0)
            {
                entryValue = enteredValue;
                errorMessage = string.Empty;
            }
            else
            {
                entryValue = null;
                errorMessage = "Please enter a positive number";
            }
            return (entryValue, errorMessage);
        }

        /// <summary>
        /// Set or clear the error on a text box according to the passed error message, and cancel the event if there is an error
        /// </summary>
        /// <param name="errorMessage">error message if there is an error to report</param>
        /// <param name="textbox">the textbox containing the number</param>
        /// <param name="e">the error provider validation event to cancel if the value is not a number</param>
        /// <returns>an optional number, blank if a number was not entered</returns>
        private void SetEntryError(string errorMessage, TextBox textbox, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(errorMessage))
            {
                this.errorProvider.SetError(textbox, string.Empty);
            }
            else
            {
                e.Cancel = true;
                this.errorProvider.SetError(textbox, errorMessage);
            }
        }


        /// <summary>
        /// Respond to drop on item list file text box
        /// </summary>
        /// <param name="sender">the source of the event</param>
        /// <param name="e">the event details</param>
        private void ItemListFile_DragDrop(object sender, DragEventArgs e)
        {
            // Handle FileDrop data.
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // the user can selecte multiple files.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // take the first one
                this.itemListFile.Text = files[0];
            }
        }

        /// <summary>
        /// Respond to drag onto item list file text box
        /// </summary>
        /// <param name="sender">the source of the event</param>
        /// <param name="e">the event details</param>
        private void ItemListFile_DragEnter(object sender, DragEventArgs e)
        {
            // If the data is a file or a bitmap, display the copy cursor.
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// Respond to exit menu item: close the application
        /// </summary>
        /// <param name="sender">the source of the event</param>
        /// <param name="e">the event details</param>
        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// validate entry of a burst reserve size
        /// </summary>
        /// <param name="sender">the source of the event</param>
        /// <param name="e">the event details</param>
        private void DailyReserve_Validating(object sender, CancelEventArgs e)
        {
            this.dailyReserveOpt = this.ValidatePositiveLongEntry(this.dailyReserve, e);
        }

        /// <summary>
        /// Respond to the user guide menu item
        /// </summary>
        /// <param name="sender">the source of the event</param>
        /// <param name="e">the event details</param>
        private void UserGuideMenuItem_Click(object sender, EventArgs e)
        {
            string exeFolder = Path.GetDirectoryName(Application.ExecutablePath);
            string exeName = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
            string helpFilePath = Path.Combine(exeFolder, exeName + ".pdf");

            bool helpFileExist = File.Exists(helpFilePath);

            // launch the pdf by file system asociation,  you can make a StartInfo if you really need to.
            if (helpFileExist)
            {
                Process.Start(helpFilePath);
            }
            else
            {
                this.UserAdvicePause("User Guide not currently available." + Environment.NewLine +
                                "Contact Valiance Partners for assistance.");
            }
        }

        /// <summary>
        /// Respond to the about menu item
        /// </summary>
        /// <param name="sender">the source of the event</param>
        /// <param name="e">the event details</param>
        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog();
        }

        private void ClientOrganisation_Validating(object sender, CancelEventArgs e)
        {
            this.ValidateNonEmpty(this.clientOrganisation, "Please enter the client organisation's Vault id", e);
        }

        private void VaultProduct_Validating(object sender, CancelEventArgs e)
        {
            this.ValidateNonEmpty(this.vaultProduct, "Please enter the Vault product name", e);
        }

        private void BatchSize_Validating(object sender, CancelEventArgs e)
        {
            string errorMessage;
            (this.batchSizeOpt, errorMessage) = this.ValidatePositiveIntEntry(this.batchSize.Text);
            SetEntryError(errorMessage, this.batchSize, e);
        }

        private void WithTimeOut_CheckedChanged(object sender, EventArgs e)
        {
            this.dirty = true;
            bool withTimeoutMinutesChecked = this.WithTimeout.Checked;
            SetupTimeout();
            TimeoutMinutes.Enabled = withTimeoutMinutesChecked;
            WithTimeoutRetries.Enabled = withTimeoutMinutesChecked;
            TimeoutRetries.Enabled = withTimeoutMinutesChecked && WithTimeoutRetries.Checked;
        }

        private void TimeoutMinutes_Validating(object sender, CancelEventArgs e)
        {
            string errorMessage = SetupTimeoutMinutes();
            SetEntryError(errorMessage, this.TimeoutMinutes, e);
        }

        private void SetupTimeout()
        {
            if (this.WithTimeout.Checked)
            {
                SetupTimeoutMinutes();
                if (this.WithTimeoutRetries.Checked)
                {
                    SetupTimeoutRetries();
                }
                else
                {
                    this.timeoutRetriesOpt = null;

                }
            }
            else
            {
                this.timeoutMinutesOpt = null;
                this.timeoutRetriesOpt = null;
            }
        }

        private string SetupTimeoutMinutes()
        {
            string errorMessage = "";
            (this.timeoutMinutesOpt, errorMessage) = this.ValidatePositiveIntEntry(this.TimeoutMinutes.Text);
            return errorMessage;
        }

        private void WithTimeoutRetries_CheckedChanged(object sender, EventArgs e)
        {
            this.dirty = true;
            bool withTimeoutRetriesChecked = this.WithTimeout.Checked;
            TimeoutRetries.Enabled = withTimeoutRetriesChecked;
            this.timeoutRetriesOpt = null;

        }

        private void TimeoutRetries_Validating(object sender, CancelEventArgs e)
        {
            string errorMessage = SetupTimeoutRetries();
            SetEntryError(errorMessage, this.TimeoutRetries, e);
        }

        private string SetupTimeoutRetries()
        {
            string errorMessage = "";
            (this.timeoutRetriesOpt, errorMessage) = this.ValidatePositiveIntEntry(this.TimeoutRetries.Text);
            return errorMessage;
        }
    }
}
