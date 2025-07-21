namespace Virtual_DM400
{
    partial class DM400
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ButtonLevelTank = new Button();
            TextBoxLevelTankZero = new TextBox();
            TextBoxLevelTankLimit = new TextBox();
            LabelLevelTankZero = new Label();
            LabelLevelTankLimit = new Label();
            ButtonBuildPlatform = new Button();
            LabelBuildPlatformPositionTop = new Label();
            TextBoxBuildPlatformPositionTop = new TextBox();
            LabelBuildPlatformPositionOrigin = new Label();
            TextBoxBuildPlatformPositionOrigin = new TextBox();
            LabelBuildPlatformPositionLimitA = new Label();
            TextBoxBuildPlatformPositionLimitA = new TextBox();
            LabelBuildPlatformPositionLimitB = new Label();
            TextBoxBuildPlatformPositionLimitB = new TextBox();
            ButtonPrintBlade = new Button();
            ButtonCollectBlade = new Button();
            LabelCollectBladeZero = new Label();
            TextBoxCollectBladeZero = new TextBox();
            LabelCollectBladeLimit = new Label();
            TextBoxCollectBladeLimit = new TextBox();
            LabelPrintBladeLimit = new Label();
            TextBoxPrintBladeLimit = new TextBox();
            LabelPrintBladeZero = new Label();
            TextBoxPrintBladeZero = new TextBox();
            dataGridView_PortConfiguration = new DataGridView();
            DeviceName = new DataGridViewTextBoxColumn();
            PortName = new DataGridViewTextBoxColumn();
            BaudRate = new DataGridViewTextBoxColumn();
            Parity = new DataGridViewTextBoxColumn();
            DataBits = new DataGridViewTextBoxColumn();
            StopBits = new DataGridViewTextBoxColumn();
            logText = new RichTextBox();
            buttonClear = new Button();
            buttonPortClose = new Button();
            buttonPortOpen = new Button();
            PanelStage = new Panel();
            PanelWaterLevel = new Panel();
            TextBoxCurrentWaterLevel = new TextBox();
            LabelCurrentWaterLevel = new Label();
            TextBoxCurrentWaterLevelMin = new TextBox();
            TextBoxCurrentWaterLevelMax = new TextBox();
            LabelCurrentWaterLevelRange = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView_PortConfiguration).BeginInit();
            PanelStage.SuspendLayout();
            PanelWaterLevel.SuspendLayout();
            SuspendLayout();
            // 
            // ButtonLevelTank
            // 
            ButtonLevelTank.ForeColor = Color.Blue;
            ButtonLevelTank.Location = new Point(196, 31);
            ButtonLevelTank.Name = "ButtonLevelTank";
            ButtonLevelTank.Size = new Size(109, 204);
            ButtonLevelTank.TabIndex = 0;
            ButtonLevelTank.Text = "Level Tank";
            ButtonLevelTank.UseVisualStyleBackColor = true;
            // 
            // TextBoxLevelTankZero
            // 
            TextBoxLevelTankZero.Location = new Point(66, 31);
            TextBoxLevelTankZero.Name = "TextBoxLevelTankZero";
            TextBoxLevelTankZero.Size = new Size(100, 23);
            TextBoxLevelTankZero.TabIndex = 1;
            TextBoxLevelTankZero.Text = "0";
            // 
            // TextBoxLevelTankLimit
            // 
            TextBoxLevelTankLimit.Location = new Point(66, 183);
            TextBoxLevelTankLimit.Name = "TextBoxLevelTankLimit";
            TextBoxLevelTankLimit.Size = new Size(100, 23);
            TextBoxLevelTankLimit.TabIndex = 2;
            TextBoxLevelTankLimit.Text = "300";
            // 
            // LabelLevelTankZero
            // 
            LabelLevelTankZero.AutoSize = true;
            LabelLevelTankZero.Location = new Point(22, 57);
            LabelLevelTankZero.Name = "LabelLevelTankZero";
            LabelLevelTankZero.Size = new Size(141, 15);
            LabelLevelTankZero.TabIndex = 3;
            LabelLevelTankZero.Text = "Level Tank Position: Zero";
            // 
            // LabelLevelTankLimit
            // 
            LabelLevelTankLimit.AutoSize = true;
            LabelLevelTankLimit.Location = new Point(22, 209);
            LabelLevelTankLimit.Name = "LabelLevelTankLimit";
            LabelLevelTankLimit.Size = new Size(144, 15);
            LabelLevelTankLimit.TabIndex = 4;
            LabelLevelTankLimit.Text = "Level Tank Position: Limit";
            // 
            // ButtonBuildPlatform
            // 
            ButtonBuildPlatform.ForeColor = Color.Blue;
            ButtonBuildPlatform.Location = new Point(318, 332);
            ButtonBuildPlatform.Name = "ButtonBuildPlatform";
            ButtonBuildPlatform.Size = new Size(416, 42);
            ButtonBuildPlatform.TabIndex = 5;
            ButtonBuildPlatform.Text = "Build Platform";
            ButtonBuildPlatform.UseVisualStyleBackColor = true;
            // 
            // LabelBuildPlatformPositionTop
            // 
            LabelBuildPlatformPositionTop.AutoSize = true;
            LabelBuildPlatformPositionTop.Location = new Point(844, 293);
            LabelBuildPlatformPositionTop.Name = "LabelBuildPlatformPositionTop";
            LabelBuildPlatformPositionTop.Size = new Size(158, 15);
            LabelBuildPlatformPositionTop.TabIndex = 7;
            LabelBuildPlatformPositionTop.Text = "Build Platform Position: Top";
            // 
            // TextBoxBuildPlatformPositionTop
            // 
            TextBoxBuildPlatformPositionTop.Location = new Point(844, 267);
            TextBoxBuildPlatformPositionTop.Name = "TextBoxBuildPlatformPositionTop";
            TextBoxBuildPlatformPositionTop.Size = new Size(100, 23);
            TextBoxBuildPlatformPositionTop.TabIndex = 6;
            TextBoxBuildPlatformPositionTop.Text = "22";
            // 
            // LabelBuildPlatformPositionOrigin
            // 
            LabelBuildPlatformPositionOrigin.AutoSize = true;
            LabelBuildPlatformPositionOrigin.Location = new Point(844, 358);
            LabelBuildPlatformPositionOrigin.Name = "LabelBuildPlatformPositionOrigin";
            LabelBuildPlatformPositionOrigin.Size = new Size(171, 15);
            LabelBuildPlatformPositionOrigin.TabIndex = 9;
            LabelBuildPlatformPositionOrigin.Text = "Build Platform Position: Origin";
            // 
            // TextBoxBuildPlatformPositionOrigin
            // 
            TextBoxBuildPlatformPositionOrigin.Location = new Point(844, 332);
            TextBoxBuildPlatformPositionOrigin.Name = "TextBoxBuildPlatformPositionOrigin";
            TextBoxBuildPlatformPositionOrigin.Size = new Size(100, 23);
            TextBoxBuildPlatformPositionOrigin.TabIndex = 8;
            TextBoxBuildPlatformPositionOrigin.Text = "121.1";
            // 
            // LabelBuildPlatformPositionLimitA
            // 
            LabelBuildPlatformPositionLimitA.AutoSize = true;
            LabelBuildPlatformPositionLimitA.Location = new Point(844, 465);
            LabelBuildPlatformPositionLimitA.Name = "LabelBuildPlatformPositionLimitA";
            LabelBuildPlatformPositionLimitA.Size = new Size(173, 15);
            LabelBuildPlatformPositionLimitA.TabIndex = 11;
            LabelBuildPlatformPositionLimitA.Text = "Build Platform Position: LimitA";
            // 
            // TextBoxBuildPlatformPositionLimitA
            // 
            TextBoxBuildPlatformPositionLimitA.Location = new Point(844, 439);
            TextBoxBuildPlatformPositionLimitA.Name = "TextBoxBuildPlatformPositionLimitA";
            TextBoxBuildPlatformPositionLimitA.Size = new Size(100, 23);
            TextBoxBuildPlatformPositionLimitA.TabIndex = 10;
            TextBoxBuildPlatformPositionLimitA.Text = "361";
            // 
            // LabelBuildPlatformPositionLimitB
            // 
            LabelBuildPlatformPositionLimitB.AutoSize = true;
            LabelBuildPlatformPositionLimitB.Location = new Point(844, 526);
            LabelBuildPlatformPositionLimitB.Name = "LabelBuildPlatformPositionLimitB";
            LabelBuildPlatformPositionLimitB.Size = new Size(172, 15);
            LabelBuildPlatformPositionLimitB.TabIndex = 13;
            LabelBuildPlatformPositionLimitB.Text = "Build Platform Position: LimitB";
            // 
            // TextBoxBuildPlatformPositionLimitB
            // 
            TextBoxBuildPlatformPositionLimitB.Location = new Point(844, 500);
            TextBoxBuildPlatformPositionLimitB.Name = "TextBoxBuildPlatformPositionLimitB";
            TextBoxBuildPlatformPositionLimitB.Size = new Size(100, 23);
            TextBoxBuildPlatformPositionLimitB.TabIndex = 12;
            TextBoxBuildPlatformPositionLimitB.Text = "381";
            // 
            // ButtonPrintBlade
            // 
            ButtonPrintBlade.ForeColor = Color.Blue;
            ButtonPrintBlade.Location = new Point(750, 274);
            ButtonPrintBlade.Name = "ButtonPrintBlade";
            ButtonPrintBlade.Size = new Size(59, 52);
            ButtonPrintBlade.TabIndex = 14;
            ButtonPrintBlade.Text = "Print Blade";
            ButtonPrintBlade.UseVisualStyleBackColor = true;
            // 
            // ButtonCollectBlade
            // 
            ButtonCollectBlade.ForeColor = Color.Blue;
            ButtonCollectBlade.Location = new Point(750, 209);
            ButtonCollectBlade.Name = "ButtonCollectBlade";
            ButtonCollectBlade.Size = new Size(59, 52);
            ButtonCollectBlade.TabIndex = 15;
            ButtonCollectBlade.Text = "Collect Blade";
            ButtonCollectBlade.UseVisualStyleBackColor = true;
            // 
            // LabelCollectBladeZero
            // 
            LabelCollectBladeZero.AutoSize = true;
            LabelCollectBladeZero.Location = new Point(318, 102);
            LabelCollectBladeZero.Name = "LabelCollectBladeZero";
            LabelCollectBladeZero.Size = new Size(155, 15);
            LabelCollectBladeZero.TabIndex = 17;
            LabelCollectBladeZero.Text = "Collect Blade Position: Zero";
            // 
            // TextBoxCollectBladeZero
            // 
            TextBoxCollectBladeZero.Location = new Point(318, 76);
            TextBoxCollectBladeZero.Name = "TextBoxCollectBladeZero";
            TextBoxCollectBladeZero.Size = new Size(100, 23);
            TextBoxCollectBladeZero.TabIndex = 16;
            TextBoxCollectBladeZero.Text = "0";
            // 
            // LabelCollectBladeLimit
            // 
            LabelCollectBladeLimit.AutoSize = true;
            LabelCollectBladeLimit.Location = new Point(750, 102);
            LabelCollectBladeLimit.Name = "LabelCollectBladeLimit";
            LabelCollectBladeLimit.Size = new Size(158, 15);
            LabelCollectBladeLimit.TabIndex = 19;
            LabelCollectBladeLimit.Text = "Collect Blade Position: Limit";
            // 
            // TextBoxCollectBladeLimit
            // 
            TextBoxCollectBladeLimit.Location = new Point(750, 76);
            TextBoxCollectBladeLimit.Name = "TextBoxCollectBladeLimit";
            TextBoxCollectBladeLimit.Size = new Size(100, 23);
            TextBoxCollectBladeLimit.TabIndex = 18;
            TextBoxCollectBladeLimit.Text = "60000";
            // 
            // LabelPrintBladeLimit
            // 
            LabelPrintBladeLimit.AutoSize = true;
            LabelPrintBladeLimit.Location = new Point(750, 183);
            LabelPrintBladeLimit.Name = "LabelPrintBladeLimit";
            LabelPrintBladeLimit.Size = new Size(146, 15);
            LabelPrintBladeLimit.TabIndex = 23;
            LabelPrintBladeLimit.Text = "Print Blade Position: Limit";
            // 
            // TextBoxPrintBladeLimit
            // 
            TextBoxPrintBladeLimit.Location = new Point(750, 157);
            TextBoxPrintBladeLimit.Name = "TextBoxPrintBladeLimit";
            TextBoxPrintBladeLimit.Size = new Size(100, 23);
            TextBoxPrintBladeLimit.TabIndex = 22;
            TextBoxPrintBladeLimit.Text = "60000";
            // 
            // LabelPrintBladeZero
            // 
            LabelPrintBladeZero.AutoSize = true;
            LabelPrintBladeZero.Location = new Point(318, 183);
            LabelPrintBladeZero.Name = "LabelPrintBladeZero";
            LabelPrintBladeZero.Size = new Size(143, 15);
            LabelPrintBladeZero.TabIndex = 21;
            LabelPrintBladeZero.Text = "Print Blade Position: Zero";
            // 
            // TextBoxPrintBladeZero
            // 
            TextBoxPrintBladeZero.Location = new Point(318, 157);
            TextBoxPrintBladeZero.Name = "TextBoxPrintBladeZero";
            TextBoxPrintBladeZero.Size = new Size(100, 23);
            TextBoxPrintBladeZero.TabIndex = 20;
            TextBoxPrintBladeZero.Text = "0";
            // 
            // dataGridView_PortConfiguration
            // 
            dataGridView_PortConfiguration.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView_PortConfiguration.Columns.AddRange(new DataGridViewColumn[] { DeviceName, PortName, BaudRate, Parity, DataBits, StopBits });
            dataGridView_PortConfiguration.Location = new Point(12, 673);
            dataGridView_PortConfiguration.Name = "dataGridView_PortConfiguration";
            dataGridView_PortConfiguration.Size = new Size(751, 243);
            dataGridView_PortConfiguration.TabIndex = 24;
            // 
            // DeviceName
            // 
            DeviceName.HeaderText = "DeviceName";
            DeviceName.Name = "DeviceName";
            DeviceName.Width = 200;
            // 
            // PortName
            // 
            PortName.HeaderText = "PortName";
            PortName.Name = "PortName";
            // 
            // BaudRate
            // 
            BaudRate.HeaderText = "BaudRate";
            BaudRate.Name = "BaudRate";
            // 
            // Parity
            // 
            Parity.HeaderText = "Parity";
            Parity.Name = "Parity";
            // 
            // DataBits
            // 
            DataBits.HeaderText = "DataBits";
            DataBits.Name = "DataBits";
            // 
            // StopBits
            // 
            StopBits.HeaderText = "StopBits";
            StopBits.Name = "StopBits";
            // 
            // logText
            // 
            logText.Location = new Point(772, 673);
            logText.Name = "logText";
            logText.Size = new Size(292, 212);
            logText.TabIndex = 25;
            logText.Text = "";
            // 
            // buttonClear
            // 
            buttonClear.Location = new Point(974, 891);
            buttonClear.Name = "buttonClear";
            buttonClear.Size = new Size(90, 25);
            buttonClear.TabIndex = 26;
            buttonClear.Text = "Clear";
            buttonClear.UseVisualStyleBackColor = true;
            buttonClear.Click += buttonClear_Click;
            // 
            // buttonPortClose
            // 
            buttonPortClose.Location = new Point(866, 891);
            buttonPortClose.Name = "buttonPortClose";
            buttonPortClose.Size = new Size(90, 25);
            buttonPortClose.TabIndex = 28;
            buttonPortClose.Text = "Port Close";
            buttonPortClose.UseVisualStyleBackColor = true;
            buttonPortClose.Click += buttonPortClose_Click;
            // 
            // buttonPortOpen
            // 
            buttonPortOpen.Location = new Point(772, 891);
            buttonPortOpen.Name = "buttonPortOpen";
            buttonPortOpen.Size = new Size(90, 25);
            buttonPortOpen.TabIndex = 27;
            buttonPortOpen.Text = "Port Open";
            buttonPortOpen.UseVisualStyleBackColor = true;
            buttonPortOpen.Click += buttonPortOpen_Click;
            // 
            // PanelStage
            // 
            PanelStage.BorderStyle = BorderStyle.FixedSingle;
            PanelStage.Controls.Add(TextBoxLevelTankZero);
            PanelStage.Controls.Add(ButtonLevelTank);
            PanelStage.Controls.Add(TextBoxLevelTankLimit);
            PanelStage.Controls.Add(LabelLevelTankZero);
            PanelStage.Controls.Add(LabelLevelTankLimit);
            PanelStage.Controls.Add(ButtonBuildPlatform);
            PanelStage.Controls.Add(LabelPrintBladeLimit);
            PanelStage.Controls.Add(TextBoxBuildPlatformPositionTop);
            PanelStage.Controls.Add(TextBoxPrintBladeLimit);
            PanelStage.Controls.Add(LabelBuildPlatformPositionTop);
            PanelStage.Controls.Add(LabelPrintBladeZero);
            PanelStage.Controls.Add(TextBoxBuildPlatformPositionOrigin);
            PanelStage.Controls.Add(TextBoxPrintBladeZero);
            PanelStage.Controls.Add(LabelBuildPlatformPositionOrigin);
            PanelStage.Controls.Add(LabelCollectBladeLimit);
            PanelStage.Controls.Add(TextBoxBuildPlatformPositionLimitA);
            PanelStage.Controls.Add(TextBoxCollectBladeLimit);
            PanelStage.Controls.Add(LabelBuildPlatformPositionLimitA);
            PanelStage.Controls.Add(LabelCollectBladeZero);
            PanelStage.Controls.Add(TextBoxBuildPlatformPositionLimitB);
            PanelStage.Controls.Add(TextBoxCollectBladeZero);
            PanelStage.Controls.Add(LabelBuildPlatformPositionLimitB);
            PanelStage.Controls.Add(ButtonCollectBlade);
            PanelStage.Controls.Add(ButtonPrintBlade);
            PanelStage.Location = new Point(12, 12);
            PanelStage.Name = "PanelStage";
            PanelStage.Size = new Size(1052, 563);
            PanelStage.TabIndex = 29;
            // 
            // PanelWaterLevel
            // 
            PanelWaterLevel.BorderStyle = BorderStyle.FixedSingle;
            PanelWaterLevel.Controls.Add(TextBoxCurrentWaterLevelMax);
            PanelWaterLevel.Controls.Add(TextBoxCurrentWaterLevelMin);
            PanelWaterLevel.Controls.Add(LabelCurrentWaterLevelRange);
            PanelWaterLevel.Controls.Add(TextBoxCurrentWaterLevel);
            PanelWaterLevel.Controls.Add(LabelCurrentWaterLevel);
            PanelWaterLevel.Location = new Point(12, 590);
            PanelWaterLevel.Name = "PanelWaterLevel";
            PanelWaterLevel.Size = new Size(497, 65);
            PanelWaterLevel.TabIndex = 30;
            // 
            // TextBoxCurrentWaterLevel
            // 
            TextBoxCurrentWaterLevel.Enabled = false;
            TextBoxCurrentWaterLevel.Location = new Point(143, 16);
            TextBoxCurrentWaterLevel.Name = "TextBoxCurrentWaterLevel";
            TextBoxCurrentWaterLevel.Size = new Size(100, 23);
            TextBoxCurrentWaterLevel.TabIndex = 5;
            TextBoxCurrentWaterLevel.Text = "-3.0000";
            // 
            // LabelCurrentWaterLevel
            // 
            LabelCurrentWaterLevel.AutoSize = true;
            LabelCurrentWaterLevel.Location = new Point(12, 19);
            LabelCurrentWaterLevel.Name = "LabelCurrentWaterLevel";
            LabelCurrentWaterLevel.Size = new Size(113, 15);
            LabelCurrentWaterLevel.TabIndex = 6;
            LabelCurrentWaterLevel.Text = "Current Water Level";
            // 
            // TextBoxCurrentWaterLevelMin
            // 
            TextBoxCurrentWaterLevelMin.Location = new Point(294, 16);
            TextBoxCurrentWaterLevelMin.Name = "TextBoxCurrentWaterLevelMin";
            TextBoxCurrentWaterLevelMin.Size = new Size(64, 23);
            TextBoxCurrentWaterLevelMin.TabIndex = 7;
            TextBoxCurrentWaterLevelMin.Text = "-3.1";
            // 
            // TextBoxCurrentWaterLevelMax
            // 
            TextBoxCurrentWaterLevelMax.Location = new Point(386, 16);
            TextBoxCurrentWaterLevelMax.Name = "TextBoxCurrentWaterLevelMax";
            TextBoxCurrentWaterLevelMax.Size = new Size(64, 23);
            TextBoxCurrentWaterLevelMax.TabIndex = 8;
            TextBoxCurrentWaterLevelMax.Text = "-2.9";
            // 
            // LabelCurrentWaterLevelRange
            // 
            LabelCurrentWaterLevelRange.AutoSize = true;
            LabelCurrentWaterLevelRange.Location = new Point(281, 19);
            LabelCurrentWaterLevelRange.Name = "LabelCurrentWaterLevelRange";
            LabelCurrentWaterLevelRange.Size = new Size(183, 15);
            LabelCurrentWaterLevelRange.TabIndex = 9;
            LabelCurrentWaterLevelRange.Text = "(                    ~                    )";
            // 
            // DM400
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1076, 928);
            Controls.Add(PanelWaterLevel);
            Controls.Add(PanelStage);
            Controls.Add(buttonPortClose);
            Controls.Add(buttonPortOpen);
            Controls.Add(buttonClear);
            Controls.Add(logText);
            Controls.Add(dataGridView_PortConfiguration);
            Name = "DM400";
            Text = "DM400";
            FormClosing += DM400_FormClosing;
            Load += DM400_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView_PortConfiguration).EndInit();
            PanelStage.ResumeLayout(false);
            PanelStage.PerformLayout();
            PanelWaterLevel.ResumeLayout(false);
            PanelWaterLevel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Label LabelLevelTankZero;
        private Label LabelLevelTankLimit;
        private Label LabelBuildPlatformPositionTop;
        private Label LabelBuildPlatformPositionOrigin;
        private Label LabelBuildPlatformPositionLimitA;
        private Label LabelBuildPlatformPositionLimitB;
        private Label LabelCollectBladeZero;
        private Label LabelCollectBladeLimit;
        private Label LabelPrintBladeLimit;
        private Label LabelPrintBladeZero;
        private DataGridView dataGridView_PortConfiguration;
        private RichTextBox logText;
        private Button buttonClear;
        private Button buttonPortClose;
        private Button buttonPortOpen;
        private DataGridViewTextBoxColumn DeviceName;
        private DataGridViewTextBoxColumn PortName;
        private DataGridViewTextBoxColumn BaudRate;
        private DataGridViewTextBoxColumn Parity;
        private DataGridViewTextBoxColumn DataBits;
        private DataGridViewTextBoxColumn StopBits;
        public Button ButtonLevelTank;
        public TextBox TextBoxLevelTankZero;
        public TextBox TextBoxLevelTankLimit;
        public Button ButtonBuildPlatform;
        public TextBox TextBoxBuildPlatformPositionTop;
        public TextBox TextBoxBuildPlatformPositionOrigin;
        public TextBox TextBoxBuildPlatformPositionLimitA;
        public TextBox TextBoxBuildPlatformPositionLimitB;
        public Button ButtonPrintBlade;
        public Button ButtonCollectBlade;
        public TextBox TextBoxCollectBladeZero;
        public TextBox TextBoxCollectBladeLimit;
        public TextBox TextBoxPrintBladeLimit;
        public TextBox TextBoxPrintBladeZero;
        private Panel PanelStage;
        private Panel PanelWaterLevel;
        public TextBox TextBoxCurrentWaterLevel;
        private Label LabelCurrentWaterLevel;
        public TextBox TextBoxCurrentWaterLevelMax;
        public TextBox TextBoxCurrentWaterLevelMin;
        private Label LabelCurrentWaterLevelRange;
    }
}
