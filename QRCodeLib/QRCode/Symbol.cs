using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

using Ys.QRCode.Encoder;
using Ys.QRCode.Format;
using Ys.Image;
using Ys.Util;

namespace Ys.QRCode
{
    /// <summary>
    /// シンボルを表します。
    /// </summary>
    public class Symbol
    {
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
                
            _dataBitCapacity = DataCodeword.GetTotalNumber(
                parent.ErrorCorrectionLevel, parent.MinVersion) * 8;

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
        /// 親オブジェクトを取得します。
        /// </summary>
        public Symbols Parent
        {
            get { return _parent; }
        }

        /// <summary>
        /// 型番を取得します。
        /// </summary>
        public int Version
        {
            get { return _currVersion; }
        }

        /// <summary>
        /// 現在の符号化モードを取得します。
        /// </summary>
        internal EncodingMode CurrentEncodingMode
        {
            get { return _currEncodingMode; }
        }
            
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
            QRCodeEncoder encoder = QRCodeEncoder.CreateEncoder(encMode, _parent.ByteModeEncoding);
            int bitLength = encoder.GetCodewordBitLength(c);

            while (_dataBitCapacity <
                        _dataBitCounter +
                        ModeIndicator.LENGTH +
                        CharCountIndicator.GetLength(_currVersion, encMode) +
                        bitLength)
            {
                if (_currVersion >= _parent.MaxVersion)
                    return false;

                SelectVersion();
            }

            _dataBitCounter += ModeIndicator.LENGTH +
                               CharCountIndicator.GetLength(_currVersion, encMode);

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

                _dataBitCounter += num * CharCountIndicator.GetLength(_currVersion + 1, encMode) -
                                   num * CharCountIndicator.GetLength(_currVersion + 0, encMode);
            }
                
            _currVersion++;
            _dataBitCapacity = DataCodeword.GetTotalNumber(
                _parent.ErrorCorrectionLevel, _currVersion) * 8;
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

            int index = 0;

