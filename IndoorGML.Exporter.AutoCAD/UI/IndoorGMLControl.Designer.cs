using System;
using System.Windows.Forms;

namespace CityGML.Exporter.AutoCAD.UI
{
    partial class IndoorGMLControl
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label11 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox_FileName = new System.Windows.Forms.TextBox();
            this.button_DeleteSpace = new System.Windows.Forms.Button();
            this.button_ClearRooms = new System.Windows.Forms.Button();
            this.button_RoomList = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.label_Tolerance = new System.Windows.Forms.Label();
            this.textBox_GapTolerance = new System.Windows.Forms.TextBox();
            this.button_CreateRoom = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label_Gap = new System.Windows.Forms.Label();
            this.textBox_Gap = new System.Windows.Forms.TextBox();
            this.button_FixGap = new System.Windows.Forms.Button();
            this.button_BrowseFile = new System.Windows.Forms.Button();
            this.textBox_Output = new System.Windows.Forms.TextBox();
            this.dataGridView_Model = new System.Windows.Forms.DataGridView();
            this.ColumnID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ColumnFunction = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ColumnHeight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnZElevation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button_Export = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBox_DefaultFunction = new System.Windows.Forms.ComboBox();
            this.comboBox_DefaultClass = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label_unit = new System.Windows.Forms.Label();
            this.label_unit_1 = new System.Windows.Forms.Label();
            this.label_unit_2 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.textBox_VerticalTransitionTolerance = new System.Windows.Forms.TextBox();
            this.button_Create = new System.Windows.Forms.Button();
            this.label_BoundaryLayerName = new System.Windows.Forms.Label();
            this.textBox_RoomLayerName = new System.Windows.Forms.TextBox();
            this.button_BrowseLog = new System.Windows.Forms.Button();
            this.button_OpenLog = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox_LogFile = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox_RoomSeperate = new System.Windows.Forms.TextBox();
            this.label_FloorHeight = new System.Windows.Forms.Label();
            this.textBox_Height = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_Elevation = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_Building = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_LayerVirtualDoor = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_FloorName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBox_Unit = new System.Windows.Forms.ComboBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button_AddFiles = new System.Windows.Forms.Button();
            this.button_Browse = new System.Windows.Forms.Button();
            this.textBox_CombineOutput = new System.Windows.Forms.TextBox();
            this.button_Combines = new System.Windows.Forms.Button();
            this.dataGridView_CombineFiles = new System.Windows.Forms.DataGridView();
            this.ColumnFile = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnFloor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnBuilding = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.combineMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Model)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_CombineFiles)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(350, 555);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.textBox_FileName);
            this.tabPage1.Controls.Add(this.button_DeleteSpace);
            this.tabPage1.Controls.Add(this.button_ClearRooms);
            this.tabPage1.Controls.Add(this.button_RoomList);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.label_Tolerance);
            this.tabPage1.Controls.Add(this.textBox_GapTolerance);
            this.tabPage1.Controls.Add(this.button_CreateRoom);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.label_Gap);
            this.tabPage1.Controls.Add(this.textBox_Gap);
            this.tabPage1.Controls.Add(this.button_FixGap);
            this.tabPage1.Controls.Add(this.button_BrowseFile);
            this.tabPage1.Controls.Add(this.textBox_Output);
            this.tabPage1.Controls.Add(this.dataGridView_Model);
            this.tabPage1.Controls.Add(this.button_Export);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(342, 529);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Exporter";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 503);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(52, 13);
            this.label11.TabIndex = 58;
            this.label11.Text = "File name";
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 477);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(68, 13);
            this.label9.TabIndex = 57;
            this.label9.Text = "Output folder";
            // 
            // textBox_FileName
            // 
            this.textBox_FileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_FileName.Location = new System.Drawing.Point(87, 500);
            this.textBox_FileName.Name = "textBox_FileName";
            this.textBox_FileName.Size = new System.Drawing.Size(165, 20);
            this.textBox_FileName.TabIndex = 56;
            // 
            // button_DeleteSpace
            // 
            this.button_DeleteSpace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_DeleteSpace.Location = new System.Drawing.Point(87, 67);
            this.button_DeleteSpace.Name = "button_DeleteSpace";
            this.button_DeleteSpace.Size = new System.Drawing.Size(75, 23);
            this.button_DeleteSpace.TabIndex = 55;
            this.button_DeleteSpace.Text = "Delete";
            this.button_DeleteSpace.UseVisualStyleBackColor = true;
            this.button_DeleteSpace.Click += new System.EventHandler(this.button_DeleteSpace_Click);
            // 
            // button_ClearRooms
            // 
            this.button_ClearRooms.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_ClearRooms.Location = new System.Drawing.Point(177, 67);
            this.button_ClearRooms.Name = "button_ClearRooms";
            this.button_ClearRooms.Size = new System.Drawing.Size(75, 23);
            this.button_ClearRooms.TabIndex = 54;
            this.button_ClearRooms.Text = "Clear";
            this.button_ClearRooms.UseVisualStyleBackColor = true;
            this.button_ClearRooms.Click += new System.EventHandler(this.button_ClearRooms_Click);
            // 
            // button_RoomList
            // 
            this.button_RoomList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_RoomList.Location = new System.Drawing.Point(261, 67);
            this.button_RoomList.Name = "button_RoomList";
            this.button_RoomList.Size = new System.Drawing.Size(75, 23);
            this.button_RoomList.TabIndex = 53;
            this.button_RoomList.Text = "Load";
            this.button_RoomList.UseVisualStyleBackColor = true;
            this.button_RoomList.Click += new System.EventHandler(this.button_RoomList_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 72);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(55, 13);
            this.label10.TabIndex = 52;
            this.label10.Text = "Rooms list";
            // 
            // label_Tolerance
            // 
            this.label_Tolerance.AutoSize = true;
            this.label_Tolerance.Location = new System.Drawing.Point(7, 44);
            this.label_Tolerance.Name = "label_Tolerance";
            this.label_Tolerance.Size = new System.Drawing.Size(74, 13);
            this.label_Tolerance.TabIndex = 51;
            this.label_Tolerance.Text = "Gap tolerance";
            // 
            // textBox_GapTolerance
            // 
            this.textBox_GapTolerance.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_GapTolerance.Location = new System.Drawing.Point(87, 41);
            this.textBox_GapTolerance.Name = "textBox_GapTolerance";
            this.textBox_GapTolerance.Size = new System.Drawing.Size(165, 20);
            this.textBox_GapTolerance.TabIndex = 50;
            this.textBox_GapTolerance.Text = "100";
            this.textBox_GapTolerance.TextChanged += new System.EventHandler(this.updateSettingTextBox);
            // 
            // button_CreateRoom
            // 
            this.button_CreateRoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_CreateRoom.Image = global::IndoorGML.Exporter.AutoCAD.Properties.Resources.navtools_focus;
            this.button_CreateRoom.Location = new System.Drawing.Point(295, 424);
            this.button_CreateRoom.Margin = new System.Windows.Forms.Padding(2);
            this.button_CreateRoom.Name = "button_CreateRoom";
            this.button_CreateRoom.Size = new System.Drawing.Size(42, 37);
            this.button_CreateRoom.TabIndex = 33;
            this.button_CreateRoom.UseVisualStyleBackColor = true;
            this.button_CreateRoom.Click += new System.EventHandler(this.button_CreateRoom_Click);
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 424);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(245, 26);
            this.label7.TabIndex = 49;
            this.label7.Text = "Use following command to create Room boundary \r\nat bldg-GML-room layer";
            // 
            // label_Gap
            // 
            this.label_Gap.AutoSize = true;
            this.label_Gap.Location = new System.Drawing.Point(7, 18);
            this.label_Gap.Name = "label_Gap";
            this.label_Gap.Size = new System.Drawing.Size(52, 13);
            this.label_Gap.TabIndex = 48;
            this.label_Gap.Text = "Gap layer";
            // 
            // textBox_Gap
            // 
            this.textBox_Gap.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_Gap.Location = new System.Drawing.Point(87, 15);
            this.textBox_Gap.Name = "textBox_Gap";
            this.textBox_Gap.Size = new System.Drawing.Size(165, 20);
            this.textBox_Gap.TabIndex = 47;
            this.textBox_Gap.Text = "GML-bldg-gap";
            this.textBox_Gap.TextChanged += new System.EventHandler(this.updateSettingTextBox);
            // 
            // button_FixGap
            // 
            this.button_FixGap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_FixGap.Location = new System.Drawing.Point(261, 13);
            this.button_FixGap.Name = "button_FixGap";
            this.button_FixGap.Size = new System.Drawing.Size(75, 48);
            this.button_FixGap.TabIndex = 46;
            this.button_FixGap.Text = "Automatic\r\nFix Gap";
            this.button_FixGap.UseVisualStyleBackColor = true;
            this.button_FixGap.Click += new System.EventHandler(this.button_FixGap_Click);
            // 
            // button_BrowseFile
            // 
            this.button_BrowseFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_BrowseFile.Location = new System.Drawing.Point(223, 474);
            this.button_BrowseFile.Name = "button_BrowseFile";
            this.button_BrowseFile.Size = new System.Drawing.Size(29, 23);
            this.button_BrowseFile.TabIndex = 36;
            this.button_BrowseFile.Text = "...";
            this.button_BrowseFile.UseVisualStyleBackColor = true;
            this.button_BrowseFile.Click += new System.EventHandler(this.button_BrowseFile_Click);
            // 
            // textBox_Output
            // 
            this.textBox_Output.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_Output.Location = new System.Drawing.Point(87, 474);
            this.textBox_Output.Name = "textBox_Output";
            this.textBox_Output.Size = new System.Drawing.Size(130, 20);
            this.textBox_Output.TabIndex = 35;
            // 
            // dataGridView_Model
            // 
            this.dataGridView_Model.AllowUserToDeleteRows = false;
            this.dataGridView_Model.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView_Model.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView_Model.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_Model.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnID,
            this.ColumnName,
            this.ColumnType,
            this.ColumnFunction,
            this.ColumnHeight,
            this.ColumnZElevation});
            this.dataGridView_Model.Location = new System.Drawing.Point(7, 96);
            this.dataGridView_Model.Name = "dataGridView_Model";
            this.dataGridView_Model.RowHeadersWidth = 51;
            this.dataGridView_Model.Size = new System.Drawing.Size(327, 316);
            this.dataGridView_Model.TabIndex = 34;
            this.dataGridView_Model.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_Model_CellContentClick);
            this.dataGridView_Model.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_Model_CellEndEdit);
            this.dataGridView_Model.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridView_Model_CurrentCellDirtyStateChanged);
            this.dataGridView_Model.SelectionChanged += new System.EventHandler(this.dataGridView_Model_SelectionChanged);
            this.dataGridView_Model.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dataGridView_Model_UserDeletingRow);
            // 
            // ColumnID
            // 
            this.ColumnID.DataPropertyName = "ID";
            this.ColumnID.HeaderText = "ID";
            this.ColumnID.MinimumWidth = 6;
            this.ColumnID.Name = "ColumnID";
            this.ColumnID.Width = 43;
            // 
            // ColumnName
            // 
            this.ColumnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ColumnName.DataPropertyName = "Name";
            this.ColumnName.HeaderText = "Name";
            this.ColumnName.MinimumWidth = 6;
            this.ColumnName.Name = "ColumnName";
            this.ColumnName.Width = 74;
            // 
            // ColumnType
            // 
            this.ColumnType.DataPropertyName = "Type";
            this.ColumnType.HeaderText = "Type";
            this.ColumnType.MinimumWidth = 6;
            this.ColumnType.Name = "ColumnType";
            this.ColumnType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ColumnType.Width = 56;
            // 
            // ColumnFunction
            // 
            this.ColumnFunction.DataPropertyName = "Function";
            this.ColumnFunction.HeaderText = "Function";
            this.ColumnFunction.Name = "ColumnFunction";
            this.ColumnFunction.Width = 54;
            // 
            // ColumnHeight
            // 
            this.ColumnHeight.DataPropertyName = "FloorHeight";
            this.ColumnHeight.HeaderText = "Height";
            this.ColumnHeight.MinimumWidth = 6;
            this.ColumnHeight.Name = "ColumnHeight";
            this.ColumnHeight.Width = 63;
            // 
            // ColumnZElevation
            // 
            this.ColumnZElevation.DataPropertyName = "Elevation";
            this.ColumnZElevation.HeaderText = "Elevation";
            this.ColumnZElevation.MinimumWidth = 6;
            this.ColumnZElevation.Name = "ColumnZElevation";
            this.ColumnZElevation.Width = 76;
            // 
            // button_Export
            // 
            this.button_Export.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Export.Location = new System.Drawing.Point(258, 474);
            this.button_Export.Name = "button_Export";
            this.button_Export.Size = new System.Drawing.Size(81, 49);
            this.button_Export.TabIndex = 32;
            this.button_Export.Text = "Export";
            this.button_Export.UseVisualStyleBackColor = true;
            this.button_Export.Click += new System.EventHandler(this.button_Export_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox1);
            this.tabPage3.Controls.Add(this.label_unit);
            this.tabPage3.Controls.Add(this.label_unit_1);
            this.tabPage3.Controls.Add(this.label_unit_2);
            this.tabPage3.Controls.Add(this.label12);
            this.tabPage3.Controls.Add(this.textBox_VerticalTransitionTolerance);
            this.tabPage3.Controls.Add(this.button_Create);
            this.tabPage3.Controls.Add(this.label_BoundaryLayerName);
            this.tabPage3.Controls.Add(this.textBox_RoomLayerName);
            this.tabPage3.Controls.Add(this.button_BrowseLog);
            this.tabPage3.Controls.Add(this.button_OpenLog);
            this.tabPage3.Controls.Add(this.label8);
            this.tabPage3.Controls.Add(this.textBox_LogFile);
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Controls.Add(this.textBox_RoomSeperate);
            this.tabPage3.Controls.Add(this.label_FloorHeight);
            this.tabPage3.Controls.Add(this.textBox_Height);
            this.tabPage3.Controls.Add(this.label3);
            this.tabPage3.Controls.Add(this.textBox_Elevation);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Controls.Add(this.textBox_Building);
            this.tabPage3.Controls.Add(this.label1);
            this.tabPage3.Controls.Add(this.textBox_LayerVirtualDoor);
            this.tabPage3.Controls.Add(this.label2);
            this.tabPage3.Controls.Add(this.textBox_FloorName);
            this.tabPage3.Controls.Add(this.label4);
            this.tabPage3.Controls.Add(this.comboBox_Unit);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(342, 529);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Setting";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBox_DefaultFunction);
            this.groupBox1.Controls.Add(this.comboBox_DefaultClass);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Location = new System.Drawing.Point(3, 256);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(267, 79);
            this.groupBox1.TabIndex = 79;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Default Space for [empty room type and door space]";
            // 
            // comboBox_DefaultFunction
            // 
            this.comboBox_DefaultFunction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_DefaultFunction.FormattingEnabled = true;
            this.comboBox_DefaultFunction.Location = new System.Drawing.Point(101, 47);
            this.comboBox_DefaultFunction.Name = "comboBox_DefaultFunction";
            this.comboBox_DefaultFunction.Size = new System.Drawing.Size(150, 21);
            this.comboBox_DefaultFunction.TabIndex = 87;
            // 
            // comboBox_DefaultClass
            // 
            this.comboBox_DefaultClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_DefaultClass.FormattingEnabled = true;
            this.comboBox_DefaultClass.Location = new System.Drawing.Point(102, 20);
            this.comboBox_DefaultClass.Name = "comboBox_DefaultClass";
            this.comboBox_DefaultClass.Size = new System.Drawing.Size(149, 21);
            this.comboBox_DefaultClass.TabIndex = 86;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 21);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(32, 13);
            this.label15.TabIndex = 83;
            this.label15.Text = "Class";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(5, 52);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(48, 13);
            this.label14.TabIndex = 84;
            this.label14.Text = "Function";
            // 
            // label_unit
            // 
            this.label_unit.AutoSize = true;
            this.label_unit.Location = new System.Drawing.Point(199, 93);
            this.label_unit.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_unit.Name = "label_unit";
            this.label_unit.Size = new System.Drawing.Size(55, 13);
            this.label_unit.TabIndex = 78;
            this.label_unit.Text = "model unit";
            // 
            // label_unit_1
            // 
            this.label_unit_1.AutoSize = true;
            this.label_unit_1.Location = new System.Drawing.Point(199, 120);
            this.label_unit_1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_unit_1.Name = "label_unit_1";
            this.label_unit_1.Size = new System.Drawing.Size(55, 13);
            this.label_unit_1.TabIndex = 77;
            this.label_unit_1.Text = "model unit";
            // 
            // label_unit_2
            // 
            this.label_unit_2.AutoSize = true;
            this.label_unit_2.Location = new System.Drawing.Point(199, 145);
            this.label_unit_2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_unit_2.Name = "label_unit_2";
            this.label_unit_2.Size = new System.Drawing.Size(55, 13);
            this.label_unit_2.TabIndex = 76;
            this.label_unit_2.Text = "model unit";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(9, 144);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(93, 13);
            this.label12.TabIndex = 75;
            this.label12.Text = "Vertical Tolerance";
            // 
            // textBox_VerticalTransitionTolerance
            // 
            this.textBox_VerticalTransitionTolerance.Location = new System.Drawing.Point(105, 141);
            this.textBox_VerticalTransitionTolerance.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_VerticalTransitionTolerance.Name = "textBox_VerticalTransitionTolerance";
            this.textBox_VerticalTransitionTolerance.Size = new System.Drawing.Size(91, 20);
            this.textBox_VerticalTransitionTolerance.TabIndex = 5;
            this.textBox_VerticalTransitionTolerance.Text = "1000";
            this.textBox_VerticalTransitionTolerance.TextChanged += new System.EventHandler(this.updateSettingTextBox);
            // 
            // button_Create
            // 
            this.button_Create.Location = new System.Drawing.Point(202, 168);
            this.button_Create.Margin = new System.Windows.Forms.Padding(2);
            this.button_Create.Name = "button_Create";
            this.button_Create.Size = new System.Drawing.Size(68, 82);
            this.button_Create.TabIndex = 10;
            this.button_Create.Text = "Create";
            this.button_Create.UseVisualStyleBackColor = true;
            this.button_Create.Click += new System.EventHandler(this.button_Create_Click);
            // 
            // label_BoundaryLayerName
            // 
            this.label_BoundaryLayerName.AutoSize = true;
            this.label_BoundaryLayerName.Location = new System.Drawing.Point(8, 229);
            this.label_BoundaryLayerName.Name = "label_BoundaryLayerName";
            this.label_BoundaryLayerName.Size = new System.Drawing.Size(60, 13);
            this.label_BoundaryLayerName.TabIndex = 72;
            this.label_BoundaryLayerName.Text = "Room layer";
            // 
            // textBox_RoomLayerName
            // 
            this.textBox_RoomLayerName.Location = new System.Drawing.Point(104, 228);
            this.textBox_RoomLayerName.Name = "textBox_RoomLayerName";
            this.textBox_RoomLayerName.Size = new System.Drawing.Size(92, 20);
            this.textBox_RoomLayerName.TabIndex = 9;
            this.textBox_RoomLayerName.Text = "GML-bldg-room";
            this.textBox_RoomLayerName.TextChanged += new System.EventHandler(this.updateSettingTextBox);
            // 
            // button_BrowseLog
            // 
            this.button_BrowseLog.Location = new System.Drawing.Point(213, 366);
            this.button_BrowseLog.Margin = new System.Windows.Forms.Padding(2);
            this.button_BrowseLog.Name = "button_BrowseLog";
            this.button_BrowseLog.Size = new System.Drawing.Size(56, 25);
            this.button_BrowseLog.TabIndex = 13;
            this.button_BrowseLog.Text = "Browse";
            this.button_BrowseLog.UseVisualStyleBackColor = true;
            this.button_BrowseLog.Click += new System.EventHandler(this.button_BrowseLog_Click);
            // 
            // button_OpenLog
            // 
            this.button_OpenLog.Location = new System.Drawing.Point(151, 364);
            this.button_OpenLog.Margin = new System.Windows.Forms.Padding(2);
            this.button_OpenLog.Name = "button_OpenLog";
            this.button_OpenLog.Size = new System.Drawing.Size(56, 25);
            this.button_OpenLog.TabIndex = 12;
            this.button_OpenLog.Text = "Open";
            this.button_OpenLog.UseVisualStyleBackColor = true;
            this.button_OpenLog.Click += new System.EventHandler(this.button_OpenLog_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 341);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 13);
            this.label8.TabIndex = 66;
            this.label8.Text = "Log file";
            // 
            // textBox_LogFile
            // 
            this.textBox_LogFile.Location = new System.Drawing.Point(104, 341);
            this.textBox_LogFile.Name = "textBox_LogFile";
            this.textBox_LogFile.Size = new System.Drawing.Size(165, 20);
            this.textBox_LogFile.TabIndex = 11;
            this.textBox_LogFile.TextChanged += new System.EventHandler(this.updateSettingTextBox);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 200);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 13);
            this.label6.TabIndex = 64;
            this.label6.Text = "Room seperator";
            // 
            // textBox_RoomSeperate
            // 
            this.textBox_RoomSeperate.Location = new System.Drawing.Point(105, 200);
            this.textBox_RoomSeperate.Name = "textBox_RoomSeperate";
            this.textBox_RoomSeperate.Size = new System.Drawing.Size(92, 20);
            this.textBox_RoomSeperate.TabIndex = 8;
            this.textBox_RoomSeperate.Text = "GML-bldg-CS";
            this.textBox_RoomSeperate.TextChanged += new System.EventHandler(this.updateSettingTextBox);
            // 
            // label_FloorHeight
            // 
            this.label_FloorHeight.AutoSize = true;
            this.label_FloorHeight.Location = new System.Drawing.Point(9, 120);
            this.label_FloorHeight.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_FloorHeight.Name = "label_FloorHeight";
            this.label_FloorHeight.Size = new System.Drawing.Size(64, 13);
            this.label_FloorHeight.TabIndex = 62;
            this.label_FloorHeight.Text = "Floor Height";
            // 
            // textBox_Height
            // 
            this.textBox_Height.Location = new System.Drawing.Point(105, 117);
            this.textBox_Height.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_Height.Name = "textBox_Height";
            this.textBox_Height.Size = new System.Drawing.Size(91, 20);
            this.textBox_Height.TabIndex = 4;
            this.textBox_Height.Text = "3000";
            this.textBox_Height.TextChanged += new System.EventHandler(this.updateSettingTextBox);
            this.textBox_Height.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_NotChar_Validating);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 93);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 60;
            this.label3.Text = "Elevation";
            // 
            // textBox_Elevation
            // 
            this.textBox_Elevation.Location = new System.Drawing.Point(105, 90);
            this.textBox_Elevation.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_Elevation.Name = "textBox_Elevation";
            this.textBox_Elevation.Size = new System.Drawing.Size(92, 20);
            this.textBox_Elevation.TabIndex = 3;
            this.textBox_Elevation.Text = "0";
            this.textBox_Elevation.TextChanged += new System.EventHandler(this.updateSettingTextBox);
            this.textBox_Elevation.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_NotChar_Validating);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 19);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Building name";
            // 
            // textBox_Building
            // 
            this.textBox_Building.Location = new System.Drawing.Point(105, 15);
            this.textBox_Building.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_Building.Name = "textBox_Building";
            this.textBox_Building.Size = new System.Drawing.Size(92, 20);
            this.textBox_Building.TabIndex = 1;
            this.textBox_Building.Text = "Building";
            this.textBox_Building.TextChanged += new System.EventHandler(this.updateSettingTextBox);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 174);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 56;
            this.label1.Text = "Virtual door";
            // 
            // textBox_LayerVirtualDoor
            // 
            this.textBox_LayerVirtualDoor.Location = new System.Drawing.Point(105, 170);
            this.textBox_LayerVirtualDoor.Name = "textBox_LayerVirtualDoor";
            this.textBox_LayerVirtualDoor.Size = new System.Drawing.Size(92, 20);
            this.textBox_LayerVirtualDoor.TabIndex = 7;
            this.textBox_LayerVirtualDoor.Text = "GML-bldg-pdoor";
            this.textBox_LayerVirtualDoor.TextChanged += new System.EventHandler(this.updateSettingTextBox);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 45);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 54;
            this.label2.Text = "Floor name";
            // 
            // textBox_FloorName
            // 
            this.textBox_FloorName.Location = new System.Drawing.Point(105, 41);
            this.textBox_FloorName.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_FloorName.Name = "textBox_FloorName";
            this.textBox_FloorName.Size = new System.Drawing.Size(92, 20);
            this.textBox_FloorName.TabIndex = 2;
            this.textBox_FloorName.Text = "1F";
            this.textBox_FloorName.TextChanged += new System.EventHandler(this.updateSettingTextBox);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 68);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 13);
            this.label4.TabIndex = 52;
            this.label4.Text = "Drawing Unit";
            // 
            // comboBox_Unit
            // 
            this.comboBox_Unit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Unit.FormattingEnabled = true;
            this.comboBox_Unit.Location = new System.Drawing.Point(104, 65);
            this.comboBox_Unit.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox_Unit.Name = "comboBox_Unit";
            this.comboBox_Unit.Size = new System.Drawing.Size(92, 21);
            this.comboBox_Unit.TabIndex = 6;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.button_AddFiles);
            this.tabPage2.Controls.Add(this.button_Browse);
            this.tabPage2.Controls.Add(this.textBox_CombineOutput);
            this.tabPage2.Controls.Add(this.button_Combines);
            this.tabPage2.Controls.Add(this.dataGridView_CombineFiles);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(342, 529);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Combines";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // button_AddFiles
            // 
            this.button_AddFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_AddFiles.Location = new System.Drawing.Point(187, 502);
            this.button_AddFiles.Margin = new System.Windows.Forms.Padding(2);
            this.button_AddFiles.Name = "button_AddFiles";
            this.button_AddFiles.Size = new System.Drawing.Size(71, 23);
            this.button_AddFiles.TabIndex = 5;
            this.button_AddFiles.Text = "Add files";
            this.button_AddFiles.UseVisualStyleBackColor = true;
            this.button_AddFiles.Click += new System.EventHandler(this.button_AddCombineFiles_Click);
            // 
            // button_Browse
            // 
            this.button_Browse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Browse.Location = new System.Drawing.Point(270, 474);
            this.button_Browse.Margin = new System.Windows.Forms.Padding(2);
            this.button_Browse.Name = "button_Browse";
            this.button_Browse.Size = new System.Drawing.Size(69, 23);
            this.button_Browse.TabIndex = 4;
            this.button_Browse.Text = "Browse";
            this.button_Browse.UseVisualStyleBackColor = true;
            this.button_Browse.Click += new System.EventHandler(this.button_Browse_Click);
            // 
            // textBox_CombineOutput
            // 
            this.textBox_CombineOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_CombineOutput.Location = new System.Drawing.Point(5, 476);
            this.textBox_CombineOutput.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_CombineOutput.Name = "textBox_CombineOutput";
            this.textBox_CombineOutput.Size = new System.Drawing.Size(254, 20);
            this.textBox_CombineOutput.TabIndex = 3;
            // 
            // button_Combines
            // 
            this.button_Combines.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Combines.Location = new System.Drawing.Point(268, 502);
            this.button_Combines.Margin = new System.Windows.Forms.Padding(2);
            this.button_Combines.Name = "button_Combines";
            this.button_Combines.Size = new System.Drawing.Size(71, 23);
            this.button_Combines.TabIndex = 2;
            this.button_Combines.Text = "Combines";
            this.button_Combines.UseVisualStyleBackColor = true;
            this.button_Combines.Click += new System.EventHandler(this.button_Combines_Click);
            // 
            // dataGridView_CombineFiles
            // 
            this.dataGridView_CombineFiles.AllowUserToAddRows = false;
            this.dataGridView_CombineFiles.AllowUserToOrderColumns = true;
            this.dataGridView_CombineFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView_CombineFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_CombineFiles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnFile,
            this.ColumnFloor,
            this.ColumnBuilding});
            this.dataGridView_CombineFiles.Location = new System.Drawing.Point(5, 13);
            this.dataGridView_CombineFiles.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridView_CombineFiles.Name = "dataGridView_CombineFiles";
            this.dataGridView_CombineFiles.ReadOnly = true;
            this.dataGridView_CombineFiles.RowHeadersWidth = 51;
            this.dataGridView_CombineFiles.RowTemplate.Height = 24;
            this.dataGridView_CombineFiles.Size = new System.Drawing.Size(334, 456);
            this.dataGridView_CombineFiles.TabIndex = 0;
            // 
            // ColumnFile
            // 
            this.ColumnFile.DataPropertyName = "FileName";
            this.ColumnFile.HeaderText = "File";
            this.ColumnFile.MinimumWidth = 6;
            this.ColumnFile.Name = "ColumnFile";
            this.ColumnFile.ReadOnly = true;
            this.ColumnFile.Width = 125;
            // 
            // ColumnFloor
            // 
            this.ColumnFloor.DataPropertyName = "Floor";
            this.ColumnFloor.HeaderText = "Floor";
            this.ColumnFloor.MinimumWidth = 6;
            this.ColumnFloor.Name = "ColumnFloor";
            this.ColumnFloor.ReadOnly = true;
            this.ColumnFloor.Width = 125;
            // 
            // ColumnBuilding
            // 
            this.ColumnBuilding.DataPropertyName = "Building";
            this.ColumnBuilding.HeaderText = "Building";
            this.ColumnBuilding.MinimumWidth = 6;
            this.ColumnBuilding.Name = "ColumnBuilding";
            this.ColumnBuilding.ReadOnly = true;
            this.ColumnBuilding.Width = 125;
            // 
            // combineMenu
            // 
            this.combineMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.combineMenu.Name = "combineMenu";
            this.combineMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // IndoorGMLControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "IndoorGMLControl";
            this.Size = new System.Drawing.Size(350, 555);
            this.Load += new System.EventHandler(this.IndoorGMLControl_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Model)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_CombineFiles)).EndInit();
            this.ResumeLayout(false);

        }

        private void dataGridView_Model_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataSource == null || dataSource.Count == 0 || dataSource.Count < e.RowIndex)
                return;

            dataSource[e.RowIndex].UpdateProperty();
        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button button_BrowseFile;
        private System.Windows.Forms.TextBox textBox_Output;
        private System.Windows.Forms.DataGridView dataGridView_Model;
        private System.Windows.Forms.Button button_CreateRoom;
        private System.Windows.Forms.Button button_Export;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label_Gap;
        private System.Windows.Forms.TextBox textBox_Gap;
        private System.Windows.Forms.Button button_FixGap;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_RoomSeperate;
        private System.Windows.Forms.Label label_FloorHeight;
        private System.Windows.Forms.TextBox textBox_Height;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_Elevation;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_Building;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_LayerVirtualDoor;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_FloorName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBox_Unit;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DataGridView dataGridView_CombineFiles;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnFile;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnFloor;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnBuilding;
        private System.Windows.Forms.Button button_Combines;
        private System.Windows.Forms.Button button_Browse;
        private System.Windows.Forms.TextBox textBox_CombineOutput;
        private System.Windows.Forms.Button button_AddFiles;
        private System.Windows.Forms.ContextMenuStrip combineMenu;
        private System.Windows.Forms.Label label_Tolerance;
        private System.Windows.Forms.TextBox textBox_GapTolerance;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox_LogFile;
        private System.Windows.Forms.Button button_OpenLog;
        private System.Windows.Forms.Button button_BrowseLog;
        private System.Windows.Forms.Button button_ClearRooms;
        private System.Windows.Forms.Button button_RoomList;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label_BoundaryLayerName;
        private System.Windows.Forms.TextBox textBox_RoomLayerName;
        private System.Windows.Forms.Button button_DeleteSpace;
        private System.Windows.Forms.Button button_Create;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBox_FileName;
        private System.Windows.Forms.Label label_unit;
        private System.Windows.Forms.Label label_unit_1;
        private System.Windows.Forms.Label label_unit_2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBox_VerticalTransitionTolerance;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewComboBoxColumn ColumnType;
        private System.Windows.Forms.DataGridViewComboBoxColumn ColumnFunction;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnHeight;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnZElevation;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox comboBox_DefaultFunction;
        private System.Windows.Forms.ComboBox comboBox_DefaultClass;
    }
}
