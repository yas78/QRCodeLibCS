using System;
using System.Diagnostics;

using Ys.Util;

namespace Ys.QRCode.Encoder
{
    /// <summary>
    /// 英数字モードエンコーダー
    /// </summary>
    internal class AlphanumericEncoder : QRCodeEncoder
    {
        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        public AlphanumericEncoder() { }
        
        /// <summary>
        /// 符号化モードを取得します。
        /// </summary>
        public override EncodingMode EncodingMode
        {
            get { return EncodingMode.ALPHA_NUMERIC; }
        }

        /// <summary>
        /// モード指示子を取得します。
        /// </summary>
        public override int ModeIndicator
        {
            get { return Ys.QRCode.Format.ModeIndicator.ALPAHNUMERIC_VALUE; }
        }
        
        /// <summary>
        /// 文字を追加します。
        /// </summary>
        /// <returns>追加した文字のビット数</returns>
        public override int Append(char c)
        {
            Debug.Assert(IsInSubset(c));

            int wd = ConvertCharCode(c);
            int ret;

            if (_charCounter % 2 == 0)
            {
                _codeWords.Add(wd);
                ret = 6;
            }
            else
            {
                _codeWords[_codeWords.Count - 1] *= 45;
                _codeWords[_codeWords.Count - 1] += wd;
                ret = 5;
            }

            _charCounter++;
            _bitCounter += ret;

            return ret;
        }

        /// <summary>
        /// 指定の文字をエンコードしたコード語のビット数を返します。
        /// </summary>
        public override int GetCodewordBitLength(char c)
        {
            Debug.Assert(IsInSubset(c));

            if (_charCounter % 2 == 0)
                return 6;
            else
                return 5;
        }

        /// <summary>
        /// エンコードされたデータのバイト配列を返します。
        /// </summary>
        public override byte[] GetBytes()
        {
            var bs = new BitSequence();
            int bitLength = 11; 

            for (int i = 0; i <= (_codeWords.Count - 1) - 1; ++i)
                bs.Append(_codeWords[i], bitLength);
            
            if ((_charCounter % 2) == 0)
                bitLength = 11;
            else
                bitLength = 6;
            
            bs.Append(_codeWords[_codeWords.Count - 1], bitLength);

            return bs.GetBytes();
        }
        
        /// <summary>
        /// 指定した文字の、英数字モードにおけるコード値を返します。
        /// </summary>
        private static int ConvertCharCode(char c)
        {
            if (c >= 'A' && c <= 'Z')
                return c - 55;

            if (c >= '0' && c <= '9')
                return c - 48;

            if (c == ' ')
                return 36;

            if (c == '$' || c == '%')
                return c + 1;

            if (c == '*' || c == '+')
                return c - 3;

            if (c == '-' || c == '.')
                return c - 4;

            if (c == '/')
                return 43;

            if (c == ':')
                return 44;

            throw new ArgumentOutOfRangeException(nameof(c));
        }

        /// <summary>
        /// 指定した文字が、このモードの文字集合に含まれる場合は true を返します。
        /// </summary>
        public static bool IsInSubset(char c)
        {
            return c >= 'A' && c <= 'Z' ||
                   c >= '0' && c <= '9' ||
                   c == ' '             ||
                   c == '.'             ||
                   c == '-'             ||
                   c == '$'             ||
                   c == '%'             ||
                   c == '*'             ||
                   c == '+'             ||
                   c == '/'             ||
                   c == ':';
        }

        /// <summary>
        /// 指定した文字が、このモードの排他的部分文字集合に含まれる場合は true を返します。
        /// </summary>
        public static bool IsInExclusiveSubset(char c)
        {
            if (NumericEncoder.IsInSubset(c))
                return false;
            
            return IsInSubset(c);
        }
    }
}
