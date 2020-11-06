using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

using Ys.Image;
using Ys.Misc;
using Ys.QRCode.Encoder;
using Ys.QRCode.Format;

namespace Ys.QRCode
{
    /// <summary>
    /// シンボルを表します。
    /// </summary>
    public class Symbol
    {
        const string WHITE = "#FFFFFF";
        const string BLACK = "#000000";

        const int DEFAULT_MODULE_SIZE = 4;

        readonly Symbols _parent;

        readonly int _position;
            
        QRCodeEncoder _currEncoder;
        EncodingMode  _currEncodingMode;
        int           _currVersion;

        int _dataBitCapacity;
        int _dataBitCounter;

        readonly List<QRCodeEncoder> _segments;
        readonly Dictionary<EncodingMode, int> _segmentCounter;

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="parent">親オブジェクト</param>
        internal Symbol(Symbols parent)
        {
            _parent  = parent;

            _position = parent.Count;

            _currEncoder      = null;
            _currEncodingMode = EncodingMode.UNKNOWN;
            _currVersion      = parent.MinVersion;
                
            _dataBitCapacity = 8 * DataCodeword.GetTotalNumber(
                parent.ErrorCorrectionLevel, parent.MinVersion);
            _dataBitCounter  = 0;

            _segments = new List<QRCodeEncoder>();
            _segmentCounter = new Dictionary<EncodingMode, int>(){
                {EncodingMode.NUMERIC,        0},
                {EncodingMode.ALPHA_NUMERIC,  0},
                {EncodingMode.EIGHT_BIT_BYTE, 0},
                {EncodingMode.KANJI,          0}
            };

            if (parent.StructuredAppendAllowed)
                _dataBitCapacity -= StructuredAppend.HEADER_LENGTH; 
        }
            
        /// <summary>
        /// 親オブジェクトを取得します。
        /// </summary>
        public Symbols Parent => _parent;

        /// <summary>
        /// 型番を取得します。
        /// </summary>
        public int Version => _currVersion;

        /// <summary>
        /// 現在の符号化モードを取得します。
        /// </summary>
        internal EncodingMode CurrentEncodingMode => _currEncodingMode;
            
        /// <summary>
        /// シンボルに文字を追加します。
        /// </summary>
        /// <param name="c">対象文字</param>
        /// <returns>シンボル容量が不足している場合は false を返します。</returns>
        internal bool TryAppend(char c)
        {
            int bitLength = _currEncoder.GetCodewordBitLength(c);

            while (_dataBitCapacity < _dataBitCounter + bitLength)
            {
                if (_currVersion >= _parent.MaxVersion)
                    return false;

                SelectVersion();
            }

            _currEncoder.Append(c);
            _dataBitCounter += bitLength;
            _parent.UpdateParity(c);
            return true;
        }

        /// <summary>
        /// 符号化モードを設定します。
        /// </summary>
        /// <param name="encMode">符号化モード</param>
        /// <param name="c">符号化する最初の文字。この文字はシンボルに追加されません。</param>
        /// <returns>シンボル容量が不足している場合は false を返します。</returns>
        internal bool TrySetEncodingMode(EncodingMode encMode, char c)
        {
            QRCodeEncoder encoder = QRCodeEncoder.CreateEncoder(
                    encMode, _parent.ByteModeEncoding);
            int bitLength = encoder.GetCodewordBitLength(c);

            while (_dataBitCapacity < 
                        _dataBitCounter
                        + ModeIndicator.LENGTH
                        + CharCountIndicator.GetLength(_currVersion, encMode)
                        + bitLength)
            {
                if (_currVersion >= _parent.MaxVersion)
                    return false;

                SelectVersion();
            }

            _dataBitCounter += ModeIndicator.LENGTH
                               + CharCountIndicator.GetLength(_currVersion, encMode);
            _currEncoder = encoder;
            _segments.Add(_currEncoder);
            _segmentCounter[encMode]++;
            _currEncodingMode = encMode;

            return true;
        }