            int numPreBlockDataCodewords = RSBlock.GetNumberDataCodewords(
                    _parent.ErrorCorrectionLevel, _currVersion, true);

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
                int[] data = new int[dataBlock[blockIndex].Length + ret[blockIndex].Length];
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
            bs.Append(ModeIndicator.STRUCTURED_APPEND_VALUE, ModeIndicator.LENGTH); 
            bs.Append(_position, SymbolSequenceIndicator.POSITION_LENGTH); 
            bs.Append(_parent.Count - 1, SymbolSequenceIndicator.TOTAL_NUMBER_LENGTH); 
            bs.Append(_parent.StructuredAppendParity, StructuredAppend.PARITY_DATA_LENGTH);  
        }
        
        private void WriteSegments(BitSequence bs)
        {
            foreach (QRCodeEncoder segment in _segments)
            {
                bs.Append(segment.ModeIndicator, ModeIndicator.LENGTH); 
                bs.Append(segment.CharCount, 
                          CharCountIndicator.GetLength(_currVersion, segment.EncodingMode)); 
                
                byte[] data = segment.GetBytes();

                for (int i = 0; i <= data.Length - 2; ++i)
                    bs.Append(data[i], 8);

                int codewordBitLength = segment.BitCount % 8; 

                if (codewordBitLength == 0)
                    codewordBitLength = 8;

                bs.Append(data[data.Length - 1] >> (8 - codewordBitLength), codewordBitLength);
            }
        }

        private void WriteTerminator(BitSequence bs)
        {
            int terminatorLength = _dataBitCapacity - _dataBitCounter;

            if (terminatorLength > ModeIndicator.LENGTH)
                terminatorLength = ModeIndicator.LENGTH;

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

            while (bs.Length < numDataCodewords * 8)
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
                AlignmentPattern.Place(moduleMatrix, _currVersion);

            FormatInfo.PlaceTempBlank(moduleMatrix);

            if (_currVersion >= 7)
                VersionInfo.PlaceTempBlank(moduleMatrix);

            PlaceSymbolChar(moduleMatrix);

            RemainderBit.Place(moduleMatrix);

            int maskPatternReference = Masking.Apply(
                    moduleMatrix, _currVersion, _parent.ErrorCorrectionLevel);

            FormatInfo.Place(moduleMatrix, _parent.ErrorCorrectionLevel, maskPatternReference);

            if (_currVersion >= 7)
                VersionInfo.Place(moduleMatrix, _currVersion);

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

            for (int i = 0; i < data.Length; ++i)
            {
                int bitPos = 7;

                while (bitPos >= 0)
                {
                    if (moduleMatrix[r][c] == 0)
                    {
                        moduleMatrix[r][c] = (data[i] & (1 << bitPos)) > 0 ? 1 : -1;
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
        /// 1bppビットマップファイルのバイトデータを返します。
        /// </summary>
        public byte[] Get1bppDIB()
        {
            return Get1bppDIB(5);
        }

        /// <summary>
        /// 1bppビットマップファイルのバイトデータを返します。
        /// </summary>
        /// <param name="moduleSize">モジュールサイズ(px)</param>
        public byte[] Get1bppDIB(int moduleSize)
        {
            if (moduleSize < 1)
                throw new ArgumentOutOfRangeException(nameof(moduleSize));

            return Get1bppDIB(moduleSize, Color.Black, Color.White);
        }

        /// <summary>
        /// 1bppビットマップファイルのバイトデータを返します。
        /// </summary>
        /// <param name="moduleSize">モジュールサイズ(px)</param>
        /// <param name="foreColor">前景色</param>
        /// <param name="backColor">背景色</param>
        public byte[] Get1bppDIB(int moduleSize, Color foreColor,Color backColor)
        {
            if (moduleSize < 1)
                throw new ArgumentOutOfRangeException(nameof(moduleSize));

            int[][] moduleMatrix = QuietZone.Place(GetModuleMatrix());

            int width  = moduleMatrix.Length * moduleSize;
            int height = width;

            int hByteLen = (width + 7) / 8;

            int pack8bit  = (width % 8) == 0 ? 0 : 8 - (width % 8);
            int pack32bit = (hByteLen % 4) == 0 ? 0 : (4 - (hByteLen % 4)) * 8;

            var bs = new BitSequence();

            for (int r = moduleMatrix.Length - 1; r >= 0; --r)
            {
                for (int i = 1; i <= moduleSize; ++i)
                {
                    for (int c = 0; c < moduleMatrix[r].Length; ++c)
                        for (int j = 1; j <= moduleSize; ++j)
                            bs.Append(moduleMatrix[r][c] > 0 ? 0 : 1, 1);

                    bs.Append(0, pack8bit);
                    bs.Append(0, pack32bit);
                }
            }

            byte[] dataBlock = bs.GetBytes();

            BITMAPFILEHEADER bfh;
            BITMAPINFOHEADER bih;
            RGBQUAD[]        palette;

            bfh.bfType         = 0x4D42;
            bfh.bfSize         = 62 + dataBlock.Length;
            bfh.bfReserved1    = 0;
            bfh.bfReserved2    = 0;
            bfh.bfOffBits      = 62;

            bih.biSize             = 40;
            bih.biWidth            = width;
            bih.biHeight           = height;
            bih.biPlanes           = 1;
            bih.biBitCount         = 1;
            bih.biCompression      = 0;
            bih.biSizeImage        = 0;
            bih.biXPelsPerMeter    = 3780; // 96dpi
            bih.biYPelsPerMeter    = 3780; // 96dpi
            bih.biClrUsed          = 0;
            bih.biClrImportant     = 0;

            palette = new RGBQUAD[2];

            palette[0].rgbBlue     = foreColor.B;
            palette[0].rgbGreen    = foreColor.G;
            palette[0].rgbRed      = foreColor.R;
            palette[0].rgbReserved = 0;

            palette[1].rgbBlue     = backColor.B;
            palette[1].rgbGreen    = backColor.G;
            palette[1].rgbRed      = backColor.R;
            palette[1].rgbReserved = 0; 

            byte[] ret = new byte[62 + dataBlock.Length];

            byte[] bytes;
            int offset= 0;
            
            bytes = bfh.GetBytes();
            Buffer.BlockCopy(bytes, 0, ret, offset, bytes.Length);
            offset += bytes.Length;

            bytes = bih.GetBytes();
            Buffer.BlockCopy(bytes, 0, ret, offset, bytes.Length);
            offset += bytes.Length;

            bytes = palette[0].GetBytes();
            Buffer.BlockCopy(bytes, 0, ret, offset, bytes.Length);
            offset += bytes.Length;

            bytes = palette[1].GetBytes();
            Buffer.BlockCopy(bytes, 0, ret, offset, bytes.Length);
            offset += bytes.Length;

            bytes = dataBlock;
            Buffer.BlockCopy(bytes, 0, ret,  offset, bytes.Length);

            return ret;
        }

        /// <summary>
        /// 24bppビットマップファイルのバイトデータを返します。
        /// </summary>
        public byte[] Get24bppDIB()
        {
            return Get24bppDIB(5);
        }
        
        /// <summary>
        /// 24bppビットマップファイルのバイトデータを返します。
        /// </summary>
        /// <param name="moduleSize">モジュールサイズ(px)</param>
        public byte[] Get24bppDIB(int moduleSize)
        {
            if (moduleSize < 1)
                throw new ArgumentOutOfRangeException(nameof(moduleSize));

            return Get24bppDIB(moduleSize, Color.Black, Color.White);
        }

        /// <summary>
        /// 24bppビットマップファイルのバイトデータを返します。
        /// </summary>
        /// <param name="moduleSize">モジュールサイズ(px)</param>
        /// <param name="foreColor">前景色</param>
        /// <param name="backColor">背景色</param>
        public byte[] Get24bppDIB(int moduleSize, Color foreColor,Color backColor)
        {
            if (moduleSize < 1)
                throw new ArgumentOutOfRangeException(nameof(moduleSize));

            int[][] moduleMatrix = QuietZone.Place(GetModuleMatrix());

            int width  = moduleMatrix.Length * moduleSize;
            int height = width;

            int hByteLen = width * 3;
                
            int pack4byte = hByteLen % 4 == 0 ? 0 : 4 - (hByteLen % 4);

            byte[] dataBlock = new byte[(hByteLen + pack4byte) * (height * 3)];

            int idx = 0;

            for (int r = moduleMatrix.Length - 1; r >= 0; --r)
            {
                for (int i = 1; i <= moduleSize; ++i)
                {
                    for (int c = 0; c < moduleMatrix[r].Length; ++c)
                    { 
                        for (int j = 1; j <= moduleSize; ++j)
                        {
                            Color color = moduleMatrix[r][c] > 0 ? foreColor : backColor;
                            dataBlock[idx + 0] = color.B;
                            dataBlock[idx + 1] = color.G;
                            dataBlock[idx + 2] = color.R;
                            idx += 3;
                        }
                    }

                    idx += pack4byte;
                }
            }
                
            BITMAPFILEHEADER bfh;
            BITMAPINFOHEADER bih;

            bfh.bfType         = 0x4D42;
            bfh.bfSize         = 54 + dataBlock.Length;
            bfh.bfReserved1    = 0;
            bfh.bfReserved2    = 0;
            bfh.bfOffBits      = 54;

            bih.biSize             = 40;
            bih.biWidth            = width;
            bih.biHeight           = height;
            bih.biPlanes           = 1;
            bih.biBitCount         = 24;
            bih.biCompression      = 0;
            bih.biSizeImage        = 0;
            bih.biXPelsPerMeter    = 3780; // 96dpi
            bih.biYPelsPerMeter    = 3780; // 96dpi
            bih.biClrUsed          = 0;
            bih.biClrImportant     = 0;
                
            byte[] ret = new byte[54 + dataBlock.Length];

            byte[] bytes;
            int offset= 0;
            
            bytes = bfh.GetBytes();
            Buffer.BlockCopy(bytes, 0, ret, offset, bytes.Length);
            offset += bytes.Length;

            bytes = bih.GetBytes();
            Buffer.BlockCopy(bytes, 0, ret, offset, bytes.Length);
            offset += bytes.Length;
                
            bytes = dataBlock;
            Buffer.BlockCopy(bytes, 0, ret,  offset, bytes.Length);

            return ret;
        }

        /// <summary>
        /// 1bppのシンボル画像を返します。
        /// </summary>
        /// <param name="moduleSize">モジュールサイズ(px)</param>
        public System.Drawing.Image Get1bppImage()
        {
            return Get1bppImage(5);
        }
        
        /// <summary>
        /// 1bppのシンボル画像を返します。
        /// </summary>
        /// <param name="moduleSize">モジュールサイズ(px)</param>
        public System.Drawing.Image Get1bppImage(int moduleSize)
        {
            if (moduleSize < 1)
                throw new ArgumentOutOfRangeException(nameof(moduleSize));

            return Get1bppImage(moduleSize, Color.Black, Color.White);
        }

        /// <summary>
        /// 1bppのシンボル画像を返します。
        /// </summary>
        /// <param name="moduleSize">モジュールサイズ(px)</param>
        /// <param name="foreColor">前景色</param>
        /// <param name="backColor">背景色</param>
        public System.Drawing.Image Get1bppImage(int moduleSize, Color foreColor, Color backColor)
        {
            if (moduleSize < 1 )
                throw new ArgumentOutOfRangeException(nameof(moduleSize));

            byte[] dib = Get1bppDIB(moduleSize, foreColor, backColor);

            ImageConverter converter = new ImageConverter();
            System.Drawing.Image ret = (System.Drawing.Image)converter.ConvertFrom(dib);

            return ret;
        }

        /// <summary>
        /// 24bppのシンボル画像を返します。
        /// </summary>
        public System.Drawing.Image Get24bppImage()
        {
            return Get24bppImage(5);
        }

        /// <summary>
        /// 24bppのシンボル画像を返します。
        /// </summary>
        /// <param name="moduleSize">モジュールサイズ(px)</param>
        public System.Drawing.Image Get24bppImage(int moduleSize)
        {
            if (moduleSize < 1)
                throw new ArgumentOutOfRangeException(nameof(moduleSize));

            return Get24bppImage(moduleSize, Color.Black, Color.White);
        }

        /// <summary>
        /// 24bppのシンボル画像を返します。
        /// </summary>
        /// <param name="moduleSize">モジュールサイズ(px)</param>
        /// <param name="foreColor">前景色</param>
        /// <param name="backColor">背景色</param>
        public System.Drawing.Image Get24bppImage(int moduleSize, Color foreColor, Color backColor)
        {
            if (moduleSize < 1 )
                throw new ArgumentOutOfRangeException(nameof(moduleSize));

            byte[] dib = Get24bppDIB(moduleSize, foreColor, backColor);

            ImageConverter converter = new ImageConverter();
            System.Drawing.Image ret = (System.Drawing.Image)converter.ConvertFrom(dib);

            return ret;
        }

        /// <summary>
        /// 1bppシンボル画像をファイルに保存します。
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        public void Save1bppDIB(string fileName)
        {
            Save1bppDIB(fileName, 5);
        }

        /// <summary>
        /// 1bppシンボル画像をファイルに保存します。
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <param name="moduleSize">モジュールサイズ(px)</param>
        public void Save1bppDIB(string fileName, int moduleSize)
        {
            if (moduleSize < 1)
                throw new ArgumentOutOfRangeException(nameof(moduleSize));

            Save1bppDIB(fileName, moduleSize, Color.Black, Color.White);
        }
            
        /// <summary>
        /// 1bppシンボル画像をファイルに保存します。
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <param name="moduleSize">モジュールサイズ(px)</param>
        /// <param name="foreColor">前景色</param>
        /// <param name="backColor">背景色</param>
        public void Save1bppDIB(string fileName, int moduleSize, Color foreColor, Color backColor)
        {
            if (moduleSize < 1)
                throw new ArgumentOutOfRangeException(nameof(moduleSize));

            byte[] dib = Get1bppDIB(moduleSize, foreColor, backColor);
            File.WriteAllBytes(fileName, dib);
        }

        /// <summary>
        /// 24bppシンボル画像をファイルに保存します。
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        public void Save24bppDIB(string fileName)
        {
            Save24bppDIB(fileName, 5);
        }

        /// <summary>
        /// 24bppシンボル画像をファイルに保存します。
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <param name="moduleSize">モジュールサイズ(px)</param>
        public void Save24bppDIB(string fileName, int moduleSize)
        {
            if (moduleSize < 1)
                throw new ArgumentOutOfRangeException(nameof(moduleSize));

            Save24bppDIB(fileName, moduleSize, Color.Black, Color.White);
        }
        
        /// <summary>
        /// 24bppシンボル画像をファイルに保存します。
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <param name="moduleSize">モジュールサイズ(px)</param>
        /// <param name="foreColor">前景色</param>
        /// <param name="backColor">背景色</param>
        public void Save24bppDIB(string fileName, int moduleSize, Color foreColor, Color backColor)
        {
            if (moduleSize < 1)
                throw new ArgumentOutOfRangeException(nameof(moduleSize));

            byte[] dib = Get24bppDIB(moduleSize, foreColor, backColor);
            File.WriteAllBytes(fileName, dib);
        }
    }
}
