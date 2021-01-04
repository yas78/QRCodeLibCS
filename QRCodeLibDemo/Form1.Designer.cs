namespace Demo
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.txtData = new System.Windows.Forms.TextBox();
            this.qrcodePanel = new System.Windows.Forms.FlowLayoutPanel();
            this.lblData = new System.Windows.Forms.Label();
            this.chkStructuredAppend = new System.Windows.Forms.CheckBox();
            this.cmbErrorCorrectionLevel = new System.Windows.Forms.ComboBox();
            this.cmbMaxVersion = new System.Windows.Forms.ComboBox();
            this.lblErrorCorrectionLevel = new System.Windows.Forms.Label();
            this.lblMaxVersion = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblModuleSize = new System.Windows.Forms.Label();
            this.nudModuleSize = new System.Windows.Forms.NumericUpDown();
            this.lblEncoding = new System.Windows.Forms.Label();
            this.cmbEncoding = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudModuleSize)).BeginInit();
            this.SuspendLayout();
            // 
            // txtData
            // 
            this.txtData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtData.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtData.Location = new System.Drawing.Point(14, 447);
            this.txtData.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtData.Multiline = true;
            this.txtData.Name = "txtData";
            this.txtData.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtData.Size = new System.Drawing.Size(656, 130);
            this.txtData.TabIndex = 0;
            this.txtData.WordWrap = false;
            this.txtData.TextChanged += new System.EventHandler(this.UpdateQRCodePanel);
            // 
            // qrcodePanel
            // 
            this.qrcodePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.qrcodePanel.AutoScroll = true;
            this.qrcodePanel.Location = new System.Drawing.Point(14, 17);
            this.qrcodePanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.qrcodePanel.Name = "qrcodePanel";
            this.qrcodePanel.Size = new System.Drawing.Size(656, 405);
            this.qrcodePanel.TabIndex = 11;
            // 
            // lblData
            // 
            this.lblData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblData.AutoSize = true;
            this.lblData.Location = new System.Drawing.Point(10, 424);
            this.lblData.Name = "lblData";
            this.lblData.Size = new System.Drawing.Size(42, 17);
            this.lblData.TabIndex = 12;
            this.lblData.Text = "&Data :";
            // 
            // chkStructuredAppend
            // 
            this.chkStructuredAppend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkStructuredAppend.AutoSize = true;
            this.chkStructuredAppend.Location = new System.Drawing.Point(237, 626);
            this.chkStructuredAppend.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkStructuredAppend.Name = "chkStructuredAppend";
            this.chkStructuredAppend.Size = new System.Drawing.Size(137, 21);
            this.chkStructuredAppend.TabIndex = 7;
            this.chkStructuredAppend.Text = "Structured &Append";
            this.chkStructuredAppend.UseVisualStyleBackColor = true;
            this.chkStructuredAppend.CheckedChanged += new System.EventHandler(this.UpdateQRCodePanel);
            // 
            // cmbErrorCorrectionLevel
            // 
            this.cmbErrorCorrectionLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmbErrorCorrectionLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbErrorCorrectionLevel.FormattingEnabled = true;
            this.cmbErrorCorrectionLevel.Location = new System.Drawing.Point(159, 589);
            this.cmbErrorCorrectionLevel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbErrorCorrectionLevel.Name = "cmbErrorCorrectionLevel";
            this.cmbErrorCorrectionLevel.Size = new System.Drawing.Size(55, 25);
            this.cmbErrorCorrectionLevel.TabIndex = 2;
            this.cmbErrorCorrectionLevel.SelectedIndexChanged += new System.EventHandler(this.UpdateQRCodePanel);
            // 
            // cmbMaxVersion
            // 
            this.cmbMaxVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmbMaxVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMaxVersion.FormattingEnabled = true;
            this.cmbMaxVersion.Location = new System.Drawing.Point(159, 623);
            this.cmbMaxVersion.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbMaxVersion.Name = "cmbMaxVersion";
            this.cmbMaxVersion.Size = new System.Drawing.Size(55, 25);
            this.cmbMaxVersion.TabIndex = 6;
            this.cmbMaxVersion.SelectedIndexChanged += new System.EventHandler(this.UpdateQRCodePanel);
            // 
            // lblErrorCorrectionLevel
            // 
            this.lblErrorCorrectionLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblErrorCorrectionLevel.AutoSize = true;
            this.lblErrorCorrectionLevel.Location = new System.Drawing.Point(10, 593);
            this.lblErrorCorrectionLevel.Name = "lblErrorCorrectionLevel";
            this.lblErrorCorrectionLevel.Size = new System.Drawing.Size(143, 17);
            this.lblErrorCorrectionLevel.TabIndex = 1;
            this.lblErrorCorrectionLevel.Text = "Error Correction &Level :";
            // 
            // lblMaxVersion
            // 
            this.lblMaxVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblMaxVersion.AutoSize = true;
            this.lblMaxVersion.Location = new System.Drawing.Point(10, 627);
            this.lblMaxVersion.Name = "lblMaxVersion";
            this.lblMaxVersion.Size = new System.Drawing.Size(87, 17);
            this.lblMaxVersion.TabIndex = 5;
            this.lblMaxVersion.Text = "Max &Version :";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSave.Location = new System.Drawing.Point(552, 623);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(118, 25);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblModuleSize
            // 
            this.lblModuleSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblModuleSize.AutoSize = true;
            this.lblModuleSize.Location = new System.Drawing.Point(391, 627);
            this.lblModuleSize.Name = "lblModuleSize";
            this.lblModuleSize.Size = new System.Drawing.Size(87, 17);
            this.lblModuleSize.TabIndex = 8;
            this.lblModuleSize.Text = "&Module Size :";
            // 
            // nudModuleSize
            // 
            this.nudModuleSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nudModuleSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudModuleSize.Location = new System.Drawing.Point(486, 623);
            this.nudModuleSize.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.nudModuleSize.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudModuleSize.Name = "nudModuleSize";
            this.nudModuleSize.Size = new System.Drawing.Size(40, 25);
            this.nudModuleSize.TabIndex = 9;
            this.nudModuleSize.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudModuleSize.ValueChanged += new System.EventHandler(this.UpdateQRCodePanel);
            // 
            // lblEncoding
            // 
            this.lblEncoding.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblEncoding.AutoSize = true;
            this.lblEncoding.Location = new System.Drawing.Point(237, 593);
            this.lblEncoding.Name = "lblEncoding";
            this.lblEncoding.Size = new System.Drawing.Size(135, 17);
            this.lblEncoding.TabIndex = 3;
            this.lblEncoding.Text = "Byte mode &Encoding :";
            // 
            // cmbEncoding
            // 
            this.cmbEncoding.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmbEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEncoding.FormattingEnabled = true;
            this.cmbEncoding.Location = new System.Drawing.Point(378, 589);
            this.cmbEncoding.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbEncoding.Name = "cmbEncoding";
            this.cmbEncoding.Size = new System.Drawing.Size(292, 25);
            this.cmbEncoding.TabIndex = 4;
            this.cmbEncoding.SelectedIndexChanged += new System.EventHandler(this.UpdateQRCodePanel);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 661);
            this.Controls.Add(this.cmbEncoding);
            this.Controls.Add(this.lblEncoding);
            this.Controls.Add(this.nudModuleSize);
            this.Controls.Add(this.lblModuleSize);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblMaxVersion);
            this.Controls.Add(this.lblErrorCorrectionLevel);
            this.Controls.Add(this.cmbMaxVersion);
            this.Controls.Add(this.cmbErrorCorrectionLevel);
            this.Controls.Add(this.chkStructuredAppend);
            this.Controls.Add(this.lblData);
            this.Controls.Add(this.qrcodePanel);
            this.Controls.Add(this.txtData);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(627, 692);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "QR Code";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudModuleSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtData;
        private System.Windows.Forms.FlowLayoutPanel qrcodePanel;
        private System.Windows.Forms.Label lblData;
        private System.Windows.Forms.CheckBox chkStructuredAppend;
        private System.Windows.Forms.ComboBox cmbErrorCorrectionLevel;
        private System.Windows.Forms.ComboBox cmbMaxVersion;
        private System.Windows.Forms.Label lblErrorCorrectionLevel;
        private System.Windows.Forms.Label lblMaxVersion;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblModuleSize;
        private System.Windows.Forms.NumericUpDown nudModuleSize;
        private System.Windows.Forms.Label lblEncoding;
        private System.Windows.Forms.ComboBox cmbEncoding;
    }
}

