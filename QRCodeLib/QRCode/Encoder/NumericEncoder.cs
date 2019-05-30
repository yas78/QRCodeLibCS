using System;
using System.Diagnostics;

using Ys.Util;

namespace Ys.QRCode.Encoder
{
    /// <summary>
    /// 数字モードエンコーダー
    /// </summary>
    internal class NumericEncoder : QRCodeEncoder
    {
        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        public NumericEncoder(){ }
        
        /// <summary>
        /// 符号化モードを取得します。
        /// </summary>
        public override EncodingMode EncodingMode
        {
            get { return EncodingMode.NUMERIC; }
        }

        /// <summary>
        /// モード指示子を取得します。
        /// </summary>
        public override int ModeIndicator
        {
            get { return Ys.QRCode.Format.ModeIndicator.NUMERIC_VALUE; }
        }
        
        /// <summary>
        /// 文字を追加します。
        /// </summary>
        /// <returns>追加した文字のビット数</returns>
        public override int Append(char c)
        {
            int wd = Int32.Parse(c.ToString());
            int ret;

            if (_charCounter % 3 == 0)
            {
                _codeWords.Add(wd);
                ret = 4;
            }
            else
            {
                _codeWords[_codeWords.Count - 1] *= 10;
                _codeWords[_codeWords.Count - 1] += wd;
                ret = 3;
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
            if (_charCounter % 3 == 0)
                return 4;
            else
                return 3;
        }

        /// <summary>
        /// エンコードされたデータのバイト配列を返します。
        /// </summary>
        public override byte[] GetBytes()
        {
            var bs = new BitSequence();
            int bitLength = 10; 

            for (int i = 0; i <= (_codeWords.Count - 1) - 1; ++i)
                bs.Append(_codeWords[i], bitLength);

            switch (_charCounter % 3)
            {
                case 1:
                    bitLength = 4;
                    break;

                case 2:
                    bitLength = 7;
                    break;

                default:
                    bitLength = 10;
                    break;
            }

            bs.Append(_codeWords[_codeWords.Count - 1], bitLength);

            return bs.GetBytes();
        }

        /// <summary>
        /// 指定した文字が、このモードの文字集合に含まれる場合は true を返します。
        /// </summary>
        public static bool InSubset(char c)
        {
            return '0' <= c && c <= '9';
        }

        /// <summary>
        /// 指定した文字が、このモードの排他的部分文字集合に含まれる場合は true を返します。
        /// </summary>
        public static bool InExclusiveSubset(char c)
        {
            return InSubset(c);
        }
    }
}
