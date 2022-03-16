
namespace VariScan
{
    partial class FormSessionManager
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.StartScanButton = new System.Windows.Forms.Button();
            this.CloseButton = new System.Windows.Forms.Button();
            this.ExposureTimeSetting = new System.Windows.Forms.NumericUpDown();
            this.MinAltitudeSetting = new System.Windows.Forms.NumericUpDown();
            this.MinAltitudeLabel = new System.Windows.Forms.Label();
            this.ExposureTimeLabel = new System.Windows.Forms.Label();
            this.TargetCountLabel = new System.Windows.Forms.Label();
            this.CurrentTargetLabel = new System.Windows.Forms.Label();
            this.CurrentTargetCount = new System.Windows.Forms.Label();
            this.CurrentTargetName = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.CurrentTargetFilter = new System.Windows.Forms.Label();
            this.CollectionGroupBox = new System.Windows.Forms.GroupBox();
            this.SetCollectionButton = new System.Windows.Forms.Button();
            this.ConfigurationGroupBox = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.ImagesPerSampleBox = new System.Windows.Forms.NumericUpDown();
            this.ExposureGroupBox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.AutoExposureCheckBox = new System.Windows.Forms.CheckBox();
            this.MaxADUBox = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.RetakeIntervalBox = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.DomeCheckBox = new System.Windows.Forms.CheckBox();
            this.ReductionListBox = new System.Windows.Forms.ComboBox();
            this.WatchWeatherCheckBox = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.CCDTemperatureSetting = new System.Windows.Forms.NumericUpDown();
            this.AutoRunCheckBox = new System.Windows.Forms.CheckBox();
            this.AutoFocusGroupBox = new System.Windows.Forms.GroupBox();
            this.FocusPresetBox = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.AtFocusTypeBox = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.FocusFilterBox = new System.Windows.Forms.ComboBox();
            this.OnTopCheckBox = new System.Windows.Forms.CheckBox();
            this.LogBox = new System.Windows.Forms.TextBox();
            this.AbortButton = new System.Windows.Forms.Button();
            this.AnalyzeButton = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.CLSReductionBox = new System.Windows.Forms.ComboBox();
            this.EnableCLSBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.ExposureTimeSetting)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinAltitudeSetting)).BeginInit();
            this.CollectionGroupBox.SuspendLayout();
            this.ConfigurationGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImagesPerSampleBox)).BeginInit();
            this.ExposureGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MaxADUBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RetakeIntervalBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CCDTemperatureSetting)).BeginInit();
            this.AutoFocusGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // StartScanButton
            // 
            this.StartScanButton.BackColor = System.Drawing.Color.LightGreen;
            this.StartScanButton.Location = new System.Drawing.Point(12, 587);
            this.StartScanButton.Name = "StartScanButton";
            this.StartScanButton.Size = new System.Drawing.Size(75, 23);
            this.StartScanButton.TabIndex = 0;
            this.StartScanButton.Text = "Imaging";
            this.StartScanButton.UseVisualStyleBackColor = false;
            this.StartScanButton.Click += new System.EventHandler(this.StartScanButton_Click);
            // 
            // CloseButton
            // 
            this.CloseButton.BackColor = System.Drawing.Color.LightGreen;
            this.CloseButton.Location = new System.Drawing.Point(212, 618);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(75, 23);
            this.CloseButton.TabIndex = 1;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = false;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // ExposureTimeSetting
            // 
            this.ExposureTimeSetting.Location = new System.Drawing.Point(189, 17);
            this.ExposureTimeSetting.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.ExposureTimeSetting.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ExposureTimeSetting.Name = "ExposureTimeSetting";
            this.ExposureTimeSetting.Size = new System.Drawing.Size(61, 20);
            this.ExposureTimeSetting.TabIndex = 2;
            this.ExposureTimeSetting.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ExposureTimeSetting.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.ExposureTimeSetting.ValueChanged += new System.EventHandler(this.ExposureTimeSetting_ValueChanged);
            // 
            // MinAltitudeSetting
            // 
            this.MinAltitudeSetting.Location = new System.Drawing.Point(189, 89);
            this.MinAltitudeSetting.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.MinAltitudeSetting.Name = "MinAltitudeSetting";
            this.MinAltitudeSetting.Size = new System.Drawing.Size(70, 20);
            this.MinAltitudeSetting.TabIndex = 3;
            this.MinAltitudeSetting.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.MinAltitudeSetting.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.MinAltitudeSetting.ValueChanged += new System.EventHandler(this.MinAltitudeSetting_ValueChanged);
            // 
            // MinAltitudeLabel
            // 
            this.MinAltitudeLabel.AutoSize = true;
            this.MinAltitudeLabel.Location = new System.Drawing.Point(11, 91);
            this.MinAltitudeLabel.Name = "MinAltitudeLabel";
            this.MinAltitudeLabel.Size = new System.Drawing.Size(169, 13);
            this.MinAltitudeLabel.TabIndex = 4;
            this.MinAltitudeLabel.Text = "Minimum Target Altitude (Degrees)";
            this.MinAltitudeLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ExposureTimeLabel
            // 
            this.ExposureTimeLabel.AutoSize = true;
            this.ExposureTimeLabel.Location = new System.Drawing.Point(6, 19);
            this.ExposureTimeLabel.Name = "ExposureTimeLabel";
            this.ExposureTimeLabel.Size = new System.Drawing.Size(149, 13);
            this.ExposureTimeLabel.TabIndex = 5;
            this.ExposureTimeLabel.Text = "Maximum Exposure (Seconds)";
            // 
            // TargetCountLabel
            // 
            this.TargetCountLabel.AutoSize = true;
            this.TargetCountLabel.Location = new System.Drawing.Point(25, 19);
            this.TargetCountLabel.Name = "TargetCountLabel";
            this.TargetCountLabel.Size = new System.Drawing.Size(102, 13);
            this.TargetCountLabel.TabIndex = 6;
            this.TargetCountLabel.Text = "Prospective Targets";
            // 
            // CurrentTargetLabel
            // 
            this.CurrentTargetLabel.AutoSize = true;
            this.CurrentTargetLabel.Location = new System.Drawing.Point(25, 42);
            this.CurrentTargetLabel.Name = "CurrentTargetLabel";
            this.CurrentTargetLabel.Size = new System.Drawing.Size(75, 13);
            this.CurrentTargetLabel.TabIndex = 7;
            this.CurrentTargetLabel.Text = "Current Target";
            // 
            // CurrentTargetCount
            // 
            this.CurrentTargetCount.AutoSize = true;
            this.CurrentTargetCount.Location = new System.Drawing.Point(129, 19);
            this.CurrentTargetCount.Name = "CurrentTargetCount";
            this.CurrentTargetCount.Size = new System.Drawing.Size(29, 13);
            this.CurrentTargetCount.TabIndex = 10;
            this.CurrentTargetCount.Text = "TBD";
            // 
            // CurrentTargetName
            // 
            this.CurrentTargetName.AutoSize = true;
            this.CurrentTargetName.Location = new System.Drawing.Point(129, 42);
            this.CurrentTargetName.Name = "CurrentTargetName";
            this.CurrentTargetName.Size = new System.Drawing.Size(29, 13);
            this.CurrentTargetName.TabIndex = 11;
            this.CurrentTargetName.Text = "TBD";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Current Filter";
            // 
            // CurrentTargetFilter
            // 
            this.CurrentTargetFilter.AutoSize = true;
            this.CurrentTargetFilter.Location = new System.Drawing.Point(129, 67);
            this.CurrentTargetFilter.Name = "CurrentTargetFilter";
            this.CurrentTargetFilter.Size = new System.Drawing.Size(29, 13);
            this.CurrentTargetFilter.TabIndex = 14;
            this.CurrentTargetFilter.Text = "TBD";
            // 
            // CollectionGroupBox
            // 
            this.CollectionGroupBox.BackColor = System.Drawing.Color.LightSeaGreen;
            this.CollectionGroupBox.Controls.Add(this.SetCollectionButton);
            this.CollectionGroupBox.Controls.Add(this.CurrentTargetFilter);
            this.CollectionGroupBox.Controls.Add(this.label2);
            this.CollectionGroupBox.Controls.Add(this.TargetCountLabel);
            this.CollectionGroupBox.Controls.Add(this.CurrentTargetName);
            this.CollectionGroupBox.Controls.Add(this.CurrentTargetLabel);
            this.CollectionGroupBox.Controls.Add(this.CurrentTargetCount);
            this.CollectionGroupBox.ForeColor = System.Drawing.Color.White;
            this.CollectionGroupBox.Location = new System.Drawing.Point(11, 12);
            this.CollectionGroupBox.Name = "CollectionGroupBox";
            this.CollectionGroupBox.Size = new System.Drawing.Size(277, 94);
            this.CollectionGroupBox.TabIndex = 15;
            this.CollectionGroupBox.TabStop = false;
            this.CollectionGroupBox.Text = "Collection";
            // 
            // SetCollectionButton
            // 
            this.SetCollectionButton.BackColor = System.Drawing.Color.LightGreen;
            this.SetCollectionButton.ForeColor = System.Drawing.Color.Black;
            this.SetCollectionButton.Location = new System.Drawing.Point(196, 17);
            this.SetCollectionButton.Name = "SetCollectionButton";
            this.SetCollectionButton.Size = new System.Drawing.Size(75, 63);
            this.SetCollectionButton.TabIndex = 15;
            this.SetCollectionButton.Text = "Choose Another Collection";
            this.SetCollectionButton.UseVisualStyleBackColor = false;
            this.SetCollectionButton.Click += new System.EventHandler(this.SetCollectionButton_Click);
            // 
            // ConfigurationGroupBox
            // 
            this.ConfigurationGroupBox.BackColor = System.Drawing.Color.LightSeaGreen;
            this.ConfigurationGroupBox.Controls.Add(this.EnableCLSBox);
            this.ConfigurationGroupBox.Controls.Add(this.CLSReductionBox);
            this.ConfigurationGroupBox.Controls.Add(this.label9);
            this.ConfigurationGroupBox.Controls.Add(this.label6);
            this.ConfigurationGroupBox.Controls.Add(this.ImagesPerSampleBox);
            this.ConfigurationGroupBox.Controls.Add(this.ExposureGroupBox);
            this.ConfigurationGroupBox.Controls.Add(this.label4);
            this.ConfigurationGroupBox.Controls.Add(this.RetakeIntervalBox);
            this.ConfigurationGroupBox.Controls.Add(this.label5);
            this.ConfigurationGroupBox.Controls.Add(this.DomeCheckBox);
            this.ConfigurationGroupBox.Controls.Add(this.ReductionListBox);
            this.ConfigurationGroupBox.Controls.Add(this.WatchWeatherCheckBox);
            this.ConfigurationGroupBox.Controls.Add(this.label3);
            this.ConfigurationGroupBox.Controls.Add(this.CCDTemperatureSetting);
            this.ConfigurationGroupBox.Controls.Add(this.AutoRunCheckBox);
            this.ConfigurationGroupBox.Controls.Add(this.MinAltitudeLabel);
            this.ConfigurationGroupBox.Controls.Add(this.MinAltitudeSetting);
            this.ConfigurationGroupBox.Controls.Add(this.AutoFocusGroupBox);
            this.ConfigurationGroupBox.ForeColor = System.Drawing.Color.White;
            this.ConfigurationGroupBox.Location = new System.Drawing.Point(12, 112);
            this.ConfigurationGroupBox.Name = "ConfigurationGroupBox";
            this.ConfigurationGroupBox.Size = new System.Drawing.Size(276, 328);
            this.ConfigurationGroupBox.TabIndex = 16;
            this.ConfigurationGroupBox.TabStop = false;
            this.ConfigurationGroupBox.Text = "Configuration";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(10, 43);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(181, 23);
            this.label6.TabIndex = 35;
            this.label6.Text = "Images per Sample";
            // 
            // ImagesPerSampleBox
            // 
            this.ImagesPerSampleBox.Location = new System.Drawing.Point(188, 41);
            this.ImagesPerSampleBox.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.ImagesPerSampleBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ImagesPerSampleBox.Name = "ImagesPerSampleBox";
            this.ImagesPerSampleBox.Size = new System.Drawing.Size(70, 20);
            this.ImagesPerSampleBox.TabIndex = 34;
            this.ImagesPerSampleBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ImagesPerSampleBox.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ImagesPerSampleBox.ValueChanged += new System.EventHandler(this.ImagesPerSampleBox_ValueChanged);
            // 
            // ExposureGroupBox
            // 
            this.ExposureGroupBox.Controls.Add(this.label1);
            this.ExposureGroupBox.Controls.Add(this.AutoExposureCheckBox);
            this.ExposureGroupBox.Controls.Add(this.MaxADUBox);
            this.ExposureGroupBox.Controls.Add(this.ExposureTimeSetting);
            this.ExposureGroupBox.Controls.Add(this.ExposureTimeLabel);
            this.ExposureGroupBox.ForeColor = System.Drawing.Color.White;
            this.ExposureGroupBox.Location = new System.Drawing.Point(7, 182);
            this.ExposureGroupBox.Name = "ExposureGroupBox";
            this.ExposureGroupBox.Size = new System.Drawing.Size(262, 68);
            this.ExposureGroupBox.TabIndex = 33;
            this.ExposureGroupBox.TabStop = false;
            this.ExposureGroupBox.Text = "Exposure";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(127, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Max ADU";
            // 
            // AutoExposureCheckBox
            // 
            this.AutoExposureCheckBox.AutoSize = true;
            this.AutoExposureCheckBox.Location = new System.Drawing.Point(11, 43);
            this.AutoExposureCheckBox.Name = "AutoExposureCheckBox";
            this.AutoExposureCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.AutoExposureCheckBox.Size = new System.Drawing.Size(83, 17);
            this.AutoExposureCheckBox.TabIndex = 34;
            this.AutoExposureCheckBox.Text = "AutoExpose";
            this.AutoExposureCheckBox.UseVisualStyleBackColor = true;
            this.AutoExposureCheckBox.CheckedChanged += new System.EventHandler(this.AutoExposureCheckBox_CheckedChanged);
            // 
            // MaxADUBox
            // 
            this.MaxADUBox.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.MaxADUBox.Location = new System.Drawing.Point(190, 42);
            this.MaxADUBox.Maximum = new decimal(new int[] {
            65000,
            0,
            0,
            0});
            this.MaxADUBox.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.MaxADUBox.Name = "MaxADUBox";
            this.MaxADUBox.Size = new System.Drawing.Size(60, 20);
            this.MaxADUBox.TabIndex = 34;
            this.MaxADUBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.MaxADUBox.Value = new decimal(new int[] {
            24000,
            0,
            0,
            0});
            this.MaxADUBox.ValueChanged += new System.EventHandler(this.MaxADUBox_ValueChanged);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(10, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(181, 23);
            this.label4.TabIndex = 28;
            this.label4.Text = "Min Interval Between Samples (hrs)";
            // 
            // RetakeIntervalBox
            // 
            this.RetakeIntervalBox.DecimalPlaces = 1;
            this.RetakeIntervalBox.Location = new System.Drawing.Point(188, 65);
            this.RetakeIntervalBox.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.RetakeIntervalBox.Name = "RetakeIntervalBox";
            this.RetakeIntervalBox.Size = new System.Drawing.Size(70, 20);
            this.RetakeIntervalBox.TabIndex = 27;
            this.RetakeIntervalBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.RetakeIntervalBox.ValueChanged += new System.EventHandler(this.RetakeIntervalBox_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 140);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(115, 13);
            this.label5.TabIndex = 25;
            this.label5.Text = "Image Reduction Type";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // DomeCheckBox
            // 
            this.DomeCheckBox.AutoSize = true;
            this.DomeCheckBox.Location = new System.Drawing.Point(197, 19);
            this.DomeCheckBox.Name = "DomeCheckBox";
            this.DomeCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.DomeCheckBox.Size = new System.Drawing.Size(76, 17);
            this.DomeCheckBox.TabIndex = 20;
            this.DomeCheckBox.Text = "Has Dome";
            this.DomeCheckBox.UseVisualStyleBackColor = true;
            this.DomeCheckBox.CheckedChanged += new System.EventHandler(this.DomeCheckBox_CheckedChanged);
            // 
            // ReductionListBox
            // 
            this.ReductionListBox.FormattingEnabled = true;
            this.ReductionListBox.Items.AddRange(new object[] {
            "None",
            "Auto Dark",
            "Full"});
            this.ReductionListBox.Location = new System.Drawing.Point(188, 137);
            this.ReductionListBox.Name = "ReductionListBox";
            this.ReductionListBox.Size = new System.Drawing.Size(70, 21);
            this.ReductionListBox.TabIndex = 24;
            this.ReductionListBox.Text = "None";
            this.ReductionListBox.SelectedIndexChanged += new System.EventHandler(this.ReductionListBox_SelectedIndexChanged);
            // 
            // WatchWeatherCheckBox
            // 
            this.WatchWeatherCheckBox.AutoSize = true;
            this.WatchWeatherCheckBox.Location = new System.Drawing.Point(87, 19);
            this.WatchWeatherCheckBox.Name = "WatchWeatherCheckBox";
            this.WatchWeatherCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.WatchWeatherCheckBox.Size = new System.Drawing.Size(102, 17);
            this.WatchWeatherCheckBox.TabIndex = 19;
            this.WatchWeatherCheckBox.Text = "Watch Weather";
            this.WatchWeatherCheckBox.UseVisualStyleBackColor = true;
            this.WatchWeatherCheckBox.CheckedChanged += new System.EventHandler(this.WatchWeatherCheckBox_CheckedChanged);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(10, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(138, 23);
            this.label3.TabIndex = 26;
            this.label3.Text = "Camera Temperature";
            // 
            // CCDTemperatureSetting
            // 
            this.CCDTemperatureSetting.Location = new System.Drawing.Point(188, 113);
            this.CCDTemperatureSetting.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.CCDTemperatureSetting.Name = "CCDTemperatureSetting";
            this.CCDTemperatureSetting.Size = new System.Drawing.Size(70, 20);
            this.CCDTemperatureSetting.TabIndex = 16;
            this.CCDTemperatureSetting.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.CCDTemperatureSetting.Value = new decimal(new int[] {
            20,
            0,
            0,
            -2147483648});
            this.CCDTemperatureSetting.ValueChanged += new System.EventHandler(this.CCDTemperatureSetting_ValueChanged);
            // 
            // AutoRunCheckBox
            // 
            this.AutoRunCheckBox.AutoSize = true;
            this.AutoRunCheckBox.Location = new System.Drawing.Point(13, 19);
            this.AutoRunCheckBox.Name = "AutoRunCheckBox";
            this.AutoRunCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.AutoRunCheckBox.Size = new System.Drawing.Size(68, 17);
            this.AutoRunCheckBox.TabIndex = 12;
            this.AutoRunCheckBox.Text = "AutoRun";
            this.AutoRunCheckBox.UseVisualStyleBackColor = true;
            this.AutoRunCheckBox.CheckedChanged += new System.EventHandler(this.AutoStartCheckBox_CheckedChanged);
            // 
            // AutoFocusGroupBox
            // 
            this.AutoFocusGroupBox.Controls.Add(this.FocusPresetBox);
            this.AutoFocusGroupBox.Controls.Add(this.label8);
            this.AutoFocusGroupBox.Controls.Add(this.AtFocusTypeBox);
            this.AutoFocusGroupBox.Controls.Add(this.label7);
            this.AutoFocusGroupBox.Controls.Add(this.FocusFilterBox);
            this.AutoFocusGroupBox.ForeColor = System.Drawing.Color.White;
            this.AutoFocusGroupBox.Location = new System.Drawing.Point(6, 256);
            this.AutoFocusGroupBox.Name = "AutoFocusGroupBox";
            this.AutoFocusGroupBox.Size = new System.Drawing.Size(263, 66);
            this.AutoFocusGroupBox.TabIndex = 32;
            this.AutoFocusGroupBox.TabStop = false;
            this.AutoFocusGroupBox.Text = "AutoFocus";
            // 
            // FocusPresetBox
            // 
            this.FocusPresetBox.AutoSize = true;
            this.FocusPresetBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.FocusPresetBox.Location = new System.Drawing.Point(12, 41);
            this.FocusPresetBox.Name = "FocusPresetBox";
            this.FocusPresetBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.FocusPresetBox.Size = new System.Drawing.Size(56, 17);
            this.FocusPresetBox.TabIndex = 35;
            this.FocusPresetBox.Text = "Preset";
            this.FocusPresetBox.UseVisualStyleBackColor = true;
            this.FocusPresetBox.CheckedChanged += new System.EventHandler(this.FocusPresetBox_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(111, 16);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(31, 13);
            this.label8.TabIndex = 37;
            this.label8.Text = "Type";
            // 
            // AtFocusTypeBox
            // 
            this.AtFocusTypeBox.FormattingEnabled = true;
            this.AtFocusTypeBox.Items.AddRange(new object[] {
            "None",
            "@Focus2",
            "@Focus3"});
            this.AtFocusTypeBox.Location = new System.Drawing.Point(148, 12);
            this.AtFocusTypeBox.Name = "AtFocusTypeBox";
            this.AtFocusTypeBox.Size = new System.Drawing.Size(103, 21);
            this.AtFocusTypeBox.TabIndex = 36;
            this.AtFocusTypeBox.SelectedIndexChanged += new System.EventHandler(this.AtFocusBox_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(111, 42);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 13);
            this.label7.TabIndex = 35;
            this.label7.Text = "Focus Filter Index";
            // 
            // FocusFilterBox
            // 
            this.FocusFilterBox.FormattingEnabled = true;
            this.FocusFilterBox.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8"});
            this.FocusFilterBox.Location = new System.Drawing.Point(207, 39);
            this.FocusFilterBox.Name = "FocusFilterBox";
            this.FocusFilterBox.Size = new System.Drawing.Size(44, 21);
            this.FocusFilterBox.TabIndex = 32;
            this.FocusFilterBox.SelectedIndexChanged += new System.EventHandler(this.FocusFilterBox_SelectedIndexChanged);
            // 
            // OnTopCheckBox
            // 
            this.OnTopCheckBox.AutoSize = true;
            this.OnTopCheckBox.ForeColor = System.Drawing.Color.White;
            this.OnTopCheckBox.Location = new System.Drawing.Point(106, 622);
            this.OnTopCheckBox.Name = "OnTopCheckBox";
            this.OnTopCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.OnTopCheckBox.Size = new System.Drawing.Size(86, 17);
            this.OnTopCheckBox.TabIndex = 13;
            this.OnTopCheckBox.Text = "Stay On Top";
            this.OnTopCheckBox.UseVisualStyleBackColor = true;
            this.OnTopCheckBox.CheckedChanged += new System.EventHandler(this.OnTopCheckBox_CheckedChanged);
            // 
            // LogBox
            // 
            this.LogBox.BackColor = System.Drawing.Color.LightSeaGreen;
            this.LogBox.ForeColor = System.Drawing.Color.White;
            this.LogBox.Location = new System.Drawing.Point(10, 446);
            this.LogBox.Multiline = true;
            this.LogBox.Name = "LogBox";
            this.LogBox.ReadOnly = true;
            this.LogBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LogBox.Size = new System.Drawing.Size(277, 135);
            this.LogBox.TabIndex = 18;
            this.LogBox.Text = "Session Log";
            // 
            // AbortButton
            // 
            this.AbortButton.BackColor = System.Drawing.Color.LightGreen;
            this.AbortButton.Location = new System.Drawing.Point(212, 587);
            this.AbortButton.Name = "AbortButton";
            this.AbortButton.Size = new System.Drawing.Size(75, 23);
            this.AbortButton.TabIndex = 20;
            this.AbortButton.Text = "Abort";
            this.AbortButton.UseVisualStyleBackColor = false;
            this.AbortButton.Click += new System.EventHandler(this.AbortButton_Click);
            // 
            // AnalyzeButton
            // 
            this.AnalyzeButton.BackColor = System.Drawing.Color.LightGreen;
            this.AnalyzeButton.Location = new System.Drawing.Point(12, 618);
            this.AnalyzeButton.Name = "AnalyzeButton";
            this.AnalyzeButton.Size = new System.Drawing.Size(75, 23);
            this.AnalyzeButton.TabIndex = 27;
            this.AnalyzeButton.Text = "Photometry";
            this.AnalyzeButton.UseVisualStyleBackColor = false;
            this.AnalyzeButton.Click += new System.EventHandler(this.AnalyzeButton_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(104, 166);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(79, 13);
            this.label9.TabIndex = 36;
            this.label9.Text = "CLS Reduction";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // CLSReductionBox
            // 
            this.CLSReductionBox.FormattingEnabled = true;
            this.CLSReductionBox.Items.AddRange(new object[] {
            "None",
            "Auto Dark",
            "Full"});
            this.CLSReductionBox.Location = new System.Drawing.Point(188, 163);
            this.CLSReductionBox.Name = "CLSReductionBox";
            this.CLSReductionBox.Size = new System.Drawing.Size(70, 21);
            this.CLSReductionBox.TabIndex = 37;
            this.CLSReductionBox.Text = "None";
            // 
            // EnableCLSBox
            // 
            this.EnableCLSBox.AutoSize = true;
            this.EnableCLSBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.EnableCLSBox.Location = new System.Drawing.Point(7, 165);
            this.EnableCLSBox.Name = "EnableCLSBox";
            this.EnableCLSBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.EnableCLSBox.Size = new System.Drawing.Size(82, 17);
            this.EnableCLSBox.TabIndex = 38;
            this.EnableCLSBox.Text = "Enable CLS";
            this.EnableCLSBox.UseVisualStyleBackColor = true;
            this.EnableCLSBox.CheckedChanged += new System.EventHandler(this.EnableCLSBox_CheckedChanged);
            // 
            // FormSessionManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkCyan;
            this.ClientSize = new System.Drawing.Size(296, 652);
            this.Controls.Add(this.AnalyzeButton);
            this.Controls.Add(this.AbortButton);
            this.Controls.Add(this.LogBox);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.StartScanButton);
            this.Controls.Add(this.CollectionGroupBox);
            this.Controls.Add(this.ConfigurationGroupBox);
            this.Controls.Add(this.OnTopCheckBox);
            this.ForeColor = System.Drawing.Color.Black;
            this.Location = new System.Drawing.Point(1200, 120);
            this.Name = "FormSessionManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "  ";
            ((System.ComponentModel.ISupportInitialize)(this.ExposureTimeSetting)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinAltitudeSetting)).EndInit();
            this.CollectionGroupBox.ResumeLayout(false);
            this.CollectionGroupBox.PerformLayout();
            this.ConfigurationGroupBox.ResumeLayout(false);
            this.ConfigurationGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImagesPerSampleBox)).EndInit();
            this.ExposureGroupBox.ResumeLayout(false);
            this.ExposureGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MaxADUBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RetakeIntervalBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CCDTemperatureSetting)).EndInit();
            this.AutoFocusGroupBox.ResumeLayout(false);
            this.AutoFocusGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button StartScanButton;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.NumericUpDown ExposureTimeSetting;
        private System.Windows.Forms.NumericUpDown MinAltitudeSetting;
        private System.Windows.Forms.Label MinAltitudeLabel;
        private System.Windows.Forms.Label ExposureTimeLabel;
        private System.Windows.Forms.Label TargetCountLabel;
        private System.Windows.Forms.Label CurrentTargetLabel;
        private System.Windows.Forms.Label CurrentTargetCount;
        private System.Windows.Forms.Label CurrentTargetName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label CurrentTargetFilter;
        private System.Windows.Forms.GroupBox CollectionGroupBox;
        private System.Windows.Forms.GroupBox ConfigurationGroupBox;
        private System.Windows.Forms.TextBox LogBox;
        private System.Windows.Forms.CheckBox AutoRunCheckBox;
        private System.Windows.Forms.CheckBox OnTopCheckBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown CCDTemperatureSetting;
        private System.Windows.Forms.Button AbortButton;
        private System.Windows.Forms.CheckBox WatchWeatherCheckBox;
        private System.Windows.Forms.CheckBox DomeCheckBox;
        private System.Windows.Forms.ComboBox ReductionListBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button AnalyzeButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown RetakeIntervalBox;
        private System.Windows.Forms.GroupBox AutoFocusGroupBox;
        private System.Windows.Forms.Button SetCollectionButton;
        private System.Windows.Forms.GroupBox ExposureGroupBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox AutoExposureCheckBox;
        private System.Windows.Forms.NumericUpDown MaxADUBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown ImagesPerSampleBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox FocusFilterBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox AtFocusTypeBox;
        private System.Windows.Forms.CheckBox FocusPresetBox;
        private System.Windows.Forms.CheckBox EnableCLSBox;
        private System.Windows.Forms.ComboBox CLSReductionBox;
        private System.Windows.Forms.Label label9;
    }
}