        /// <summary>
        /// 型番を決定します。
        /// </summary>
        private void SelectVersion()
        {
            foreach (EncodingMode encMode in _segmentCounter.Keys)
            {
                int num = _segmentCounter[encMode];

                _dataBitCounter += 
                    num * CharCountIndicator.GetLength(_currVersion + 1, encMode) 
                    - num * CharCountIndicator.GetLength(_currVersion + 0, encMode);
            }
                
            _currVersion++;
            _dataBitCapacity = 8 * DataCodeword.GetTotalNumber(
                _parent.ErrorCorrectionLevel, _currVersion);
            _parent.MinVersion = _currVersion;

            if (_parent.StructuredAppendAllowed)
                _dataBitCapacity -= StructuredAppend.HEADER_LENGTH; 
        }

        /// <summary>
        /// データブロックを返します。
        /// </summary>
        private Byte[][] BuildDataBlock()
        {
            byte[] dataBytes = GetMessageBytes();
                
            int numPreBlocks = RSBlock.GetTotalNumber(
                    _parent.ErrorCorrectionLevel, _currVersion, true);  
            int numFolBlocks = RSBlock.GetTotalNumber(
                    _parent.ErrorCorrectionLevel, _currVersion, false); 

            byte[][] ret = new byte[numPreBlocks + numFolBlocks][];

            int numPreBlockDataCodewords = RSBlock.GetNumberDataCodewords(
                    _parent.ErrorCorrectionLevel, _currVersion, true);
            int index = 0;
            
            for (int i = 0; i < numPreBlocks; ++i)
            {
                byte[] data = new byte[numPreBlockDataCodewords];
                Array.Copy(dataBytes, index, data, 0, data.Length);
                index += data.Length;
                ret[i] = data;
            }
                
            int numFolBlockDataCodewords = RSBlock.GetNumberDataCodewords(
                    _parent.ErrorCorrectionLevel, _currVersion, false);

            for (int i = numPreBlocks; i < numPreBlocks + numFolBlocks; ++i)
            {
                byte[] data = new byte[numFolBlockDataCodewords];
                Array.Copy(dataBytes, index, data, 0, data.Length);
                index += data.Length;
                ret[i] = data;
            }
            return ret;
        }

        /// <summary>
        /// 誤り訂正データ領域のブロックを生成します。
        /// </summary>
        /// <param name="dataBlock">データ領域のブロック</param>
        private byte[][] BuildErrorCorrectionBlock(byte[][] dataBlock)
        {
            int numECCodewords = RSBlock.GetNumberECCodewords(
                    _parent.ErrorCorrectionLevel, _currVersion);
            int numPreBlocks = RSBlock.GetTotalNumber(
                    _parent.ErrorCorrectionLevel, _currVersion, true);
            int numFolBlocks = RSBlock.GetTotalNumber(
                    _parent.ErrorCorrectionLevel, _currVersion, false);

            byte[][] ret = new byte[numPreBlocks + numFolBlocks][];

            for (int i = 0; i < ret.Length; ++i)
                ret[i] = new byte[numECCodewords];

            int[] gp = GeneratorPolynomials.Item(numECCodewords);

            for (int blockIndex = 0; blockIndex < dataBlock.Length; ++blockIndex)
            {
                int size = dataBlock[blockIndex].Length + ret[blockIndex].Length;
                int[] data = new int[size];
                int eccIndex = data.Length - 1;

                for (int i = 0; i < dataBlock[blockIndex].Length; ++i)
                {
                    data[eccIndex] = dataBlock[blockIndex][i];
                    eccIndex--;
                }

                for (int i = data.Length - 1; i >= numECCodewords; --i)
                {
                    if (data[i] > 0)
                    {
                        int exp = GaloisField256.ToExp(data[i]);
                        eccIndex = i;

                        for (int j = gp.Length - 1; j >= 0; --j)
                        {
                            data[eccIndex] ^= GaloisField256.ToInt((gp[j] + exp) % 255);
                            eccIndex--;
                        }
                    }
                }
                    
                eccIndex = numECCodewords - 1;

                for (int i = 0; i < ret[blockIndex].Length; ++i)
                {
                    ret[blockIndex][i] = (byte)data[eccIndex];
                    eccIndex--;
                }
            }
            return ret;
        }

