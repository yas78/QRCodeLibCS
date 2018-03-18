using System;
using System.Diagnostics;
using System.Text;

using Ys.Util;

namespace Ys.QRCode.Encoder
{
    /// <summary>
    /// 漢字モードエンコーダー
    /// </summary>
    internal class KanjiEncoder : QRCodeEncoder
    {
        static readonly Encoding _textEncoding = 
            Encoding.GetEncoding("shift_jis");

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        public KanjiEncoder() { }
        
        /// <summary>
        /// 符号化モードを取得します。
        /// </summary>
        public override EncodingMode EncodingMode
        {
            get { return EncodingMode.KANJI; }
        }

        /// <summary>
        /// モード指示子を取得します。
        /// </summary>
        public override int ModeIndicator
        {
            get { return Ys.QRCode.Format.ModeIndicator.KANJI_VALUE; }
        }
        
        /// <summary>
        /// 文字を追加します。
        /// </summary>
        /// <returns>追加した文字のビット数</returns>
        public override int Append(char c)
        {
            Debug.Assert(IsInSubset(c));

            byte[] charBytes = _textEncoding.GetBytes(c.ToString());
            int wd = (charBytes[0] << 8) | charBytes[1];

            if (wd >= 0x8140 && wd <= 0x9FFC)
                wd -= 0x8140;

            else if (wd >= 0xE040 && wd <= 0xEBBF)
                wd -= 0xC140;

            else
                throw new ArgumentOutOfRangeException(nameof(c));

            wd = ((wd >> 8) * 0xC0) + (wd & 0xFF);

            _codeWords.Add(wd);
            _charCounter++;
            _bitCounter += 13;

            return 13;
        }

        /// <summary>
        /// 指定の文字をエンコードしたコード語のビット数を返します。
        /// </summary>
        public override int GetCodewordBitLength(char c)
        {
            Debug.Assert(IsInSubset(c));

            return 13;
        }

        /// <summary>
        /// エンコードされたデータのバイト配列を返します。
        /// </summary>
        public override byte[] GetBytes()
        {
            var bs = new BitSequence();

            for (int i = 0; i < _codeWords.Count; ++i)
                bs.Append(_codeWords[i], 13);

            return bs.GetBytes();
        }

        /// <summary>
        /// 指定した文字が、このモードの文字集合に含まれる場合は true を返します。
        /// </summary>
        public static bool IsInSubset(char c)
        {
            byte[] charBytes = _textEncoding.GetBytes(c.ToString());

            if (charBytes.Length != 2)
                return false;

            int code = (charBytes[0] << 8) | charBytes[1];
            
            if (code >= 0x8140 && code <= 0x9FFC || 
                code >= 0xE040 && code <= 0xEBBF)
            {
                return charBytes[1] >= 0x40 &&
                       charBytes[1] <= 0xFC &&
                       charBytes[1] != 0x7F;
            }
            else
                return false;
        }

        /// <summary>
        /// 指定した文字が、このモードの排他的部分文字集合に含まれる場合は true を返します。
        /// </summary>
        public static bool IsInExclusiveSubset(char c)
        {
            return IsInSubset(c);
        }
    }
}
