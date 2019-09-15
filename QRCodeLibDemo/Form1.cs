﻿using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using Ys.QRCode;

namespace Demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void UpdateQRCodePanel(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
            qrcodePanel.Controls.Clear();

            if (string.IsNullOrEmpty(txtData.Text))
                return;
            
            ErrorCorrectionLevel ecLevel = (ErrorCorrectionLevel)cmbErrorCorrectionLevel.SelectedItem;
            int version = (int)cmbMaxVersion.SelectedItem;
            bool allowStructuredAppend = chkStructuredAppend.Checked;
            Encoding encoding = ((EncodingInfo)cmbEncoding.SelectedItem).GetEncoding();

            Symbols symbols = new Symbols(ecLevel, version, allowStructuredAppend, encoding.WebName);
            
            try
            {
                symbols.AppendText(txtData.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            foreach (Symbol symbol in symbols)
            {
                Image image = symbol.GetImage((int)nudModuleSize.Value);
                PictureBox pictureBox = new PictureBox()
                {
                    Size = image.Size,
                    Image = image
                };
                qrcodePanel.Controls.Add(pictureBox);
            }

            btnSave.Enabled = txtData.TextLength > 0;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string baseName;
            bool isMonochrome;

            using (SaveFileDialog fd = new SaveFileDialog())
            {
                fd.Filter = "Monochrome Bitmap(*.bmp)|*.bmp|24-bit Bitmap(*.bmp)|*.bmp";

                if (fd.ShowDialog() != DialogResult.OK)
                    return;

                isMonochrome = fd.FilterIndex == 1;
                baseName = Path.Combine(
                    Path.GetDirectoryName(fd.FileName), 
                    Path.GetFileNameWithoutExtension(fd.FileName));
            }

            ErrorCorrectionLevel ecLevel = (ErrorCorrectionLevel)cmbErrorCorrectionLevel.SelectedItem;
            int version = (int)cmbMaxVersion.SelectedItem;
            bool allowStructuredAppend = chkStructuredAppend.Checked;
            Encoding encoding = ((EncodingInfo)cmbEncoding.SelectedItem).GetEncoding();

            Symbols symbols = new Symbols(ecLevel, version, allowStructuredAppend, encoding.WebName);
            
            try
            {
                symbols.AppendText(txtData.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            for (int i = 0; i < symbols.Count; ++i)
            {
                string filename;

                if (symbols.Count == 1)
                    filename = baseName;
                else
                    filename = baseName + "_" + (i + 1).ToString();

                symbols[i].SaveBitmap(filename + ".bmp", (int)nudModuleSize.Value, isMonochrome);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbErrorCorrectionLevel.DataSource =
                Enum.GetValues(typeof(ErrorCorrectionLevel));
            cmbErrorCorrectionLevel.SelectedItem = ErrorCorrectionLevel.M;

            for (int i = Constants.MIN_VERSION; i <= Constants.MAX_VERSION; ++i)
                cmbMaxVersion.Items.Add(i);

            cmbMaxVersion.SelectedIndex = cmbMaxVersion.Items.Count - 1;

            cmbEncoding.DisplayMember = "DisplayName";
            cmbEncoding.ValueMember = "Name";
            cmbEncoding.DataSource =  Encoding.GetEncodings();
            cmbEncoding.Text = Encoding.Default.EncodingName;
            
            nudModuleSize.Value = 4;
            chkStructuredAppend.Checked = false;
            btnSave.Enabled = false;
        }
    }
}