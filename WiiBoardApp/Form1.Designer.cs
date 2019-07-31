

namespace WiiBoardApp
{
    partial class WiiBoardAppForm
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
            this.acquisitionTypeLabel = new System.Windows.Forms.Label();
            this.acquisitionLengthLabel = new System.Windows.Forms.Label();
            this.acquisitionLength = new System.Windows.Forms.TextBox();
            this.jsonOutputButton = new System.Windows.Forms.RadioButton();
            this.csvOutputButton = new System.Windows.Forms.RadioButton();
            this.txtOutputButton = new System.Windows.Forms.RadioButton();
            this.acquisitionTypeComboBox = new System.Windows.Forms.ComboBox();
            this.connectButton = new System.Windows.Forms.Button();
            this.startAcquisitionButton = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.stateLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // acquisitionTypeLabel
            // 
            this.acquisitionTypeLabel.AutoSize = true;
            this.acquisitionTypeLabel.Location = new System.Drawing.Point(26, 22);
            this.acquisitionTypeLabel.Name = "acquisitionTypeLabel";
            this.acquisitionTypeLabel.Size = new System.Drawing.Size(81, 13);
            this.acquisitionTypeLabel.TabIndex = 0;
            this.acquisitionTypeLabel.Text = "Acquisition type";
            this.acquisitionTypeLabel.Click += new System.EventHandler(this.acquisitionTypeLabel_Click);
            // 
            // acquisitionLengthLabel
            // 
            this.acquisitionLengthLabel.AutoSize = true;
            this.acquisitionLengthLabel.Location = new System.Drawing.Point(29, 57);
            this.acquisitionLengthLabel.Name = "acquisitionLengthLabel";
            this.acquisitionLengthLabel.Size = new System.Drawing.Size(90, 13);
            this.acquisitionLengthLabel.TabIndex = 1;
            this.acquisitionLengthLabel.Text = "Acquisition length";
            // 
            // acquisitionLength
            // 
            this.acquisitionLength.Location = new System.Drawing.Point(143, 54);
            this.acquisitionLength.Name = "acquisitionLength";
            this.acquisitionLength.Size = new System.Drawing.Size(100, 20);
            this.acquisitionLength.TabIndex = 2;
            // 
            // jsonOutputButton
            // 
            this.jsonOutputButton.AutoSize = true;
            this.jsonOutputButton.Location = new System.Drawing.Point(54, 105);
            this.jsonOutputButton.Name = "jsonOutputButton";
            this.jsonOutputButton.Size = new System.Drawing.Size(53, 17);
            this.jsonOutputButton.TabIndex = 3;
            this.jsonOutputButton.TabStop = true;
            this.jsonOutputButton.Text = "JSON";
            this.jsonOutputButton.UseVisualStyleBackColor = true;
            this.jsonOutputButton.CheckedChanged += new System.EventHandler(this.jsonOutputButton_CheckedChanged);
            // 
            // csvOutputButton
            // 
            this.csvOutputButton.AutoSize = true;
            this.csvOutputButton.Location = new System.Drawing.Point(200, 105);
            this.csvOutputButton.Name = "csvOutputButton";
            this.csvOutputButton.Size = new System.Drawing.Size(43, 17);
            this.csvOutputButton.TabIndex = 4;
            this.csvOutputButton.TabStop = true;
            this.csvOutputButton.Text = "Csv";
            this.csvOutputButton.UseVisualStyleBackColor = true;
            this.csvOutputButton.CheckedChanged += new System.EventHandler(this.csvOutputButton_CheckedChanged);
            // 
            // txtOutputButton
            // 
            this.txtOutputButton.AutoSize = true;
            this.txtOutputButton.Location = new System.Drawing.Point(351, 105);
            this.txtOutputButton.Name = "txtOutputButton";
            this.txtOutputButton.Size = new System.Drawing.Size(36, 17);
            this.txtOutputButton.TabIndex = 5;
            this.txtOutputButton.TabStop = true;
            this.txtOutputButton.Text = "txt";
            this.txtOutputButton.UseVisualStyleBackColor = true;
            this.txtOutputButton.CheckedChanged += new System.EventHandler(this.txtOutputButton_CheckedChanged);
            // 
            // acquisitionTypeComboBox
            // 
            this.acquisitionTypeComboBox.FormattingEnabled = true;
            this.acquisitionTypeComboBox.Items.AddRange(new object[] {
            "Timed acquisition",
            "Manual acquisition"});
            this.acquisitionTypeComboBox.Location = new System.Drawing.Point(143, 19);
            this.acquisitionTypeComboBox.Name = "acquisitionTypeComboBox";
            this.acquisitionTypeComboBox.Size = new System.Drawing.Size(121, 21);
            this.acquisitionTypeComboBox.TabIndex = 6;
            this.acquisitionTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.acquisitionTypeComboBox_SelectedIndexChanged);
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(29, 140);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(107, 23);
            this.connectButton.TabIndex = 7;
            this.connectButton.Text = "Connect board";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // startAcquisitionButton
            // 
            this.startAcquisitionButton.Location = new System.Drawing.Point(170, 140);
            this.startAcquisitionButton.Name = "startAcquisitionButton";
            this.startAcquisitionButton.Size = new System.Drawing.Size(111, 23);
            this.startAcquisitionButton.TabIndex = 8;
            this.startAcquisitionButton.Text = "Start acquisition";
            this.startAcquisitionButton.UseVisualStyleBackColor = true;
            this.startAcquisitionButton.Click += new System.EventHandler(this.startAcquisitionButton_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(314, 140);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(110, 23);
            this.button3.TabIndex = 9;
            this.button3.Text = "Stop acquisition";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.stopAcquisitionButton_Click);
            // 
            // stateLabel
            // 
            this.stateLabel.AutoSize = true;
            this.stateLabel.Location = new System.Drawing.Point(167, 197);
            this.stateLabel.Name = "stateLabel";
            this.stateLabel.Size = new System.Drawing.Size(102, 13);
            this.stateLabel.TabIndex = 11;
            this.stateLabel.Text = "Board disconnected";
            // 
            // WiiBoardAppForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 227);
            this.Controls.Add(this.stateLabel);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.startAcquisitionButton);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.acquisitionTypeComboBox);
            this.Controls.Add(this.txtOutputButton);
            this.Controls.Add(this.csvOutputButton);
            this.Controls.Add(this.jsonOutputButton);
            this.Controls.Add(this.acquisitionLength);
            this.Controls.Add(this.acquisitionLengthLabel);
            this.Controls.Add(this.acquisitionTypeLabel);
            this.Name = "WiiBoardAppForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label acquisitionTypeLabel;
        private System.Windows.Forms.Label acquisitionLengthLabel;
        private System.Windows.Forms.TextBox acquisitionLength;
        private System.Windows.Forms.RadioButton jsonOutputButton;
        private System.Windows.Forms.RadioButton csvOutputButton;
        private System.Windows.Forms.RadioButton txtOutputButton;
        private System.Windows.Forms.ComboBox acquisitionTypeComboBox;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Button startAcquisitionButton;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label stateLabel;
    }
}

