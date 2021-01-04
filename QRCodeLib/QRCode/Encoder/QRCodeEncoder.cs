using System;
using System.Collections.Generic;
using System.Text;

namespace Ys.QRCode.Encoder
{
    /// <summary>
    /// エンコーダーの基本抽象クラス
    /// </summary>
    internal abstract class QRCodeEncoder
    {
        protected List<int> _codeWords   = new List<int>();
        protected int       _charCounter = 0;
        protected int       _bitCounter  = 0;

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        public QRCodeEncoder() { }

        /// <summary>
        /// 文字数を取得します。
        /// </summary>
        public int CharCount => _charCounter;

        /// <summary>
        /// データビット数を取得します。
        /// </summary>
        public int BitCount => _bitCounter;

        /// <summary>
        /// 符号化モードを取得します。
        /// </summary>
        public abstract EncodingMode EncodingMode { get; }

        /// <summary>
        /// モード指示子を取得します。
        /// </summary>
        public abstract int ModeIndicator { get; }

        /// <summary>
        /// 文字を追加します。
        /// </summary>
        public abstract int Append(char c);

        /// <summary>
        /// 指定の文字をエンコードしたコード語のビット数を返します。
        /// </summary>
        public abstract int GetCodewordBitLength(char c);

        /// <summary>
        /// エンコードされたデータのバイト配列を返します。
        /// </summary>
        public abstract byte[] GetBytes();

        /// <summary>
        /// 指定した符号化モードのエンコーダーを返します。
        /// </summary>
        /// <param name="encMode">符号化モード</param>
        /// <param name="byteModeEncoding">バイトモードに適用する文字エンコーディング</param>
        public static QRCodeEncoder CreateEncoder(EncodingMode encMode, 
                                                  Encoding byteModeEncoding)
        {
            switch (encMode)
            {
                case EncodingMode.NUMERIC:
                    return new NumericEncoder();
                case EncodingMode.ALPHA_NUMERIC:
                    return new AlphanumericEncoder();
                case EncodingMode.EIGHT_BIT_BYTE:
                    return new ByteEncoder(byteModeEncoding);
                case EncodingMode.KANJI:
                    return new KanjiEncoder();
                default:
                    throw new ArgumentOutOfRangeException(nameof(encMode));
            }
        }
    }
}
