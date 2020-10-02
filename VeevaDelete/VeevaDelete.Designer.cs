//-----------------------------------------------------------------------
// <copyright file="VeevaDelete.Designer.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
// <summary>
// This file contains VeevaDelete class.
// </summary>
//-----------------------------------------------------------------------
namespace VeevaDelete
{
    /// <summary>
    /// Designer portion of Delete Tool Form
    /// </summary>
    partial class VeevaDelete
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.components != null)
                {
                    this.components.Dispose();
                }
                this.StopListeners();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label label6;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label8;
            System.Windows.Forms.Label label15;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.Label label11;
            System.Windows.Forms.Label label10;
            System.Windows.Forms.Label label9;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VeevaDelete));
            this.logFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.logFileDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.queryFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.burstLimit = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel5 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.dailyLimit = new System.Windows.Forms.ToolStripStatusLabel();
            this.labelProgress = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.saveStateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadStateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userGuideMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.famousToolTips = new System.Windows.Forms.ToolTip(this.components);
            this.StartStop = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.selection = new System.Windows.Forms.GroupBox();
            this.sourceContainer = new System.Windows.Forms.SplitContainer();
            this.connection = new System.Windows.Forms.GroupBox();
            this.vaultProduct = new System.Windows.Forms.TextBox();
            this.clientOrganisation = new System.Windows.Forms.TextBox();
            this.testConnection = new System.Windows.Forms.Button();
            this.vaultApiVersion = new System.Windows.Forms.TextBox();
            this.vaultPassword = new System.Windows.Forms.TextBox();
            this.vaultUrl = new System.Windows.Forms.TextBox();
            this.vaultUsername = new System.Windows.Forms.TextBox();
            this.sourceGroupBox = new System.Windows.Forms.GroupBox();
            this.itemTypePanel = new System.Windows.Forms.Panel();
            this.itemCategoryGroup = new System.Windows.Forms.GroupBox();
            this.archivedDocType = new System.Windows.Forms.RadioButton();
            this.objectType = new System.Windows.Forms.RadioButton();
            this.binderType = new System.Windows.Forms.RadioButton();
            this.documentType = new System.Windows.Forms.RadioButton();
            this.withVersion = new System.Windows.Forms.CheckBox();
            this.version = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.objectName = new System.Windows.Forms.TextBox();
            this.testSelection = new System.Windows.Forms.Button();
            this.sourceTypePanel = new System.Windows.Forms.Panel();
            this.itemListSourceType = new System.Windows.Forms.ComboBox();
            this.itemListSource = new System.Windows.Forms.TabControl();
            this.querySource = new System.Windows.Forms.TabPage();
            this.label14 = new System.Windows.Forms.Label();
            this.itemListKeywordSearch = new System.Windows.Forms.TextBox();
            this.itemListFilter = new System.Windows.Forms.TextBox();
            this.fileSource = new System.Windows.Forms.TabPage();
            this.chooseItemListFile = new System.Windows.Forms.Button();
            this.itemListFile = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.WithTimeoutRetries = new System.Windows.Forms.CheckBox();
            this.TimeoutRetries = new System.Windows.Forms.TextBox();
            this.WithTimeout = new System.Windows.Forms.CheckBox();
            this.TimeoutMinutes = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.batchSize = new System.Windows.Forms.TextBox();
            this.BatchProgress = new System.Windows.Forms.ProgressBar();
            this.deletionTypeGroup = new System.Windows.Forms.GroupBox();
            this.cascade = new System.Windows.Forms.RadioButton();
            this.batched = new System.Windows.Forms.RadioButton();
            this.eachItem = new System.Windows.Forms.RadioButton();
            this.label13 = new System.Windows.Forms.Label();
            this.dailyReserve = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.burstReserve = new System.Windows.Forms.TextBox();
            this.displayLog = new System.Windows.Forms.Button();
            this.chooseLogFile = new System.Windows.Forms.Button();
            this.logFilePath = new System.Windows.Forms.TextBox();
            this.linkLog = new System.Windows.Forms.LinkLabel();
            this.textLog = new System.Windows.Forms.TextBox();
            label6 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            label15 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            this.statusStrip.SuspendLayout();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.panel1.SuspendLayout();
            this.selection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sourceContainer)).BeginInit();
            this.sourceContainer.Panel1.SuspendLayout();
            this.sourceContainer.Panel2.SuspendLayout();
            this.sourceContainer.SuspendLayout();
            this.connection.SuspendLayout();
            this.sourceGroupBox.SuspendLayout();
            this.itemTypePanel.SuspendLayout();
            this.itemCategoryGroup.SuspendLayout();
            this.sourceTypePanel.SuspendLayout();
            this.itemListSource.SuspendLayout();
            this.querySource.SuspendLayout();
            this.fileSource.SuspendLayout();
            this.panel2.SuspendLayout();
            this.deletionTypeGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // label6
            // 
            label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(11, 154);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(62, 17);
            label6.TabIndex = 11;
            label6.Text = "Log File:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(7, 54);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(77, 17);
            label2.TabIndex = 2;
            label2.Text = "Username:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(8, 86);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(73, 17);
            label3.TabIndex = 4;
            label3.Text = "Password:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(8, 23);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(66, 17);
            label1.TabIndex = 0;
            label1.Text = "Vault Url:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(11, 185);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(85, 17);
            label4.TabIndex = 10;
            label4.Text = "API Version:";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(9, 118);
            label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(132, 17);
            label8.TabIndex = 6;
            label8.Text = "Client Organisation:";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new System.Drawing.Point(9, 153);
            label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label15.Name = "label15";
            label15.Size = new System.Drawing.Size(97, 17);
            label15.TabIndex = 8;
            label15.Text = "Vault Product:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(4, 91);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(94, 17);
            label5.TabIndex = 1;
            label5.Text = "Object Name:";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(8, 9);
            label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(44, 17);
            label11.TabIndex = 0;
            label11.Text = "Type:";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(3, 6);
            label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(54, 17);
            label10.TabIndex = 0;
            label10.Text = "Where:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(3, 18);
            label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(75, 17);
            label9.TabIndex = 2;
            label9.Text = "File Name:";
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.burstLimit,
            this.toolStripStatusLabel5,
            this.toolStripStatusLabel3,
            this.dailyLimit,
            this.labelProgress});
            this.statusStrip.Location = new System.Drawing.Point(0, 812);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip.Size = new System.Drawing.Size(953, 25);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(117, 20);
            this.toolStripStatusLabel1.Text = "Burst Remaining";
            // 
            // burstLimit
            // 
            this.burstLimit.Name = "burstLimit";
            this.burstLimit.Size = new System.Drawing.Size(15, 20);
            this.burstLimit.Text = "-";
            // 
            // toolStripStatusLabel5
            // 
            this.toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            this.toolStripStatusLabel5.Size = new System.Drawing.Size(13, 20);
            this.toolStripStatusLabel5.Text = "|";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(122, 20);
            this.toolStripStatusLabel3.Text = "Daily Remaining ";
            // 
            // dailyLimit
            // 
            this.dailyLimit.Name = "dailyLimit";
            this.dailyLimit.Size = new System.Drawing.Size(15, 20);
            this.dailyLimit.Text = "-";
            // 
            // labelProgress
            // 
            this.labelProgress.Name = "labelProgress";
            this.labelProgress.Size = new System.Drawing.Size(0, 20);
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.SystemColors.ControlDark;
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip.Size = new System.Drawing.Size(953, 28);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileMenu
            // 
            this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveStateMenuItem,
            this.loadStateMenuItem,
            this.exitMenuItem});
            this.fileMenu.Name = "fileMenu";
            this.fileMenu.Size = new System.Drawing.Size(44, 24);
            this.fileMenu.Text = "&File";
            // 
            // saveStateMenuItem
            // 
            this.saveStateMenuItem.Name = "saveStateMenuItem";
            this.saveStateMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
            this.saveStateMenuItem.Size = new System.Drawing.Size(200, 26);
            this.saveStateMenuItem.Text = "&Save State";
            this.saveStateMenuItem.Click += new System.EventHandler(this.SaveStateMenuItem_Click);
            // 
            // loadStateMenuItem
            // 
            this.loadStateMenuItem.Name = "loadStateMenuItem";
            this.loadStateMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.L)));
            this.loadStateMenuItem.Size = new System.Drawing.Size(200, 26);
            this.loadStateMenuItem.Text = "&Load State";
            this.loadStateMenuItem.Click += new System.EventHandler(this.LoadStateMenuItem_Click);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitMenuItem.Size = new System.Drawing.Size(200, 26);
            this.exitMenuItem.Text = "E&xit";
            this.exitMenuItem.Click += new System.EventHandler(this.ExitMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.userGuideMenuItem,
            this.aboutMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // userGuideMenuItem
            // 
            this.userGuideMenuItem.Name = "userGuideMenuItem";
            this.userGuideMenuItem.Size = new System.Drawing.Size(156, 26);
            this.userGuideMenuItem.Text = "&User Guide";
            this.userGuideMenuItem.Click += new System.EventHandler(this.UserGuideMenuItem_Click);
            // 
            // aboutMenuItem
            // 
            this.aboutMenuItem.Name = "aboutMenuItem";
            this.aboutMenuItem.Size = new System.Drawing.Size(156, 26);
            this.aboutMenuItem.Text = "&About";
            this.aboutMenuItem.Click += new System.EventHandler(this.AboutMenuItem_Click);
            // 
            // StartStop
            // 
            this.StartStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StartStop.BackColor = System.Drawing.Color.IndianRed;
            this.StartStop.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.StartStop.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.StartStop.Location = new System.Drawing.Point(8, 197);
            this.StartStop.Margin = new System.Windows.Forms.Padding(4);
            this.StartStop.Name = "StartStop";
            this.StartStop.Size = new System.Drawing.Size(89, 31);
            this.StartStop.TabIndex = 14;
            this.famousToolTips.SetToolTip(this.StartStop, "Click Delete to Begin Deletion");
            this.StartStop.UseVisualStyleBackColor = false;
            this.StartStop.Click += new System.EventHandler(this.StartStop_Click);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 28);
            this.splitContainer.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.panel1);
            this.splitContainer.Panel1.Controls.Add(this.panel2);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.textLog);
            this.splitContainer.Size = new System.Drawing.Size(953, 784);
            this.splitContainer.SplitterDistance = 453;
            this.splitContainer.SplitterWidth = 5;
            this.splitContainer.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.selection);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(453, 554);
            this.panel1.TabIndex = 35;
            // 
            // selection
            // 
            this.selection.Controls.Add(this.sourceContainer);
            this.selection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selection.Location = new System.Drawing.Point(0, 0);
            this.selection.Margin = new System.Windows.Forms.Padding(4);
            this.selection.Name = "selection";
            this.selection.Padding = new System.Windows.Forms.Padding(4);
            this.selection.Size = new System.Drawing.Size(453, 554);
            this.selection.TabIndex = 0;
            this.selection.TabStop = false;
            this.selection.Text = "Connections";
            // 
            // sourceContainer
            // 
            this.sourceContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.sourceContainer.Location = new System.Drawing.Point(4, 19);
            this.sourceContainer.Margin = new System.Windows.Forms.Padding(4);
            this.sourceContainer.Name = "sourceContainer";
            this.sourceContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // sourceContainer.Panel1
            // 
            this.sourceContainer.Panel1.Controls.Add(this.connection);
            // 
            // sourceContainer.Panel2
            // 
            this.sourceContainer.Panel2.Controls.Add(this.sourceGroupBox);
            this.sourceContainer.Size = new System.Drawing.Size(445, 531);
            this.sourceContainer.SplitterDistance = 196;
            this.sourceContainer.SplitterWidth = 5;
            this.sourceContainer.TabIndex = 2;
            // 
            // connection
            // 
            this.connection.Controls.Add(this.vaultProduct);
            this.connection.Controls.Add(label15);
            this.connection.Controls.Add(this.clientOrganisation);
            this.connection.Controls.Add(label8);
            this.connection.Controls.Add(this.testConnection);
            this.connection.Controls.Add(this.vaultApiVersion);
            this.connection.Controls.Add(this.vaultPassword);
            this.connection.Controls.Add(label4);
            this.connection.Controls.Add(label1);
            this.connection.Controls.Add(label3);
            this.connection.Controls.Add(this.vaultUrl);
            this.connection.Controls.Add(this.vaultUsername);
            this.connection.Controls.Add(label2);
            this.connection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.connection.Location = new System.Drawing.Point(0, 0);
            this.connection.Margin = new System.Windows.Forms.Padding(4);
            this.connection.Name = "connection";
            this.connection.Padding = new System.Windows.Forms.Padding(4);
            this.connection.Size = new System.Drawing.Size(445, 196);
            this.connection.TabIndex = 1;
            this.connection.TabStop = false;
            this.connection.Text = "Target Connection (Vault)";
            // 
            // vaultProduct
            // 
            this.vaultProduct.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vaultProduct.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::VeevaDelete.Properties.Settings.Default, "VaultProduct", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.vaultProduct.Location = new System.Drawing.Point(148, 149);
            this.vaultProduct.Margin = new System.Windows.Forms.Padding(4);
            this.vaultProduct.Name = "vaultProduct";
            this.vaultProduct.Size = new System.Drawing.Size(282, 22);
            this.vaultProduct.TabIndex = 9;
            this.vaultProduct.Text = global::VeevaDelete.Properties.Settings.Default.VaultProduct;
            this.vaultProduct.TextChanged += new System.EventHandler(this.Setting_Changed);
            this.vaultProduct.Validating += new System.ComponentModel.CancelEventHandler(this.VaultProduct_Validating);
            // 
            // clientOrganisation
            // 
            this.clientOrganisation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clientOrganisation.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::VeevaDelete.Properties.Settings.Default, "ClientOrganisation", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.clientOrganisation.Location = new System.Drawing.Point(148, 114);
            this.clientOrganisation.Margin = new System.Windows.Forms.Padding(4);
            this.clientOrganisation.Name = "clientOrganisation";
            this.clientOrganisation.Size = new System.Drawing.Size(282, 22);
            this.clientOrganisation.TabIndex = 7;
            this.clientOrganisation.Text = global::VeevaDelete.Properties.Settings.Default.ClientOrganisation;
            this.clientOrganisation.TextChanged += new System.EventHandler(this.Setting_Changed);
            this.clientOrganisation.Validating += new System.ComponentModel.CancelEventHandler(this.ClientOrganisation_Validating);
            // 
            // testConnection
            // 
            this.testConnection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.testConnection.Location = new System.Drawing.Point(285, 177);
            this.testConnection.Margin = new System.Windows.Forms.Padding(4);
            this.testConnection.Name = "testConnection";
            this.testConnection.Size = new System.Drawing.Size(147, 28);
            this.testConnection.TabIndex = 12;
            this.testConnection.Text = "Test Connection";
            this.testConnection.UseVisualStyleBackColor = true;
            this.testConnection.Click += new System.EventHandler(this.TestConnection_Click);
            // 
            // vaultApiVersion
            // 
            this.vaultApiVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vaultApiVersion.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::VeevaDelete.Properties.Settings.Default, "ApiVersion", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.vaultApiVersion.Location = new System.Drawing.Point(148, 181);
            this.vaultApiVersion.Margin = new System.Windows.Forms.Padding(4);
            this.vaultApiVersion.Name = "vaultApiVersion";
            this.vaultApiVersion.Size = new System.Drawing.Size(127, 22);
            this.vaultApiVersion.TabIndex = 11;
            this.vaultApiVersion.Text = global::VeevaDelete.Properties.Settings.Default.ApiVersion;
            this.vaultApiVersion.TextChanged += new System.EventHandler(this.Setting_Changed);
            this.vaultApiVersion.Validating += new System.ComponentModel.CancelEventHandler(this.VaultApiVersion_Validating);
            // 
            // vaultPassword
            // 
            this.vaultPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vaultPassword.Location = new System.Drawing.Point(91, 82);
            this.vaultPassword.Margin = new System.Windows.Forms.Padding(4);
            this.vaultPassword.Name = "vaultPassword";
            this.vaultPassword.Size = new System.Drawing.Size(341, 22);
            this.vaultPassword.TabIndex = 5;
            this.vaultPassword.UseSystemPasswordChar = true;
            this.vaultPassword.Validating += new System.ComponentModel.CancelEventHandler(this.VaultPassword_Validating);
            // 
            // vaultUrl
            // 
            this.vaultUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vaultUrl.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::VeevaDelete.Properties.Settings.Default, "VaultUrl", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.vaultUrl.Location = new System.Drawing.Point(91, 20);
            this.vaultUrl.Margin = new System.Windows.Forms.Padding(4);
            this.vaultUrl.Name = "vaultUrl";
            this.vaultUrl.Size = new System.Drawing.Size(341, 22);
            this.vaultUrl.TabIndex = 1;
            this.vaultUrl.Text = global::VeevaDelete.Properties.Settings.Default.VaultUrl;
            this.vaultUrl.TextChanged += new System.EventHandler(this.Setting_Changed);
            this.vaultUrl.Validating += new System.ComponentModel.CancelEventHandler(this.VaultUrl_Validating);
            // 
            // vaultUsername
            // 
            this.vaultUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vaultUsername.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::VeevaDelete.Properties.Settings.Default, "UserName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.vaultUsername.Location = new System.Drawing.Point(91, 52);
            this.vaultUsername.Margin = new System.Windows.Forms.Padding(4);
            this.vaultUsername.Name = "vaultUsername";
            this.vaultUsername.Size = new System.Drawing.Size(341, 22);
            this.vaultUsername.TabIndex = 3;
            this.vaultUsername.Text = global::VeevaDelete.Properties.Settings.Default.UserName;
            this.vaultUsername.TextChanged += new System.EventHandler(this.Setting_Changed);
            this.vaultUsername.Validating += new System.ComponentModel.CancelEventHandler(this.VaultUsername_Validating);
            // 
            // sourceGroupBox
            // 
            this.sourceGroupBox.Controls.Add(this.itemTypePanel);
            this.sourceGroupBox.Controls.Add(this.sourceTypePanel);
            this.sourceGroupBox.Controls.Add(this.itemListSource);
            this.sourceGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceGroupBox.Location = new System.Drawing.Point(0, 0);
            this.sourceGroupBox.Margin = new System.Windows.Forms.Padding(4);
            this.sourceGroupBox.Name = "sourceGroupBox";
            this.sourceGroupBox.Padding = new System.Windows.Forms.Padding(4);
            this.sourceGroupBox.Size = new System.Drawing.Size(445, 330);
            this.sourceGroupBox.TabIndex = 2;
            this.sourceGroupBox.TabStop = false;
            this.sourceGroupBox.Text = "Source Selection";
            // 
            // itemTypePanel
            // 
            this.itemTypePanel.Controls.Add(this.itemCategoryGroup);
            this.itemTypePanel.Controls.Add(this.withVersion);
            this.itemTypePanel.Controls.Add(label5);
            this.itemTypePanel.Controls.Add(this.version);
            this.itemTypePanel.Controls.Add(this.label7);
            this.itemTypePanel.Controls.Add(this.objectName);
            this.itemTypePanel.Controls.Add(this.testSelection);
            this.itemTypePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.itemTypePanel.Location = new System.Drawing.Point(4, 209);
            this.itemTypePanel.Margin = new System.Windows.Forms.Padding(0);
            this.itemTypePanel.Name = "itemTypePanel";
            this.itemTypePanel.Size = new System.Drawing.Size(437, 117);
            this.itemTypePanel.TabIndex = 1;
            // 
            // itemCategoryGroup
            // 
            this.itemCategoryGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.itemCategoryGroup.Controls.Add(this.archivedDocType);
            this.itemCategoryGroup.Controls.Add(this.objectType);
            this.itemCategoryGroup.Controls.Add(this.binderType);
            this.itemCategoryGroup.Controls.Add(this.documentType);
            this.itemCategoryGroup.Location = new System.Drawing.Point(4, 4);
            this.itemCategoryGroup.Margin = new System.Windows.Forms.Padding(4);
            this.itemCategoryGroup.Name = "itemCategoryGroup";
            this.itemCategoryGroup.Padding = new System.Windows.Forms.Padding(4);
            this.itemCategoryGroup.Size = new System.Drawing.Size(433, 49);
            this.itemCategoryGroup.TabIndex = 0;
            this.itemCategoryGroup.TabStop = false;
            this.itemCategoryGroup.Text = "Item Category";
            // 
            // archivedDocType
            // 
            this.archivedDocType.AutoSize = true;
            this.archivedDocType.Location = new System.Drawing.Point(98, 23);
            this.archivedDocType.Margin = new System.Windows.Forms.Padding(4);
            this.archivedDocType.Name = "archivedDocType";
            this.archivedDocType.Size = new System.Drawing.Size(152, 21);
            this.archivedDocType.TabIndex = 3;
            this.archivedDocType.TabStop = true;
            this.archivedDocType.Tag = "archived_document";
            this.archivedDocType.Text = "Archived Document";
            this.archivedDocType.UseVisualStyleBackColor = true;
            this.archivedDocType.CheckedChanged += new System.EventHandler(this.ItemType_CheckedChanged);
            // 
            // objectType
            // 
            this.objectType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.objectType.AutoSize = true;
            this.objectType.Location = new System.Drawing.Point(354, 23);
            this.objectType.Margin = new System.Windows.Forms.Padding(4);
            this.objectType.Name = "objectType";
            this.objectType.Size = new System.Drawing.Size(70, 21);
            this.objectType.TabIndex = 2;
            this.objectType.TabStop = true;
            this.objectType.Tag = "object";
            this.objectType.Text = "Object";
            this.objectType.UseVisualStyleBackColor = true;
            this.objectType.CheckedChanged += new System.EventHandler(this.ItemType_CheckedChanged);
            // 
            // binderType
            // 
            this.binderType.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.binderType.AutoSize = true;
            this.binderType.Location = new System.Drawing.Point(260, 23);
            this.binderType.Margin = new System.Windows.Forms.Padding(4);
            this.binderType.Name = "binderType";
            this.binderType.Size = new System.Drawing.Size(70, 21);
            this.binderType.TabIndex = 1;
            this.binderType.Tag = "binder";
            this.binderType.Text = "Binder";
            this.binderType.UseVisualStyleBackColor = true;
            this.binderType.CheckedChanged += new System.EventHandler(this.ItemType_CheckedChanged);
            // 
            // documentType
            // 
            this.documentType.AutoSize = true;
            this.documentType.Checked = true;
            this.documentType.Location = new System.Drawing.Point(1, 23);
            this.documentType.Margin = new System.Windows.Forms.Padding(4);
            this.documentType.Name = "documentType";
            this.documentType.Size = new System.Drawing.Size(93, 21);
            this.documentType.TabIndex = 0;
            this.documentType.TabStop = true;
            this.documentType.Tag = "document";
            this.documentType.Text = "Document";
            this.documentType.UseVisualStyleBackColor = true;
            this.documentType.CheckedChanged += new System.EventHandler(this.ItemType_CheckedChanged);
            // 
            // withVersion
            // 
            this.withVersion.AutoSize = true;
            this.withVersion.Checked = global::VeevaDelete.Properties.Settings.Default.WithVersion;
            this.withVersion.CheckState = System.Windows.Forms.CheckState.Checked;
            this.withVersion.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::VeevaDelete.Properties.Settings.Default, "WithVersion", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.withVersion.Location = new System.Drawing.Point(5, 59);
            this.withVersion.Margin = new System.Windows.Forms.Padding(4);
            this.withVersion.Name = "withVersion";
            this.withVersion.Size = new System.Drawing.Size(64, 21);
            this.withVersion.TabIndex = 3;
            this.withVersion.Text = "False";
            this.withVersion.UseVisualStyleBackColor = true;
            this.withVersion.CheckedChanged += new System.EventHandler(this.WithVersion_CheckedChanged);
            // 
            // version
            // 
            this.version.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.version.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::VeevaDelete.Properties.Settings.Default, "Version", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.version.Enabled = false;
            this.version.Location = new System.Drawing.Point(207, 55);
            this.version.Margin = new System.Windows.Forms.Padding(4);
            this.version.Name = "version";
            this.version.Size = new System.Drawing.Size(89, 22);
            this.version.TabIndex = 5;
            this.version.Text = global::VeevaDelete.Properties.Settings.Default.Version;
            this.version.TextChanged += new System.EventHandler(this.Version_TextChanged);
            this.version.Validating += new System.ComponentModel.CancelEventHandler(this.Version_Validating);
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(139, 59);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 17);
            this.label7.TabIndex = 4;
            this.label7.Text = "Version:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // objectName
            // 
            this.objectName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.objectName.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::VeevaDelete.Properties.Settings.Default, "ObjectName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.objectName.Enabled = false;
            this.objectName.Location = new System.Drawing.Point(127, 87);
            this.objectName.Margin = new System.Windows.Forms.Padding(4);
            this.objectName.Name = "objectName";
            this.objectName.Size = new System.Drawing.Size(170, 22);
            this.objectName.TabIndex = 2;
            this.objectName.Text = global::VeevaDelete.Properties.Settings.Default.ObjectName;
            this.objectName.TextChanged += new System.EventHandler(this.Setting_Changed);
            this.objectName.Validating += new System.ComponentModel.CancelEventHandler(this.ObjectName_Validating);
            // 
            // testSelection
            // 
            this.testSelection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.testSelection.Location = new System.Drawing.Point(302, 85);
            this.testSelection.Margin = new System.Windows.Forms.Padding(4);
            this.testSelection.Name = "testSelection";
            this.testSelection.Size = new System.Drawing.Size(123, 28);
            this.testSelection.TabIndex = 8;
            this.testSelection.Text = "Test Selection";
            this.testSelection.UseVisualStyleBackColor = true;
            this.testSelection.Click += new System.EventHandler(this.TestSelection_Click);
            // 
            // sourceTypePanel
            // 
            this.sourceTypePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.sourceTypePanel.Controls.Add(this.itemListSourceType);
            this.sourceTypePanel.Controls.Add(label11);
            this.sourceTypePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.sourceTypePanel.Location = new System.Drawing.Point(4, 19);
            this.sourceTypePanel.Margin = new System.Windows.Forms.Padding(0);
            this.sourceTypePanel.Name = "sourceTypePanel";
            this.sourceTypePanel.Size = new System.Drawing.Size(437, 36);
            this.sourceTypePanel.TabIndex = 0;
            // 
            // itemListSourceType
            // 
            this.itemListSourceType.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::VeevaDelete.Properties.Settings.Default, "SourceType", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.itemListSourceType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.itemListSourceType.FormattingEnabled = true;
            this.itemListSourceType.Items.AddRange(new object[] {
            "VQL Search",
            "CSV",
            "Excel",
            "Access"});
            this.itemListSourceType.Location = new System.Drawing.Point(88, 9);
            this.itemListSourceType.Margin = new System.Windows.Forms.Padding(4);
            this.itemListSourceType.Name = "itemListSourceType";
            this.itemListSourceType.Size = new System.Drawing.Size(117, 24);
            this.itemListSourceType.TabIndex = 1;
            this.itemListSourceType.Text = global::VeevaDelete.Properties.Settings.Default.SourceType;
            this.itemListSourceType.SelectedIndexChanged += new System.EventHandler(this.ItemListSourceType_SelectedIndexChanged);
            // 
            // itemListSource
            // 
            this.itemListSource.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.itemListSource.Controls.Add(this.querySource);
            this.itemListSource.Controls.Add(this.fileSource);
            this.itemListSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.itemListSource.Location = new System.Drawing.Point(4, 19);
            this.itemListSource.Margin = new System.Windows.Forms.Padding(0);
            this.itemListSource.Name = "itemListSource";
            this.itemListSource.Padding = new System.Drawing.Point(0, 5);
            this.itemListSource.SelectedIndex = 0;
            this.itemListSource.Size = new System.Drawing.Size(437, 307);
            this.itemListSource.TabIndex = 0;
            // 
            // querySource
            // 
            this.querySource.Controls.Add(this.label14);
            this.querySource.Controls.Add(this.itemListKeywordSearch);
            this.querySource.Controls.Add(label10);
            this.querySource.Controls.Add(this.itemListFilter);
            this.querySource.Location = new System.Drawing.Point(4, 32);
            this.querySource.Margin = new System.Windows.Forms.Padding(0);
            this.querySource.Name = "querySource";
            this.querySource.Size = new System.Drawing.Size(429, 271);
            this.querySource.TabIndex = 1;
            this.querySource.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(3, 96);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(39, 17);
            this.label14.TabIndex = 2;
            this.label14.Text = "Find:";
            // 
            // itemListKeywordSearch
            // 
            this.itemListKeywordSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.itemListKeywordSearch.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::VeevaDelete.Properties.Settings.Default, "SourceKeywordSearch", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.itemListKeywordSearch.Location = new System.Drawing.Point(83, 96);
            this.itemListKeywordSearch.Margin = new System.Windows.Forms.Padding(4);
            this.itemListKeywordSearch.Multiline = true;
            this.itemListKeywordSearch.Name = "itemListKeywordSearch";
            this.itemListKeywordSearch.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.itemListKeywordSearch.Size = new System.Drawing.Size(329, 38);
            this.itemListKeywordSearch.TabIndex = 3;
            this.itemListKeywordSearch.Text = global::VeevaDelete.Properties.Settings.Default.SourceKeywordSearch;
            // 
            // itemListFilter
            // 
            this.itemListFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.itemListFilter.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::VeevaDelete.Properties.Settings.Default, "SourceFilter", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.itemListFilter.Location = new System.Drawing.Point(83, 2);
            this.itemListFilter.Margin = new System.Windows.Forms.Padding(0);
            this.itemListFilter.Multiline = true;
            this.itemListFilter.Name = "itemListFilter";
            this.itemListFilter.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.itemListFilter.Size = new System.Drawing.Size(329, 89);
            this.itemListFilter.TabIndex = 0;
            this.itemListFilter.Text = global::VeevaDelete.Properties.Settings.Default.SourceFilter;
            this.itemListFilter.TextChanged += new System.EventHandler(this.Setting_Changed);
            this.itemListFilter.Validating += new System.ComponentModel.CancelEventHandler(this.ItemListFilter_Validating);
            // 
            // fileSource
            // 
            this.fileSource.Controls.Add(label9);
            this.fileSource.Controls.Add(this.chooseItemListFile);
            this.fileSource.Controls.Add(this.itemListFile);
            this.fileSource.Location = new System.Drawing.Point(4, 32);
            this.fileSource.Margin = new System.Windows.Forms.Padding(0);
            this.fileSource.Name = "fileSource";
            this.fileSource.Size = new System.Drawing.Size(429, 271);
            this.fileSource.TabIndex = 0;
            this.fileSource.UseVisualStyleBackColor = true;
            // 
            // chooseItemListFile
            // 
            this.chooseItemListFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chooseItemListFile.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.chooseItemListFile.Location = new System.Drawing.Point(375, 15);
            this.chooseItemListFile.Margin = new System.Windows.Forms.Padding(4);
            this.chooseItemListFile.Name = "chooseItemListFile";
            this.chooseItemListFile.Size = new System.Drawing.Size(29, 27);
            this.chooseItemListFile.TabIndex = 4;
            this.chooseItemListFile.Text = "...";
            this.chooseItemListFile.UseVisualStyleBackColor = true;
            this.chooseItemListFile.Click += new System.EventHandler(this.ChooseItemListFile_Click);
            // 
            // itemListFile
            // 
            this.itemListFile.AllowDrop = true;
            this.itemListFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.itemListFile.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::VeevaDelete.Properties.Settings.Default, "SourceFile", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.itemListFile.Location = new System.Drawing.Point(85, 15);
            this.itemListFile.Margin = new System.Windows.Forms.Padding(4);
            this.itemListFile.Name = "itemListFile";
            this.itemListFile.Size = new System.Drawing.Size(282, 22);
            this.itemListFile.TabIndex = 3;
            this.itemListFile.Text = global::VeevaDelete.Properties.Settings.Default.SourceFile;
            this.itemListFile.TextChanged += new System.EventHandler(this.Setting_Changed);
            this.itemListFile.Validating += new System.ComponentModel.CancelEventHandler(this.ItemListFile_Validating);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.WithTimeoutRetries);
            this.panel2.Controls.Add(this.TimeoutRetries);
            this.panel2.Controls.Add(this.WithTimeout);
            this.panel2.Controls.Add(this.TimeoutMinutes);
            this.panel2.Controls.Add(this.label16);
            this.panel2.Controls.Add(this.batchSize);
            this.panel2.Controls.Add(this.BatchProgress);
            this.panel2.Controls.Add(this.deletionTypeGroup);
            this.panel2.Controls.Add(this.label13);
            this.panel2.Controls.Add(this.dailyReserve);
            this.panel2.Controls.Add(this.label12);
            this.panel2.Controls.Add(this.burstReserve);
            this.panel2.Controls.Add(this.StartStop);
            this.panel2.Controls.Add(this.displayLog);
            this.panel2.Controls.Add(this.chooseLogFile);
            this.panel2.Controls.Add(this.logFilePath);
            this.panel2.Controls.Add(label6);
            this.panel2.Controls.Add(this.linkLog);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 554);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(453, 230);
            this.panel2.TabIndex = 34;
            // 
            // WithTimeoutRetries
            // 
            this.WithTimeoutRetries.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.WithTimeoutRetries.AutoSize = true;
            this.WithTimeoutRetries.Checked = global::VeevaDelete.Properties.Settings.Default.WithTimeoutRetries;
            this.WithTimeoutRetries.CheckState = System.Windows.Forms.CheckState.Checked;
            this.WithTimeoutRetries.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::VeevaDelete.Properties.Settings.Default, "WithTimeoutRetries", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.WithTimeoutRetries.Location = new System.Drawing.Point(230, 122);
            this.WithTimeoutRetries.Margin = new System.Windows.Forms.Padding(4);
            this.WithTimeoutRetries.Name = "WithTimeoutRetries";
            this.WithTimeoutRetries.Size = new System.Drawing.Size(130, 21);
            this.WithTimeoutRetries.TabIndex = 9;
            this.WithTimeoutRetries.Text = "Timeout Retries";
            this.WithTimeoutRetries.UseVisualStyleBackColor = true;
            this.WithTimeoutRetries.CheckedChanged += new System.EventHandler(this.WithTimeoutRetries_CheckedChanged);
            // 
            // TimeoutRetries
            // 
            this.TimeoutRetries.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TimeoutRetries.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::VeevaDelete.Properties.Settings.Default, "TimeoutRetries", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.TimeoutRetries.Location = new System.Drawing.Point(362, 119);
            this.TimeoutRetries.Margin = new System.Windows.Forms.Padding(4);
            this.TimeoutRetries.Name = "TimeoutRetries";
            this.TimeoutRetries.Size = new System.Drawing.Size(72, 22);
            this.TimeoutRetries.TabIndex = 10;
            this.TimeoutRetries.Text = global::VeevaDelete.Properties.Settings.Default.TimeoutRetries;
            this.TimeoutRetries.TextChanged += new System.EventHandler(this.Setting_Changed);
            this.TimeoutRetries.Validating += new System.ComponentModel.CancelEventHandler(this.TimeoutRetries_Validating);
            // 
            // WithTimeout
            // 
            this.WithTimeout.AutoSize = true;
            this.WithTimeout.Checked = global::VeevaDelete.Properties.Settings.Default.WithTimeout;
            this.WithTimeout.CheckState = System.Windows.Forms.CheckState.Checked;
            this.WithTimeout.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::VeevaDelete.Properties.Settings.Default, "WithTimeout", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.WithTimeout.Location = new System.Drawing.Point(8, 122);
            this.WithTimeout.Margin = new System.Windows.Forms.Padding(4);
            this.WithTimeout.Name = "WithTimeout";
            this.WithTimeout.Size = new System.Drawing.Size(134, 21);
            this.WithTimeout.TabIndex = 7;
            this.WithTimeout.Text = "Timeout Minutes";
            this.WithTimeout.UseVisualStyleBackColor = true;
            this.WithTimeout.CheckedChanged += new System.EventHandler(this.WithTimeOut_CheckedChanged);
            // 
            // TimeoutMinutes
            // 
            this.TimeoutMinutes.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::VeevaDelete.Properties.Settings.Default, "TimeoutMinutes", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.TimeoutMinutes.Location = new System.Drawing.Point(153, 118);
            this.TimeoutMinutes.Margin = new System.Windows.Forms.Padding(4);
            this.TimeoutMinutes.Name = "TimeoutMinutes";
            this.TimeoutMinutes.Size = new System.Drawing.Size(64, 22);
            this.TimeoutMinutes.TabIndex = 8;
            this.TimeoutMinutes.Text = global::VeevaDelete.Properties.Settings.Default.TimeoutMinutes;
            this.TimeoutMinutes.TextChanged += new System.EventHandler(this.Setting_Changed);
            this.TimeoutMinutes.Validating += new System.ComponentModel.CancelEventHandler(this.TimeoutMinutes_Validating);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(8, 57);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(79, 17);
            this.label16.TabIndex = 1;
            this.label16.Text = "Batch Size:";
            // 
            // batchSize
            // 
            this.batchSize.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::VeevaDelete.Properties.Settings.Default, "BatchSize", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.batchSize.Location = new System.Drawing.Point(153, 53);
            this.batchSize.Margin = new System.Windows.Forms.Padding(4);
            this.batchSize.Name = "batchSize";
            this.batchSize.Size = new System.Drawing.Size(52, 22);
            this.batchSize.TabIndex = 2;
            this.batchSize.Text = global::VeevaDelete.Properties.Settings.Default.BatchSize;
            this.batchSize.TextChanged += new System.EventHandler(this.Setting_Changed);
            this.batchSize.Validating += new System.ComponentModel.CancelEventHandler(this.BatchSize_Validating);
            // 
            // BatchProgress
            // 
            this.BatchProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BatchProgress.Location = new System.Drawing.Point(104, 197);
            this.BatchProgress.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.BatchProgress.Name = "BatchProgress";
            this.BatchProgress.Size = new System.Drawing.Size(202, 31);
            this.BatchProgress.TabIndex = 15;
            // 
            // deletionTypeGroup
            // 
            this.deletionTypeGroup.Controls.Add(this.cascade);
            this.deletionTypeGroup.Controls.Add(this.batched);
            this.deletionTypeGroup.Controls.Add(this.eachItem);
            this.deletionTypeGroup.Dock = System.Windows.Forms.DockStyle.Top;
            this.deletionTypeGroup.Location = new System.Drawing.Point(0, 0);
            this.deletionTypeGroup.Margin = new System.Windows.Forms.Padding(4);
            this.deletionTypeGroup.Name = "deletionTypeGroup";
            this.deletionTypeGroup.Padding = new System.Windows.Forms.Padding(4);
            this.deletionTypeGroup.Size = new System.Drawing.Size(453, 53);
            this.deletionTypeGroup.TabIndex = 0;
            this.deletionTypeGroup.TabStop = false;
            this.deletionTypeGroup.Text = "Deletion Type";
            this.deletionTypeGroup.Validating += new System.ComponentModel.CancelEventHandler(this.DeletionType_Validating);
            // 
            // cascade
            // 
            this.cascade.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cascade.AutoSize = true;
            this.cascade.Location = new System.Drawing.Point(362, 23);
            this.cascade.Margin = new System.Windows.Forms.Padding(4);
            this.cascade.Name = "cascade";
            this.cascade.Size = new System.Drawing.Size(84, 21);
            this.cascade.TabIndex = 2;
            this.cascade.Tag = "Cascade";
            this.cascade.Text = "Cascade";
            this.cascade.UseVisualStyleBackColor = true;
            this.cascade.CheckedChanged += new System.EventHandler(this.DeletionType_CheckedChanged);
            // 
            // batched
            // 
            this.batched.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.batched.AutoSize = true;
            this.batched.Location = new System.Drawing.Point(186, 23);
            this.batched.Margin = new System.Windows.Forms.Padding(4);
            this.batched.Name = "batched";
            this.batched.Size = new System.Drawing.Size(81, 21);
            this.batched.TabIndex = 1;
            this.batched.Tag = "Batched";
            this.batched.Text = "Batched";
            this.batched.UseVisualStyleBackColor = true;
            this.batched.CheckedChanged += new System.EventHandler(this.DeletionType_CheckedChanged);
            // 
            // eachItem
            // 
            this.eachItem.AutoSize = true;
            this.eachItem.Checked = true;
            this.eachItem.Location = new System.Drawing.Point(4, 23);
            this.eachItem.Margin = new System.Windows.Forms.Padding(4);
            this.eachItem.Name = "eachItem";
            this.eachItem.Size = new System.Drawing.Size(91, 21);
            this.eachItem.TabIndex = 0;
            this.eachItem.TabStop = true;
            this.eachItem.Tag = "Item";
            this.eachItem.Text = "Each Item";
            this.eachItem.UseVisualStyleBackColor = true;
            this.eachItem.CheckedChanged += new System.EventHandler(this.DeletionType_CheckedChanged);
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(223, 92);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(125, 17);
            this.label13.TabIndex = 5;
            this.label13.Text = "Daily API Reserve:";
            // 
            // dailyReserve
            // 
            this.dailyReserve.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dailyReserve.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::VeevaDelete.Properties.Settings.Default, "DailyReserve", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.dailyReserve.Location = new System.Drawing.Point(359, 86);
            this.dailyReserve.Margin = new System.Windows.Forms.Padding(4);
            this.dailyReserve.Name = "dailyReserve";
            this.dailyReserve.Size = new System.Drawing.Size(75, 22);
            this.dailyReserve.TabIndex = 6;
            this.dailyReserve.Text = global::VeevaDelete.Properties.Settings.Default.DailyReserve;
            this.dailyReserve.TextChanged += new System.EventHandler(this.Setting_Changed);
            this.dailyReserve.Validating += new System.ComponentModel.CancelEventHandler(this.DailyReserve_Validating);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(1, 92);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(127, 17);
            this.label12.TabIndex = 3;
            this.label12.Text = "Burst API Reserve:";
            // 
            // burstReserve
            // 
            this.burstReserve.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::VeevaDelete.Properties.Settings.Default, "BurstReserve", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.burstReserve.Location = new System.Drawing.Point(153, 86);
            this.burstReserve.Margin = new System.Windows.Forms.Padding(4);
            this.burstReserve.Name = "burstReserve";
            this.burstReserve.Size = new System.Drawing.Size(63, 22);
            this.burstReserve.TabIndex = 4;
            this.burstReserve.Text = global::VeevaDelete.Properties.Settings.Default.BurstReserve;
            this.burstReserve.TextChanged += new System.EventHandler(this.Setting_Changed);
            this.burstReserve.Validating += new System.ComponentModel.CancelEventHandler(this.BurstReserve_Validating);
            // 
            // displayLog
            // 
            this.displayLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.displayLog.Location = new System.Drawing.Point(311, 197);
            this.displayLog.Margin = new System.Windows.Forms.Padding(4);
            this.displayLog.Name = "displayLog";
            this.displayLog.Size = new System.Drawing.Size(125, 31);
            this.displayLog.TabIndex = 16;
            this.displayLog.UseMnemonic = false;
            this.displayLog.UseVisualStyleBackColor = true;
            this.displayLog.Click += new System.EventHandler(this.DisplayLog_Click);
            // 
            // chooseLogFile
            // 
            this.chooseLogFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chooseLogFile.AutoSize = true;
            this.chooseLogFile.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.chooseLogFile.Location = new System.Drawing.Point(403, 149);
            this.chooseLogFile.Margin = new System.Windows.Forms.Padding(4);
            this.chooseLogFile.Name = "chooseLogFile";
            this.chooseLogFile.Size = new System.Drawing.Size(30, 27);
            this.chooseLogFile.TabIndex = 13;
            this.chooseLogFile.Text = "...";
            this.chooseLogFile.UseVisualStyleBackColor = true;
            this.chooseLogFile.Click += new System.EventHandler(this.ChooseLogFile_Click);
            // 
            // logFilePath
            // 
            this.logFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logFilePath.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::VeevaDelete.Properties.Settings.Default, "LogFile", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.logFilePath.Location = new System.Drawing.Point(96, 150);
            this.logFilePath.Margin = new System.Windows.Forms.Padding(4);
            this.logFilePath.Name = "logFilePath";
            this.logFilePath.Size = new System.Drawing.Size(293, 22);
            this.logFilePath.TabIndex = 12;
            this.logFilePath.Text = global::VeevaDelete.Properties.Settings.Default.LogFile;
            this.logFilePath.TextChanged += new System.EventHandler(this.Setting_Changed);
            this.logFilePath.Validating += new System.ComponentModel.CancelEventHandler(this.LogFilePath_Validating);
            // 
            // linkLog
            // 
            this.linkLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLog.AutoSize = true;
            this.linkLog.Location = new System.Drawing.Point(12, 176);
            this.linkLog.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkLog.Name = "linkLog";
            this.linkLog.Size = new System.Drawing.Size(71, 17);
            this.linkLog.TabIndex = 12;
            this.linkLog.TabStop = true;
            this.linkLog.Text = "Open Log";
            this.linkLog.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLog_LinkClicked);
            // 
            // textLog
            // 
            this.textLog.BackColor = System.Drawing.SystemColors.Window;
            this.textLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textLog.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textLog.Location = new System.Drawing.Point(0, 0);
            this.textLog.Margin = new System.Windows.Forms.Padding(4);
            this.textLog.Multiline = true;
            this.textLog.Name = "textLog";
            this.textLog.ReadOnly = true;
            this.textLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textLog.Size = new System.Drawing.Size(495, 784);
            this.textLog.TabIndex = 0;
            // 
            // VeevaDelete
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.ClientSize = new System.Drawing.Size(953, 837);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "VeevaDelete";
            this.Text = "Veeva Delete";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VeevaDelete_FormClosing);
            this.Load += new System.EventHandler(this.VeevaDelete_Load);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.selection.ResumeLayout(false);
            this.sourceContainer.Panel1.ResumeLayout(false);
            this.sourceContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sourceContainer)).EndInit();
            this.sourceContainer.ResumeLayout(false);
            this.connection.ResumeLayout(false);
            this.connection.PerformLayout();
            this.sourceGroupBox.ResumeLayout(false);
            this.itemTypePanel.ResumeLayout(false);
            this.itemTypePanel.PerformLayout();
            this.itemCategoryGroup.ResumeLayout(false);
            this.itemCategoryGroup.PerformLayout();
            this.sourceTypePanel.ResumeLayout(false);
            this.sourceTypePanel.PerformLayout();
            this.itemListSource.ResumeLayout(false);
            this.querySource.ResumeLayout(false);
            this.querySource.PerformLayout();
            this.fileSource.ResumeLayout(false);
            this.fileSource.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.deletionTypeGroup.ResumeLayout(false);
            this.deletionTypeGroup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog logFolderDialog;
        private System.Windows.Forms.FolderBrowserDialog logFileDialog;
        private System.Windows.Forms.OpenFileDialog queryFileDialog;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.ToolStripMenuItem saveStateMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadStateMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel burstLimit;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel5;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel dailyLimit;
        private System.Windows.Forms.ToolTip famousToolTips;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TextBox textLog;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel labelProgress;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem userGuideMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox dailyReserve;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox burstReserve;
        private System.Windows.Forms.Button StartStop;
        private System.Windows.Forms.Button displayLog;
        private System.Windows.Forms.Button chooseLogFile;
        private System.Windows.Forms.TextBox logFilePath;
        private System.Windows.Forms.LinkLabel linkLog;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox deletionTypeGroup;
        private System.Windows.Forms.RadioButton cascade;
        private System.Windows.Forms.RadioButton batched;
        private System.Windows.Forms.RadioButton eachItem;
        private System.Windows.Forms.ProgressBar BatchProgress;
        private System.Windows.Forms.GroupBox selection;
        private System.Windows.Forms.SplitContainer sourceContainer;
        private System.Windows.Forms.GroupBox connection;
        private System.Windows.Forms.TextBox vaultProduct;
        private System.Windows.Forms.TextBox clientOrganisation;
        private System.Windows.Forms.Button testConnection;
        private System.Windows.Forms.TextBox vaultApiVersion;
        private System.Windows.Forms.TextBox vaultPassword;
        private System.Windows.Forms.TextBox vaultUrl;
        private System.Windows.Forms.TextBox vaultUsername;
        private System.Windows.Forms.GroupBox sourceGroupBox;
        private System.Windows.Forms.Panel itemTypePanel;
        private System.Windows.Forms.GroupBox itemCategoryGroup;
        private System.Windows.Forms.RadioButton objectType;
        private System.Windows.Forms.RadioButton binderType;
        private System.Windows.Forms.RadioButton documentType;
        private System.Windows.Forms.CheckBox withVersion;
        private System.Windows.Forms.TextBox version;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox objectName;
        private System.Windows.Forms.Button testSelection;
        private System.Windows.Forms.Panel sourceTypePanel;
        private System.Windows.Forms.ComboBox itemListSourceType;
        private System.Windows.Forms.TabControl itemListSource;
        private System.Windows.Forms.TabPage querySource;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox itemListKeywordSearch;
        private System.Windows.Forms.TextBox itemListFilter;
        private System.Windows.Forms.TabPage fileSource;
        private System.Windows.Forms.Button chooseItemListFile;
        private System.Windows.Forms.TextBox itemListFile;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox batchSize;
        private System.Windows.Forms.CheckBox WithTimeoutRetries;
        private System.Windows.Forms.TextBox TimeoutRetries;
        private System.Windows.Forms.CheckBox WithTimeout;
        private System.Windows.Forms.TextBox TimeoutMinutes;
        private System.Windows.Forms.RadioButton archivedDocType;
    }
}

