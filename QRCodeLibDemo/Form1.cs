using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using Ys.QRCode;

namespace Example
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

            Symbols symbols;

            try
            {
                symbols = new Symbols(
                    Int32.Parse(cmbMaxVersion.Text),
                    (ErrorCorrectionLevel)cmbErrorCorrectionLevel.SelectedItem,
                    chkStructuredAppend.Checked,
                    ((EncodingInfo)cmbEncoding.SelectedItem).GetEncoding());

                symbols.AppendString(txtData.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            foreach (Symbol symbol in symbols)
            {
                Image image = symbol.Get24bppImage((int)nudModuleSize.Value);

                PictureBox pictureBox = new PictureBox();
                pictureBox.Size = image.Size;
                pictureBox.Image = image;

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
                Path.GetDirectoryName(fd.FileName), Path.GetFileNameWithoutExtension(fd.FileName));
            }

            Symbols symbols;

            try
            {
                symbols = new Symbols(
                    Int32.Parse(cmbMaxVersion.Text),
                    (ErrorCorrectionLevel)(cmbErrorCorrectionLevel.SelectedItem),
                    chkStructuredAppend.Checked,
                    ((EncodingInfo)cmbEncoding.SelectedItem).GetEncoding());

                symbols.AppendString(txtData.Text);
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

                if (isMonochrome)
                    symbols[i].Save1bppDIB(filename + ".bmp", (int)nudModuleSize.Value);
                else
                    symbols[i].Save24bppDIB(filename + ".bmp", (int)nudModuleSize.Value);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 1; i <= 40; ++i)
                cmbMaxVersion.Items.Add(i);

            cmbErrorCorrectionLevel.DataSource =
                Enum.GetValues(typeof(ErrorCorrectionLevel));

            cmbEncoding.DisplayMember = "DisplayName";
            cmbEncoding.ValueMember = "Name";
            cmbEncoding.DataSource =  Encoding.GetEncodings();
            
            cmbMaxVersion.Text = "40";
            cmbErrorCorrectionLevel.Text = "M";
            cmbEncoding.Text = Encoding.Default.EncodingName;
            nudModuleSize.Value = 5;
            chkStructuredAppend.Checked = false;
            btnSave.Enabled = false;
        }
    }
}