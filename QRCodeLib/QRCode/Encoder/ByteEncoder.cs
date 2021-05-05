using System;
using System.Text;

using Ys.Misc;

namespace Ys.QRCode.Encoder
{
    /// <summary>
    /// バイトモードエンコーダー
    /// </summary>
    internal class ByteEncoder : QRCodeEncoder
    {
        private readonly AlphanumericEncoder _encAlpha;
        private readonly KanjiEncoder        _encKanji;

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        public ByteEncoder(Encoding encoding) : base(encoding) 
        {
            _encAlpha = new AlphanumericEncoder(encoding);

            if (Charset.IsJP(encoding.WebName))
                _encKanji = new KanjiEncoder(encoding);
        }

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
        public override void Append(char c)
        {
            byte[] charBytes = _encoding.GetBytes(c.ToString());

            foreach  (byte value in charBytes)
                _codeWords.Add(value);

            _bitCounter += GetCodewordBitLength(c);
            _charCounter += charBytes.Length;
        }

        /// <summary>
        /// 指定の文字をエンコードしたコード語のビット数を返します。
        /// </summary>
        public override int GetCodewordBitLength(char c)
        {
            byte[] charBytes = _encoding.GetBytes(c.ToString());

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
        public override bool InSubset(char c)
        {
            return true;
        }

        /// <summary>
        /// 指定した文字が、このモードの排他的部分文字集合に含まれる場合は true を返します。
        /// </summary>
        public override bool InExclusiveSubset(char c)
        {
            if (_encAlpha.InSubset(c))
                return false;

            if (_encKanji != null)
            {
                if (_encKanji.InSubset(c))
                    return false;
            }

            return InSubset(c);
        }
    }
}
