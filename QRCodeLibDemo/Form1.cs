using System;
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
            int moduleSize = (int)nudModuleSize.Value;

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
                Image image = symbol.GetImage(moduleSize);
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
            string ext;

            string[] filters = {
                "Monochrome Bitmap(*.bmp)|*.bmp",
                "24-bit Bitmap(*.bmp)|*.bmp",
                "SVG(*.svg)|*.svg"
            };

            using (var fd = new SaveFileDialog())
            {
                fd.Filter = String.Join("|", filters);

                if (fd.ShowDialog() != DialogResult.OK)
                    return;

                isMonochrome = fd.FilterIndex == 1;
                baseName = Path.Combine(
                    Path.GetDirectoryName(fd.FileName), 
                    Path.GetFileNameWithoutExtension(fd.FileName));

                switch (fd.FilterIndex)
                {
                    case 1:
                    case 2:
                        ext = FileExtension.BITMAP;
                        break;
                    case 3:
                        ext = FileExtension.SVG;
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }

            ErrorCorrectionLevel ecLevel = (ErrorCorrectionLevel)cmbErrorCorrectionLevel.SelectedItem;
            int version = (int)cmbMaxVersion.SelectedItem;
            bool allowStructuredAppend = chkStructuredAppend.Checked;
            Encoding encoding = ((EncodingInfo)cmbEncoding.SelectedItem).GetEncoding();
            int moduleSize = (int)nudModuleSize.Value;

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
                    filename = baseName + ext;
                else
                    filename = baseName + "_" + (i + 1).ToString() + ext;

                switch (ext)
                {
                    case FileExtension.BITMAP:
                        symbols[i].SaveBitmap(filename, moduleSize, isMonochrome);
                        break;
                    case FileExtension.SVG:
                        symbols[i].SaveSvg(filename, moduleSize);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbErrorCorrectionLevel.DataSource =
                Enum.GetValues(typeof(ErrorCorrectionLevel));
            cmbErrorCorrectionLevel.SelectedItem = ErrorCorrectionLevel.M;

            for (int i = 1; i <= 40; ++i)
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

    internal static class FileExtension
    {
        public const string BITMAP = ".bmp";
        public const string SVG = ".svg";
    }
}