using System;
using System.Text;

using Ys.Misc;

namespace Ys.QRCode.Encoder
{
    /// <summary>
    /// 英数字モードエンコーダー
    /// </summary>
    internal class AlphanumericEncoder : QRCodeEncoder
    {
        private readonly NumericEncoder _encNumeric;

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        public AlphanumericEncoder(Encoding encoding) : base(encoding) 
        { 
            _encNumeric = new NumericEncoder(encoding);
        }

        /// <summary>
        /// 符号化モードを取得します。
        /// </summary>
        public override EncodingMode EncodingMode => EncodingMode.ALPHA_NUMERIC;

        /// <summary>
        /// モード指示子を取得します。
        /// </summary>
        public override int ModeIndicator => Format.ModeIndicator.ALPAHNUMERIC_VALUE;

        /// <summary>
        /// 文字を追加します。
        /// </summary>
        public override void Append(char c)
        {
            int wd = ConvertCharCode(c);

            if (_charCounter % 2 == 0)
                _codeWords.Add(wd);
            else
            {
                _codeWords[_codeWords.Count - 1] *= 45;
                _codeWords[_codeWords.Count - 1] += wd;
            }

            _bitCounter += GetCodewordBitLength(c);
            _charCounter++;
        }

        /// <summary>
        /// 指定の文字をエンコードしたコード語のビット数を返します。
        /// </summary>
        public override int GetCodewordBitLength(char c)
        {
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

            if (_charCounter % 2 == 0)
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
            if ('0' <= c && c <= '9')
                return c - 48;
            if (c == ':')
                return 44;
            if ('A' <= c && c <= 'Z')
                return c - 55;

            return -1;
        }

        /// <summary>
        /// 指定した文字が、このモードの文字集合に含まれる場合は true を返します。
        /// </summary>
        public override bool InSubset(char c)
        {
            return ConvertCharCode(c) > -1;
        }

        /// <summary>
        /// 指定した文字が、このモードの排他的部分文字集合に含まれる場合は true を返します。
        /// </summary>
        public override bool InExclusiveSubset(char c)
        {
            if (_encNumeric.InSubset(c))
                return false;

            return InSubset(c);
        }
    }
}