        /// <summary>
        /// 符号化領域のバイトデータを返します。
        /// </summary>
        private byte[] GetEncodingRegionBytes()
        {
            byte[][] dataBlock  = BuildDataBlock();
            byte[][] ecBlock    = BuildErrorCorrectionBlock(dataBlock);

            int numCodewords = Codeword.GetTotalNumber(_currVersion); 
            int numDataCodewords = DataCodeword.GetTotalNumber(
                    _parent.ErrorCorrectionLevel, _currVersion);
               
            byte[] ret = new byte[numCodewords];

            int index = 0;
            int n;

            n = 0;
            while (index < numDataCodewords)
            {
                int r = n % dataBlock.Length;
                int c = n / dataBlock.Length;

                if (c <= dataBlock[r].Length - 1)
                {
                    ret[index] = dataBlock[r][c];
                    index++;
                }
                n++;
            }
                
            n = 0;
            while(index < numCodewords)
            {
                int r = n % ecBlock.Length;
                int c = n / ecBlock.Length;

                if (c <= ecBlock[r].Length - 1)
                {
                    ret[index] = ecBlock[r][c];
                    index++;
                }
                n++;
            }
            return ret;
        }

        /// <summary>
        /// コード語に変換するメッセージビット列を返します。
        /// </summary>
        private byte[] GetMessageBytes()
        {
            var bs = new BitSequence();

            if (_parent.Count > 1)
                WriteStructuredAppendHeader(bs);

            WriteSegments(bs);
            WriteTerminator(bs);
            WritePaddingBits(bs);
            WritePadCodewords(bs);

            return bs.GetBytes();
        }

        private void WriteStructuredAppendHeader(BitSequence bs)
        {
            bs.Append(ModeIndicator.STRUCTURED_APPEND_VALUE, 
                      ModeIndicator.LENGTH); 
            bs.Append(_position, 
                      SymbolSequenceIndicator.POSITION_LENGTH); 
            bs.Append(_parent.Count - 1, 
                      SymbolSequenceIndicator.TOTAL_NUMBER_LENGTH); 
            bs.Append(_parent.StructuredAppendParity, 
                      StructuredAppend.PARITY_DATA_LENGTH);  
        }
        
        private void WriteSegments(BitSequence bs)
        {
            foreach (QRCodeEncoder segment in _segments)
            {
                bs.Append(segment.ModeIndicator, ModeIndicator.LENGTH); 
                bs.Append(segment.CharCount, 
                          CharCountIndicator.GetLength(
                              _currVersion, segment.EncodingMode)
                ); 
                
                byte[] data = segment.GetBytes();

                for (int i = 0; i < data.Length - 1; ++i)
                    bs.Append(data[i], 8);

                int codewordBitLength = segment.BitCount % 8; 

                if (codewordBitLength == 0)
                    codewordBitLength = 8;

                bs.Append(data[data.Length - 1] >> (8 - codewordBitLength), 
                          codewordBitLength);
            }
        }

        private void WriteTerminator(BitSequence bs)
        {
            int terminatorLength = Math.Min(
                    ModeIndicator.LENGTH, _dataBitCapacity - _dataBitCounter);
            bs.Append(ModeIndicator.TERMINATOR_VALUE, terminatorLength);
        }

        private void WritePaddingBits(BitSequence bs)
        {
            if (bs.Length % 8 > 0)
                bs.Append(0x0, 8 - (bs.Length % 8));
        }

        private void WritePadCodewords(BitSequence bs)
        {
            int numDataCodewords = DataCodeword.GetTotalNumber(
                    _parent.ErrorCorrectionLevel, _currVersion);

            bool flag = true;

            while (bs.Length < 8 * numDataCodewords)
            {
                bs.Append(flag ? 236 : 17, 8);
                flag = !flag;
            }
        }

        /// <summary>
        /// シンボルの明暗パターンを返します。
        /// </summary>
        private int[][] GetModuleMatrix()
        {
            int numModulesPerSide = Module.GetNumModulesPerSide(_currVersion);

            int[][] moduleMatrix = new int[numModulesPerSide][];

            for (int i = 0; i < moduleMatrix.Length; ++i)
                moduleMatrix[i] = new int[moduleMatrix.Length];

            FinderPattern.Place(moduleMatrix);
            Separator.Place(moduleMatrix);
            TimingPattern.Place(moduleMatrix);

            if (_currVersion >= 2)
                AlignmentPattern.Place(_currVersion, moduleMatrix);

            FormatInfo.PlaceTempBlank(moduleMatrix);

            if (_currVersion >= 7)
                VersionInfo.PlaceTempBlank(moduleMatrix);

            PlaceSymbolChar(moduleMatrix);
            RemainderBit.Place(moduleMatrix);

            Masking.Apply(_currVersion, _parent.ErrorCorrectionLevel, ref moduleMatrix);

            return moduleMatrix;
        }

