using System;
using System.Text;

namespace Ys.QRCode.Encoder
{
    /// <summary>
    /// バイトモードエンコーダー
    /// </summary>
    internal class ByteEncoder : QRCodeEncoder
    {
        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        public ByteEncoder() : this(Encoding.GetEncoding("shift_jis")) { }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="encoding">文字エンコーディング</param>
        public ByteEncoder(Encoding encoding)
        {
            _textEncoding = encoding;
        }

        readonly Encoding _textEncoding;
        
        /// <summary>
        /// 符号化モードを取得します。
        /// </summary>
        public override EncodingMode EncodingMode => EncodingMode.EIGHT_BIT_BYTE;

        /// <summary>
        /// モード指示子を取得します。
        /// </summary>
        public override int ModeIndicator => Format.ModeIndicator.BYTE_VALUE;
        
        /// <summary>
        /// 文字を追加します。
        /// </summary>
        /// <returns>追加した文字のビット数</returns>
        public override int Append(char c)
        {
            byte[] charBytes = _textEncoding.GetBytes(c.ToString());
            int ret = 0;

            foreach  (byte value in charBytes)
            {
                _codeWords.Add(value);
                _charCounter++;
                _bitCounter += 8;
                ret += 8;
            }

            return ret;
        }

        /// <summary>
        /// 指定の文字をエンコードしたコード語のビット数を返します。
        /// </summary>
        public override int GetCodewordBitLength(char c)
        {
            byte[] charBytes = _textEncoding.GetBytes(c.ToString());

            return 8 * charBytes.Length;
        }

        /// <summary>
        /// エンコードされたデータのバイト配列を返します。
        /// </summary>
        public override byte[] GetBytes()
        {
            byte[] ret = new byte[_charCounter];

            for (int i = 0; i < _codeWords.Count; ++i)
                ret[i] = (byte)_codeWords[i];

            return ret;
        }

        /// <summary>
        /// 指定した文字が、このモードの文字集合に含まれる場合は true を返します。
        /// </summary>
        public static bool InSubset(char c)
        {
            return true;
        }

        /// <summary>
        /// 指定した文字が、このモードの排他的部分文字集合に含まれる場合は true を返します。
        /// </summary>
        public static bool InExclusiveSubset(char c)
        {
            if (AlphanumericEncoder.InSubset(c))
                return false;

            if (KanjiEncoder.InSubset(c))
                return false;

            return InSubset(c);
        }
    }
}
