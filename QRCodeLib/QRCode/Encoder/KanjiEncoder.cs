using System;
using System.Text;

using Ys.Misc;

namespace Ys.QRCode.Encoder
{
    /// <summary>
    /// 漢字モードエンコーダー
    /// </summary>
    internal class KanjiEncoder : QRCodeEncoder
    {
        private readonly AlphanumericEncoder _encAlpha;

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        public KanjiEncoder(Encoding encoding) : base(encoding) 
        { 
            _encAlpha = new AlphanumericEncoder(encoding);
        }

        /// <summary>
        /// 符号化モードを取得します。
        /// </summary>
        public override EncodingMode EncodingMode => EncodingMode.KANJI;

        /// <summary>
        /// モード指示子を取得します。
        /// </summary>
        public override int ModeIndicator => Format.ModeIndicator.KANJI_VALUE;

        /// <summary>
        /// 文字を追加します。
        /// </summary>
        public override void Append(char c)
        {
            byte[] charBytes = _encoding.GetBytes(c.ToString());
            int wd = (charBytes[0] << 8) | charBytes[1];

            if (0x8140 <= wd && wd <= 0x9FFC)
                wd -= 0x8140;
            else if (0xE040 <= wd && wd <= 0xEBBF)
                wd -= 0xC140;
            else
                throw new ArgumentOutOfRangeException(nameof(c));

            wd = ((wd >> 8) * 0xC0) + (wd & 0xFF);
            _codeWords.Add(wd);

            _bitCounter += GetCodewordBitLength(c);
            _charCounter++;
        }

        /// <summary>
        /// 指定の文字をエンコードしたコード語のビット数を返します。
        /// </summary>
        public override int GetCodewordBitLength(char c)
        {
            return 13;
        }

        /// <summary>
        /// エンコードされたデータのバイト配列を返します。
        /// </summary>
        public override byte[] GetBytes()
        {
            var bs = new BitSequence();

            foreach (int wd in _codeWords)
                bs.Append(wd, 13);

            return bs.GetBytes();
        }

        /// <summary>
        /// 指定した文字が、このモードの文字集合に含まれる場合は true を返します。
        /// </summary>
        public override bool InSubset(char c)
        {
            byte[] charBytes = _encoding.GetBytes(c.ToString());

            if (charBytes.Length != 2)
                return false;

            int code = (charBytes[0] << 8) | charBytes[1];

            if (0x8140 <= code && code <= 0x9FFC || 
                0xE040 <= code && code <= 0xEBBF)
            {
                return 0x40 <= charBytes[1] && charBytes[1] <= 0xFC &&
                       0x7F != charBytes[1];
            }

            return false;
        }

        /// <summary>
        /// 指定した文字が、このモードの排他的部分文字集合に含まれる場合は true を返します。
        /// </summary>
        public override bool InExclusiveSubset(char c)
        {
            if (_encAlpha.InSubset(c))
                return false;

            return InSubset(c);
        }
    }
}