        /// <summary>
        /// シンボルキャラクタを配置します。
        /// </summary>
        private void PlaceSymbolChar(int[][] moduleMatrix)
        {
            byte[]  data = GetEncodingRegionBytes();
                
            int r = moduleMatrix.Length - 1;
            int c = moduleMatrix[0].Length - 1;

            bool toLeft = true;
            int rowDirection = -1;

            foreach(byte value in data)
            {
                int bitPos = 7;

                while (bitPos >= 0)
                {
                    if (moduleMatrix[r][c] == 0)
                    {
                        moduleMatrix[r][c] = (value & (1 << bitPos)) > 0 ? 1 : -1;
                        bitPos--;
                    }

                    if (toLeft)
                        c--;
                    else
                    {
                        if ((r + rowDirection) < 0)
                        {
                            r = 0;
                            rowDirection = 1;
                            c--;

                            if (c == 6)
                                c = 5;
                        }
                        else if ((r + rowDirection) > (moduleMatrix.Length - 1))
                        {
                            r = moduleMatrix.Length - 1;
                            rowDirection = -1;
                            c--;

                            if (c == 6)
                                c = 5;
                        }
                        else
                        {
                            r += rowDirection;
                            c++;
                        }
                    }
                    toLeft = !toLeft;
                }
            }
        }
        
        /// <summary>
        /// ビットマップファイルのバイトデータを返します。
        /// </summary>
        /// <param name="moduleSize">モジュールサイズ(px)</param>
        /// <param name="monochrome">1bitカラーはtrue、24bitカラーはfalseを設定します。</param>
        /// <param name="foreRgb">前景色</param>
        /// <param name="backRgb">背景色</param>
        public byte[] GetBitmap(int moduleSize = DEFAULT_MODULE_SIZE, 
                                bool monochrome = false,
                                string foreRgb = BLACK, 
                                string backRgb = WHITE)
        {
            if (_dataBitCounter == 0)
                throw new InvalidOperationException();

            if (moduleSize < 1)
                throw new ArgumentOutOfRangeException(nameof(moduleSize));

            if (ColorCode.IsWebColor(foreRgb) == false)
                throw new FormatException(nameof(foreRgb));

            if (ColorCode.IsWebColor(foreRgb) == false)
                throw new FormatException(nameof(backRgb));

            if (monochrome)
                return GetBitmap1bpp(moduleSize, foreRgb, backRgb);
            else
                return GetBitmap24bpp(moduleSize, foreRgb, backRgb);
        }

        /// <summary>
        /// 1bppビットマップファイルのバイトデータを返します。
        /// </summary>
        /// <param name="moduleSize">モジュールサイズ(px)</param>
        /// <param name="foreRgb">前景色</param>
        /// <param name="backRgb">背景色</param>
        private byte[] GetBitmap1bpp(int moduleSize, string foreRgb, string backRgb)
        {
            Color foreColor = ColorTranslator.FromHtml(foreRgb);
            Color backColor = ColorTranslator.FromHtml(backRgb);
            
            int[][] moduleMatrix = QuietZone.Place(GetModuleMatrix());

            int width, height;
            width = height = moduleSize * moduleMatrix.Length;

            int rowBytesLen = (width + 7) / 8;

            int pack8bit = 0;
            if (width % 8 > 0)
                pack8bit = 8 - (width % 8);

            int pack32bit = 0;
            if (rowBytesLen % 4 > 0)
                pack32bit = 8 * (4 - (rowBytesLen % 4));

            int rowSize = (width + pack8bit + pack32bit) / 8;
            byte[] bitmapData = new byte[rowSize * height];
            int offset = 0;

            for (int r = moduleMatrix.Length - 1; r >= 0; --r)
            {
                var bs = new BitSequence();
                
                foreach (int value in moduleMatrix[r])
                {
                    int color = value > 0 ? 0 : 1;

                    for (int i = 1; i <= moduleSize; ++i)
                        bs.Append(color, 1);
                }
                bs.Append(0, pack8bit);
                bs.Append(0, pack32bit);

                byte[] bitmapRow = bs.GetBytes();

                for (int i = 1; i <= moduleSize; ++i)
                {
                    Array.Copy(bitmapRow, 0, bitmapData, offset, rowSize);
                    offset += rowSize;
                }
            }

            return DIB.Build1bppDIB(bitmapData, width, height, foreColor, backColor);
        }
        
