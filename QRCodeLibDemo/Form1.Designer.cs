namespace Example
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
            this.lblSize = new System.Windows.Forms.Label();
            this.nudSize = new System.Windows.Forms.NumericUpDown();
            this.lblEncoding = new System.Windows.Forms.Label();
            this.cmbEncoding = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudSize)).BeginInit();
            this.SuspendLayout();
            // 
            // txtData
            // 
            this.txtData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtData.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtData.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtData.Location = new System.Drawing.Point(12, 355);
            this.txtData.Multiline = true;
            this.txtData.Name = "txtData";
            this.txtData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtData.Size = new System.Drawing.Size(660, 85);
            this.txtData.TabIndex = 0;
            this.txtData.TextChanged += new System.EventHandler(this.Update_qrcodePanel);
            // 
            // qrcodePanel
            // 
            this.qrcodePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.qrcodePanel.AutoScroll = true;
            this.qrcodePanel.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.qrcodePanel.Location = new System.Drawing.Point(12, 12);
            this.qrcodePanel.Name = "qrcodePanel";
            this.qrcodePanel.Size = new System.Drawing.Size(660, 319);
            this.qrcodePanel.TabIndex = 6;
            // 
            // lblData
            // 
            this.lblData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblData.AutoSize = true;
            this.lblData.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblData.Location = new System.Drawing.Point(9, 339);
            this.lblData.Name = "lblData";
            this.lblData.Size = new System.Drawing.Size(39, 13);
            this.lblData.TabIndex = 7;
            this.lblData.Text = "Data :";
            // 
            // chkStructuredAppend
            // 
            this.chkStructuredAppend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkStructuredAppend.AutoSize = true;
            this.chkStructuredAppend.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.chkStructuredAppend.Location = new System.Drawing.Point(237, 480);
            this.chkStructuredAppend.Name = "chkStructuredAppend";
            this.chkStructuredAppend.Size = new System.Drawing.Size(132, 17);
            this.chkStructuredAppend.TabIndex = 4;
            this.chkStructuredAppend.Text = "Structured Append";
            this.chkStructuredAppend.UseVisualStyleBackColor = true;
            this.chkStructuredAppend.CheckedChanged += new System.EventHandler(this.Update_qrcodePanel);
            // 
            // cmbErrorCorrectionLevel
            // 
            this.cmbErrorCorrectionLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmbErrorCorrectionLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbErrorCorrectionLevel.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cmbErrorCorrectionLevel.FormattingEnabled = true;
            this.cmbErrorCorrectionLevel.Location = new System.Drawing.Point(164, 449);
            this.cmbErrorCorrectionLevel.Name = "cmbErrorCorrectionLevel";
            this.cmbErrorCorrectionLevel.Size = new System.Drawing.Size(48, 21);
            this.cmbErrorCorrectionLevel.TabIndex = 1;
            this.cmbErrorCorrectionLevel.SelectedIndexChanged += new System.EventHandler(this.Update_qrcodePanel);
            // 
            // cmbMaxVersion
            // 
            this.cmbMaxVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmbMaxVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMaxVersion.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cmbMaxVersion.FormattingEnabled = true;
            this.cmbMaxVersion.Location = new System.Drawing.Point(164, 478);
            this.cmbMaxVersion.Name = "cmbMaxVersion";
            this.cmbMaxVersion.Size = new System.Drawing.Size(48, 21);
            this.cmbMaxVersion.TabIndex = 2;
            this.cmbMaxVersion.SelectedIndexChanged += new System.EventHandler(this.Update_qrcodePanel);
            // 
            // lblErrorCorrectionLevel
            // 
            this.lblErrorCorrectionLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblErrorCorrectionLevel.AutoSize = true;
            this.lblErrorCorrectionLevel.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblErrorCorrectionLevel.Location = new System.Drawing.Point(9, 453);
            this.lblErrorCorrectionLevel.Name = "lblErrorCorrectionLevel";
            this.lblErrorCorrectionLevel.Size = new System.Drawing.Size(143, 13);
            this.lblErrorCorrectionLevel.TabIndex = 11;
            this.lblErrorCorrectionLevel.Text = "Error Correction Level :";
            // 
            // lblMaxVersion
            // 
            this.lblMaxVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblMaxVersion.AutoSize = true;
            this.lblMaxVersion.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblMaxVersion.Location = new System.Drawing.Point(9, 482);
            this.lblMaxVersion.Name = "lblMaxVersion";
            this.lblMaxVersion.Size = new System.Drawing.Size(83, 13);
            this.lblMaxVersion.TabIndex = 12;
            this.lblMaxVersion.Text = "Max Version :";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSave.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnSave.Location = new System.Drawing.Point(553, 477);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(103, 23);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblSize
            // 
            this.lblSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSize.AutoSize = true;
            this.lblSize.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblSize.Location = new System.Drawing.Point(384, 482);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(82, 13);
            this.lblSize.TabIndex = 15;
            this.lblSize.Text = "Module Size :";
            // 
            // nudSize
            // 
            this.nudSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nudSize.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.nudSize.Location = new System.Drawing.Point(472, 478);
            this.nudSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSize.Name = "nudSize";
            this.nudSize.Size = new System.Drawing.Size(46, 20);
            this.nudSize.TabIndex = 3;
            this.nudSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSize.ValueChanged += new System.EventHandler(this.Update_qrcodePanel);
            // 
            // lblEncoding
            // 
            this.lblEncoding.AutoSize = true;
            this.lblEncoding.Location = new System.Drawing.Point(235, 453);
            this.lblEncoding.Name = "lblEncoding";
            this.lblEncoding.Size = new System.Drawing.Size(57, 12);
            this.lblEncoding.TabIndex = 16;
            this.lblEncoding.Text = "Encoding :";
            // 
            // cmbEncoding
            // 
            this.cmbEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEncoding.FormattingEnabled = true;
            this.cmbEncoding.Location = new System.Drawing.Point(298, 449);
            this.cmbEncoding.Name = "cmbEncoding";
            this.cmbEncoding.Size = new System.Drawing.Size(358, 20);
            this.cmbEncoding.TabIndex = 17;
            this.cmbEncoding.SelectedIndexChanged += new System.EventHandler(this.Update_qrcodePanel);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 511);
            this.Controls.Add(this.cmbEncoding);
            this.Controls.Add(this.lblEncoding);
            this.Controls.Add(this.nudSize);
            this.Controls.Add(this.lblSize);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblMaxVersion);
            this.Controls.Add(this.lblErrorCorrectionLevel);
            this.Controls.Add(this.cmbMaxVersion);
            this.Controls.Add(this.cmbErrorCorrectionLevel);
            this.Controls.Add(this.chkStructuredAppend);
            this.Controls.Add(this.lblData);
            this.Controls.Add(this.qrcodePanel);
            this.Controls.Add(this.txtData);
            this.MinimumSize = new System.Drawing.Size(540, 500);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "QRCode";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudSize)).EndInit();
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
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.NumericUpDown nudSize;
        private System.Windows.Forms.Label lblEncoding;
        private System.Windows.Forms.ComboBox cmbEncoding;
    }
}