        /// <summary>
        /// 24bppビットマップファイルのバイトデータを返します。
        /// </summary>
        /// <param name="moduleSize">モジュールサイズ(px)</param>
        /// <param name="foreRgb">前景色</param>
        /// <param name="backRgb">背景色</param>
        private byte[] GetBitmap24bpp(int moduleSize, string foreRgb, string backRgb)
        {
            Color foreColor = ColorTranslator.FromHtml(foreRgb);
            Color backColor = ColorTranslator.FromHtml(backRgb);

            int[][] moduleMatrix = QuietZone.Place(GetModuleMatrix());

            int width, height;
            width = height = moduleSize * moduleMatrix.Length;

            int rowBytesLen = 3 * width;

            int pack4byte = 0;
            if (rowBytesLen % 4 > 0)
                pack4byte = 4 - (rowBytesLen % 4);

            int rowSize = rowBytesLen + pack4byte;
            byte[] bitmapData = new byte[rowSize * height];
            int offset = 0;

            for (int r = moduleMatrix.Length - 1; r >= 0; --r)
            {
                byte[] bitmapRow = new byte[rowSize];
                int index = 0;

                foreach(int value in moduleMatrix[r])
                {
                    Color color = value > 0 ? foreColor : backColor;

                    for (int i = 1; i <= moduleSize; ++i)
                    {
                        bitmapRow[index++] = color.B;
                        bitmapRow[index++] = color.G;
                        bitmapRow[index++] = color.R;
                    }
                }

                for (int i = 1; i <= moduleSize; ++i)
                {
                    Array.Copy(bitmapRow, 0, bitmapData, offset, rowSize);
                    offset += rowSize;
                }
            }

            return DIB.Build24bppDIB(bitmapData, width, height);
        }

        /// <summary>
        /// Base64エンコードされたビットマップデータを返します。
        /// </summary>
        /// <param name="moduleSize">モジュールサイズ(px)</param>
        /// <param name="monochrome">1bitカラーはtrue、24bitカラーはfalseを設定します。</param>
        /// <param name="foreRgb">前景色</param>
        /// <param name="backRgb">背景色</param>
        public string GetBitmapBase64(int moduleSize = DEFAULT_MODULE_SIZE, 
                                      bool monochrome = false,
                                      string foreRgb = BLACK, 
                                      string backRgb = WHITE)
        {
            if (_dataBitCounter == 0)
                throw new InvalidOperationException();

            if (moduleSize < 1)
                throw new ArgumentOutOfRangeException(nameof(moduleSize));

            if (ColorCode.IsWebColor(foreRgb) == false)
                throw new FormatException(nameof(foreRgb));

            if (ColorCode.IsWebColor(foreRgb) == false)
                throw new FormatException(nameof(backRgb));

            byte[] dib;

            if (monochrome)
                dib = GetBitmap1bpp(moduleSize, foreRgb, backRgb);
            else
                dib = GetBitmap24bpp(moduleSize, foreRgb, backRgb);
                
            return Convert.ToBase64String(dib);
        }

        /// <summary>
        /// シンボルのImageオブジェクトを返します。
        /// </summary>
        /// <param name="moduleSize">モジュールサイズ(px)</param>
        /// <param name="monochrome">1bitカラーはtrue、24bitカラーはfalseを設定します。</param>
        /// <param name="foreRgb">前景色</param>
        /// <param name="backRgb">背景色</param>
        public System.Drawing.Image GetImage(int moduleSize = DEFAULT_MODULE_SIZE, 
                                             bool monochrome = false,
                                             string foreRgb = BLACK, 
                                             string backRgb = WHITE)
        {
            if (_dataBitCounter == 0)
                throw new InvalidOperationException();

            if (moduleSize < 1 )
                throw new ArgumentOutOfRangeException(nameof(moduleSize));

            if (ColorCode.IsWebColor(foreRgb) == false)
                throw new FormatException(nameof(foreRgb));

            if (ColorCode.IsWebColor(foreRgb) == false)
                throw new FormatException(nameof(backRgb));

            byte[] dib;

            if (monochrome)
                dib = GetBitmap1bpp(moduleSize, foreRgb, backRgb);
            else
                dib = GetBitmap24bpp(moduleSize, foreRgb, backRgb);

            ImageConverter converter = new ImageConverter();
            return (System.Drawing.Image)converter.ConvertFrom(dib);
        }

        /// <summary>
        /// シンボルをBMP形式でファイルに保存します。
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <param name="moduleSize">モジュールサイズ(px)</param>
        /// <param name="monochrome">1bitカラーはtrue、24bitカラーはfalseを設定します。</param>
        /// <param name="foreRgb">前景色</param>
        /// <param name="backRgb">背景色</param>
        public void SaveBitmap(string fileName, 
                               int moduleSize = DEFAULT_MODULE_SIZE, 
                               bool monochrome = false,
                               string foreRgb = BLACK, 
                               string backRgb = WHITE)
        {
            if (_dataBitCounter == 0)
                throw new InvalidOperationException();

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));

            if (moduleSize < 1)
                throw new ArgumentOutOfRangeException(nameof(moduleSize));

            if (ColorCode.IsWebColor(foreRgb) == false)
                throw new FormatException(nameof(foreRgb));

            if (ColorCode.IsWebColor(foreRgb) == false)
                throw new FormatException(nameof(backRgb));

            byte[] dib;

            if (monochrome)
                dib = GetBitmap1bpp(moduleSize, foreRgb, backRgb);
            else
                dib = GetBitmap24bpp(moduleSize, foreRgb, backRgb);
            
            File.WriteAllBytes(fileName, dib);
        }

        /// <summary>
        /// シンボルをSVG形式でファイルに保存します。
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <param name="moduleSize">モジュールサイズ(px)</param>
        /// <param name="foreRgb">前景色</param>
        public void SaveSvg(string fileName,
                            int moduleSize = DEFAULT_MODULE_SIZE,
                            string foreRgb = BLACK)
        {
            if (_dataBitCounter == 0)
                throw new InvalidOperationException();

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));

            if (moduleSize < 2)
                throw new ArgumentOutOfRangeException(nameof(moduleSize));

            if (ColorCode.IsWebColor(foreRgb) == false)
                throw new FormatException(nameof(foreRgb));

            string newLine = Environment.NewLine;

            string svg = GetSvg(moduleSize, foreRgb);
            string svgFile =
                $"<?xml version='1.0' encoding='UTF-8' standalone='no'?>{newLine}" +
                $"<!DOCTYPE svg PUBLIC '-//W3C//DTD SVG 20010904//EN'{newLine}" +
                $"    'http://www.w3.org/TR/2001/REC-SVG-20010904/DTD/svg10.dtd'>{newLine}" +
                svg + newLine;

            File.WriteAllText(fileName, svgFile);
        }

        public string GetSvg(int moduleSize = DEFAULT_MODULE_SIZE,
                             string foreRgb = BLACK)
        {
            if (_dataBitCounter == 0)
                throw new InvalidOperationException();

            if (moduleSize < 2)
                throw new ArgumentOutOfRangeException(nameof(moduleSize));

            if (ColorCode.IsWebColor(foreRgb) == false)
                throw new FormatException(nameof(foreRgb));

            int[][] moduleMatrix = QuietZone.Place(GetModuleMatrix());

            int width, height;
            width = height = moduleSize * moduleMatrix.Length;

            int[][] image = new int[height][];
            for (int i = 0; i < image.Length; ++i)
                image[i] = new int[width];

            int r = 0;
            foreach (var row in moduleMatrix)
            {
                for (int i = 0; i < moduleSize; ++i)
                {
                    int c = 0;
                    foreach (var value in row)
                    {
                        for (int j = 0; j < moduleSize; ++j)
                        {
                            image[r][c] = value;
                            c++;
                        }
                    }
                    r++;
                }
            }

            Point[][] paths = FindContours(image);
            var buf = new StringBuilder();
            string indent = new string(' ', 11);

            foreach (var path in paths)
            {
                buf.Append($"{indent}M ");

                foreach (var p in path)
                    buf.Append($"{p.X},{p.Y} ");

                buf.AppendLine("Z");
            }

            string newLine = Environment.NewLine;
            string data = buf.ToString().Trim();
            string svg =
                $"<svg xmlns='http://www.w3.org/2000/svg'{newLine}" +
                $"    width='{width}' height='{height}' viewBox='0 0 {width} {height}'>{newLine}" +
                $"    <path fill='{foreRgb}' stroke='{foreRgb}' stroke-width='1'{newLine}" +
                $"        d='{data}'{newLine}" +
                $"    />{newLine}" +
                $"</svg>";

            return svg;
        }

        private Point[][] FindContours(int[][] image)
        {
            var paths = new List<Point[]>();
            List<Point> path;
            Point start;
            Direction dr;

            for (int y = 0; y < image.Length - 1; ++y)
            {
                for (int x = 0; x < image[y].Length - 1; ++x)
                {
                    if (image[y][x] == int.MaxValue)
                        continue;

                    if (!(image[y][x] > 0 && image[y][x + 1] <= 0))
                        continue;

                    image[y][x] = int.MaxValue;
                    start = new Point(x, y);
                    path = new List<Point> { start };

                    dr = Direction.UP;
                    Point p = new Point(start.X, start.Y - 1);

                    do
                    {
                        switch (dr)
                        {
                            case Direction.UP:
                                if (image[p.Y][p.X] > 0)
                                {
                                    image[p.Y][p.X] = int.MaxValue;

                                    if (image[p.Y][p.X + 1] <= 0)
                                        p = new Point(p.X, p.Y - 1);
                                    else
                                    {
                                        path.Add(p);
                                        dr = Direction.RIGHT;
                                        p = new Point(p.X + 1, p.Y);
                                    }
                                }
                                else
                                {
                                    p = new Point(p.X, p.Y + 1);
                                    path.Add(p);
                                    dr = Direction.LEFT;
                                    p = new Point(p.X - 1, p.Y);
                                }
                                break;

                            case Direction.DOWN:
                                if (image[p.Y][p.X] > 0)
                                {
                                    image[p.Y][p.X] = int.MaxValue;

                                    if (image[p.Y][p.X - 1] <= 0)
                                        p = new Point(p.X, p.Y + 1);
                                    else
                                    {
                                        path.Add(p);
                                        dr = Direction.LEFT;
                                        p = new Point(p.X - 1, p.Y);
                                    }
                                }
                                else
                                {
                                    p = new Point(p.X, p.Y - 1);
                                    path.Add(p);
                                    dr = Direction.RIGHT;
                                    p = new Point(p.X + 1, p.Y);
                                }
                                break;

                            case Direction.LEFT:
                                if (image[p.Y][p.X] > 0)
                                {
                                    image[p.Y][p.X] = int.MaxValue;

                                    if (image[p.Y - 1][p.X] <= 0)
                                        p = new Point(p.X - 1, p.Y);
                                    else
                                    {
                                        path.Add(p);
                                        dr = Direction.UP;
                                        p = new Point(p.X, p.Y - 1);
                                    }
                                }
                                else
                                {
                                    p = new Point(p.X + 1, p.Y);
                                    path.Add(p);
                                    dr = Direction.DOWN;
                                    p = new Point(p.X, p.Y + 1);
                                }
                                break;

                            case Direction.RIGHT:
                                if (image[p.Y][p.X] > 0)
                                {
                                    image[p.Y][p.X] = int.MaxValue;

                                    if (image[p.Y + 1][p.X] <= 0)
                                        p = new Point(p.X + 1, p.Y);
                                    else
                                    {
                                        path.Add(p);
                                        dr = Direction.DOWN;
                                        p = new Point(p.X, p.Y + 1);
                                    }
                                }
                                else
                                {
                                    p = new Point(p.X - 1, p.Y);
                                    path.Add(p);
                                    dr = Direction.UP;
                                    p = new Point(p.X, p.Y - 1);
                                }
                                break;

                            default:
                                throw new InvalidOperationException();
                        }

                    } while (p != start);

                    paths.Add(path.ToArray());
                }
            }

            return paths.ToArray();
        }
    }
}
